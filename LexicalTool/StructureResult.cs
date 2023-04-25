using Ferestar.Lib.LexicalCompiler.CommandModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ferestar.Lib.LexicalCompiler.LexicalTools
{
    public class StructureResult
    {
        private TerminalSymbol terminal_sysntax;
        private readonly string key = "@Var@Ferestar@ABB@";
        private List<string> LexicalWords;
        private List<DataContent> LexicalObjects;
        private Dictionary<string, string> ValueDictionary;
        private Dictionary<int, int> MarkOpenAndCloseRegionIndex;
        public string CommandText { get; set; }
        public List<Command> CommandPriority { get; set; }
        public CommandParameter RootVariable { get; set; }
        public int LineNumber { get; set; }
        public OperationResult CompileLineStatus { get; set; }
        internal StructureResult(string Statemets , int lineNumber)
        {
            this.CommandText = Statemets;
            this.LineNumber = lineNumber;
            this.terminal_sysntax = TerminalSymbol.DefaultTerminalSymbols;
            this.CommandPriority = new List<Command>();
            ValueDictionary = new Dictionary<string, string>();
            this.CompileLineStatus = new OperationResult()
            {
                ExceptionResult = null,
                CompileStatus = true
            };

        }
        internal void Compile()
        {
            StringBuilder grammar = new StringBuilder("");
            LexicalWords = SeprationLexical(CommandText);
            LexicalObjects = GetLexical(LexicalWords, out grammar);
            ControlGrammar(grammar, LexicalObjects);
            MarkOpenAndCloseRegionIndex = MarkOpenAndCloseRegion(LexicalWords, 0);
            RootVariable = GetTreeCommand(null, 0);
            if (LexicalAnalyser.AllCompiledLexicalCommand.Results.Any())
            {
                if (LexicalAnalyser.AllCompiledLexicalCommand.Results.Any(x => x.RootVariable.Name.Content == RootVariable.Name.Content && x.LineNumber != LineNumber))
                    throw new Exception("line:(" + LineNumber + ")  A local " + RootVariable.Name.Type.ToString() + " with name: '" + RootVariable.Name.Content + "' is already defined in this scope \n");

            }
            LexicalAnalyser.AllCompiledLexicalCommand.AddResult(this);
        }
        private StringBuilder ConvertToStandardLanguage(StringBuilder statement, bool justStringRegion=false)
        {
            StringBuilder standard=new StringBuilder("");
            StringBuilder regexMap = new StringBuilder("");
            int index = 0;
            var sysmbols = terminal_sysntax.SimilarSymbol.Where(x => x.Value != terminal_sysntax.StringRegion) ;
            if (justStringRegion)
                sysmbols = terminal_sysntax.SimilarSymbol.Where(x => x.Value == terminal_sysntax.StringRegion);
            foreach (var item in sysmbols)
            {
                regexMap.Append("\\" + item.Key + "|");
            }
            regexMap.Append(";");
            Match parameterValue = Regex.Match(statement.ToString(), regexMap.ToString());
            while (parameterValue.Success)
            {
                standard.Append(string.Concat(statement.ToString().Substring(index, parameterValue.Index - index), terminal_sysntax.SimilarSymbol.GetValueOrDefault(parameterValue.Value)));
                index = parameterValue.Index + parameterValue.Length;
                parameterValue = parameterValue.NextMatch();
            }
            standard.Append(statement.ToString().Substring(index, statement.Length - index));
            return standard;

        }
        /// <summary>
        /// sprate all off grammer's components for compile
        /// then all of the components store in Dictionary
        /// </summary>
        /// <param name="text">statement without ;.</param>
        /// <returns></returns>
        private List<string> SeprationLexical(string stringText)
        {

            string unique;
            StringBuilder text = new StringBuilder(stringText);
            StringBuilder new_text = new StringBuilder("");
            List<Match> machs = new List<Match>();
            text = ConvertToStandardLanguage(text,true);
            Match parameterValue = Regex.Match(text.ToString(), "\\" + terminal_sysntax.StringRegion + "[^" + terminal_sysntax.StringRegion + "]*\\" + terminal_sysntax.StringRegion);
            int index = 0;

            //replace key with string values in statements
            while (parameterValue.Success)
            {

                unique = key + Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", string.Empty).Replace("+", string.Empty);
                ValueDictionary.Add(unique, parameterValue.Value);
                new_text.Append(string.Concat(text.ToString().Substring(index, parameterValue.Index - index), unique));
                index = parameterValue.Index + parameterValue.Length;
                parameterValue = parameterValue.NextMatch();

            }
            

            

            new_text .Append( text.ToString().Substring(index, text.Length - index));
            new_text = ConvertToStandardLanguage(new_text);

            ////arrange statents
            new_text.Replace(System.Environment.NewLine, string.Empty)
            .Replace(" ", string.Empty)
            .Replace(terminal_sysntax.Assign, string.Concat(' ', terminal_sysntax.Assign, ' '))
            .Replace(terminal_sysntax.OpenRegion, string.Concat(' ', terminal_sysntax.OpenRegion, ' '))
            .Replace(terminal_sysntax.CloseRegion, string.Concat(' ', terminal_sysntax.CloseRegion, ' '))
            .Replace(terminal_sysntax.Seprator, string.Concat(' ', terminal_sysntax.Seprator, ' '))
            .Replace(terminal_sysntax.EndStatement, string.Concat(' ', terminal_sysntax.EndStatement, ' ')).ToString();
            return new_text.ToString().Split().Where(x => x != string.Empty).ToList();
        }

        /// <summary>
        /// get outer index in statements
        /// </summary>
        /// <returns></returns>
        private List<int> GetStartIndexOfParameterInOuterLexicalList(List<string> lexical, int start_index)
        {
            List<int> parametr_indexes = new List<int>();

            int countAll = lexical.Count;
            int openRegion = lexical.IndexOf(terminal_sysntax.OpenRegion, start_index) + 1;
            int closeRegion = MarkOpenAndCloseRegionIndex.GetValueOrDefault(lexical.IndexOf(terminal_sysntax.OpenRegion, start_index));
            for (int i = openRegion; i < closeRegion; i++)
            {
                int jump = 0;
                if (MarkOpenAndCloseRegionIndex.TryGetValue(i, out jump))
                {
                    i = jump;

                }
                if (lexical[i] == terminal_sysntax.Assign)
                    parametr_indexes.Add(i - 1);
            }

            return parametr_indexes;
        }
        /// <summary>
        /// for control the parentheses in statements
        /// </summary>
        /// <param name="lexical"></param>
        /// <param name="start_index"></param>
        /// <returns></returns>
        private Dictionary<int, int> MarkOpenAndCloseRegion(List<string> lexical, int start_index)
        {
            Dictionary<int, int> startAndEnd = new Dictionary<int, int>();
            Stack<int> openAndCloseStack = new Stack<int>();
            int open_index = lexical.IndexOf(terminal_sysntax.OpenRegion, start_index);
            int close_index = lexical.IndexOf(terminal_sysntax.CloseRegion, start_index);
            openAndCloseStack.Push(open_index);
            while (close_index != -1)
            {
                var next_open_index = lexical.IndexOf(terminal_sysntax.OpenRegion, open_index + 1);
                if (next_open_index < close_index && next_open_index != -1)
                {

                    open_index = lexical.IndexOf(terminal_sysntax.OpenRegion, open_index + 1);
                    openAndCloseStack.Push(open_index);
                    continue;
                }
                else
                {
                    startAndEnd.Add(openAndCloseStack.Pop(), close_index);
                    close_index = lexical.IndexOf(terminal_sysntax.CloseRegion, close_index + 1);

                }
                if (openAndCloseStack.Count == 0)
                    break;
            }
            return startAndEnd;
        }
        /// <summary>
        /// Grammer for control this syntax:
        /// S->McrAw
        /// A->B/BoA/S
        /// B->Md/{}
        /// M->ab
        /// ----------
        /// c is function or Command name
        /// r is Open Region eg { or (
        /// w is Close Region eg } or )
        /// d is parameter Value or data between  ' '
        /// a is Varible name or parameter name 
        /// b is Assign symbol eg = or :
        /// o is seprator 
        /// 
        /// </summary>
        /// <param name="lexical"></param>
        private List<DataContent> GetLexical(List<string> lexical, out StringBuilder grammar)
        {
            List<DataContent> words = new List<DataContent>();
            grammar = new StringBuilder("");
            int wordCount = lexical.Count;
            for (int i = 0; i < wordCount; i++)
            {
                string word = lexical[i];
                if (word == terminal_sysntax.Assign)
                {
                    words.Add(new DataContent(lexical[i - 1], LexicalType.VARIABLE));
                    grammar .Append( "a");
                    words.Add(new DataContent(word, LexicalType.ASSYGN));
                    grammar.Append("b");
                    if (lexical[i + 1].Contains(key))
                    {
                        words.Add(new DataContent(ValueDictionary[lexical[i + 1]], LexicalType.VALUE));
                        grammar.Append("d");
                    }
                    else
                    {
                        if (lexical[i + 2] == terminal_sysntax.OpenRegion)
                        {
                            words.Add(new DataContent(lexical[i + 1], LexicalType.FUNCTION_NAME));
                            grammar.Append("c");
                        }
                        else
                        {
                            words.Add(new DataContent(lexical[i + 1], LexicalType.VARIABLE));
                            grammar.Append("a");
                        }
                    }
                    continue;
                }
                if (word == terminal_sysntax.OpenRegion)
                {
                    words.Add(new DataContent(word, LexicalType.OPEN_REGION));
                    grammar.Append("r");

                    if (lexical[i + 1] != terminal_sysntax.CloseRegion && lexical[i + 2] != terminal_sysntax.Assign)
                    {
                        words.Add(new DataContent(string.Empty, LexicalType.VARIABLE));
                        grammar.Append("a");
                        words.Add(new DataContent(terminal_sysntax.Assign, LexicalType.ASSYGN));
                        grammar.Append("b");
                        if (lexical[i + 1].Contains(key))
                        {
                            words.Add(new DataContent(ValueDictionary[lexical[i + 1]], LexicalType.VALUE));
                            grammar.Append("d");
                        }
                        else
                        {
                            if (lexical[i + 2] == terminal_sysntax.OpenRegion)
                            {
                                words.Add(new DataContent(lexical[i + 1], LexicalType.FUNCTION_NAME));
                                grammar.Append("c");
                            }
                            else
                            {
                                words.Add(new DataContent(lexical[i + 1], LexicalType.VARIABLE));
                                grammar.Append("a");
                            }
                        }
                    }
                    continue;
                }
                if (word == terminal_sysntax.CloseRegion)
                {
                    words.Add(new DataContent(word, LexicalType.CLOSE_REGION));
                    grammar.Append("w");
                    continue;
                }
                if (word == terminal_sysntax.Seprator)
                {
                    words.Add(new DataContent(word, LexicalType.PARAMETER_SEPERATO));
                    grammar.Append("o");
                    if (lexical[i + 2] != terminal_sysntax.Assign)
                    {
                        words.Add(new DataContent(string.Empty, LexicalType.VARIABLE));
                        grammar.Append("a");
                        words.Add(new DataContent(terminal_sysntax.Assign, LexicalType.ASSYGN));
                        grammar.Append("b");
                        if (lexical[i + 1].Contains(key))
                        {
                            words.Add(new DataContent(ValueDictionary[lexical[i + 1]], LexicalType.VALUE));
                            grammar.Append("d");
                        }
                        else
                        {
                            if (lexical[i + 2] == terminal_sysntax.OpenRegion)
                            {
                                words.Add(new DataContent(lexical[i + 1], LexicalType.FUNCTION_NAME));
                                grammar.Append("c");
                            }
                            else
                            {
                                words.Add(new DataContent(lexical[i + 1], LexicalType.VARIABLE));
                                grammar.Append("a");
                            }
                        }
                    }
                    continue;
                }
                if (word == terminal_sysntax.EndStatement)
                {
                    words.Add(new DataContent(word, LexicalType.END_LINE));
                    grammar.Append("e");
                    continue;
                }
            }
            grammar = grammar.Replace("ab", "l");// l=litral or variable eg "aa="
            grammar = grammar.Replace("ld", "v");// v=litral and data eg "aa='hello'"
            grammar = grammar.Replace("la", "s");// s=litral and litral eg "aa=bb"
            grammar = grammar.Replace("lcr", "f");// f=litral and function eg "aa=F("
            LexicalWords = words.Select(x => x.Content).ToList();
            return words;
        }
        /// <summary>
        /// Grammer for control this syntax:
        /// S->McrAw
        /// A->B/BoA/S/So
        /// B->Md/Ma/{}
        /// M->ab
        /// ----------
        /// c is function or Command name
        /// r is Open Region eg { or (
        /// w is Close Region eg } or )
        /// d is parameter Value or data between  ' '
        /// a is Varible name or parameter name 
        /// b is Assign symbol eg = or :
        /// 
        /// </summary>
        /// <param name="lexicals"></param>
        /// <param name="grammar">generated per lexical command</param>
        private void ControlGrammar(StringBuilder grammar, List<DataContent> lexicals)
        {
            StringBuilder errorMessage =new StringBuilder(string.Empty);
            if (grammar.Equals(string.Empty))
            {
                errorMessage .Append("Syntax structure not true\n");
                throw new Exception("line:(" + LineNumber + ") " + errorMessage);
            }

            if (grammar[0] == 'v' || grammar[0] == 'f' || grammar[0] == 's')
            {


                Stack<int> controlRegionStack = new Stack<int>();

                int counter = 0, grammarLen = grammar.Length;
                for (int i = 0; i < grammarLen; i++)
                {

                    char item = grammar[i];
                    char nextItem = 'e';
                    if (grammar.Length > i + 1)
                        nextItem = grammar[i + 1];
                    if (item == 'f')
                    {
                        counter += 4;
                        controlRegionStack.Push(counter - 4);
                        string statement = lexicals[counter - 4].Content + lexicals[counter - 3].Content + lexicals[counter - 2].Content + lexicals[counter - 1].Content;
                        if (nextItem == 'e')
                            errorMessage.Append(terminal_sysntax.CloseRegion + " is expected near " + lexicals[counter - 1].Type.ToString() + " '" + statement + "\n");


                        if (nextItem == 'w')
                        {

                            continue;
                        }
                        else if (nextItem == 'v')
                        {
                            continue;
                        }
                        else if (nextItem == 's')
                        {
                            continue;
                        }
                        else if (nextItem == 'f')
                        {
                            continue;
                        }
                        else
                        {
                            errorMessage.Append("syntax error near function " + lexicals[counter - 1].Type.ToString() + " '" + statement + "' " + "\n");
                        }
                    }
                    else if (item == 'w')
                    {
                        counter++;
                        string statement = lexicals[counter - 4].Content + lexicals[counter - 3].Content + lexicals[counter - 2].Content + lexicals[counter - 1].Content;

                        if (!controlRegionStack.TryPop(out int opened_index))
                            errorMessage.Append(terminal_sysntax.CloseRegion + " is expected near " + lexicals[counter - 1].Type.ToString() + " '" + statement + "'" + "\n");
                        if (nextItem == 'o')
                        {

                            continue;
                        }
                        else if (nextItem == 'w')
                        {
                            continue;
                        }
                        else if (nextItem == 'e')
                        {
                            continue;
                        }
                        else
                        {
                            errorMessage.Append("syntax error near close region " + lexicals[counter - 1].Type.ToString() + " '" + statement + "'" + "\n");
                        }
                    }
                    else if (item == 'v')// v=litral and data eg "aa='hello'"
                    {
                        counter += 3;
                        string statement = lexicals[counter - 3].Content + lexicals[counter - 2].Content + lexicals[counter - 1].Content;
                        if (nextItem == 'o')
                        {

                            continue;
                        }
                        else if (nextItem == 'w')
                        {
                            continue;
                        }

                        else
                        {
                            errorMessage.Append("syntax error near set data " + lexicals[counter - 1].Type.ToString() + " '" + statement + "'" + "\n");
                        }

                    }
                    else if (item == 's')// v=litral and litral eg "aa=bb"
                    {
                        counter += 3;
                        string statement = lexicals[counter - 3].Content + lexicals[counter - 2].Content + lexicals[counter - 1].Content;
                        if (nextItem == 'o')
                        {

                            continue;
                        }
                        else if (nextItem == 'w')
                        {
                            continue;
                        }

                        else
                        {
                            errorMessage.Append("syntax error near set data " + lexicals[counter - 1].Type.ToString() + " '" + statement + "'" + "\n");
                        }

                    }
                    else if (item == 'o')
                    {
                        counter++;
                        string statement = lexicals[counter - 5].Content + lexicals[counter - 4].Content + lexicals[counter - 3].Content + lexicals[counter - 2].Content + lexicals[counter - 1].Content;
                        if (nextItem == 'f')
                        {

                            continue;
                        }
                        else if (nextItem == 'v')
                        {
                            continue;
                        }
                        else if (nextItem == 's')
                        {
                            continue;
                        }
                        else
                        {
                            errorMessage.Append("syntax error near seprator " + lexicals[counter - 1].Type.ToString() + " '" + statement + "'" + "\n");
                        }

                    }
                    else
                    {
                        errorMessage.Append("syntax error near " + lexicals[counter].Type.ToString() + "'" + lexicals[counter].Content + "'" + "\n");
                    }
                }
                int start_index = 0;
                if (controlRegionStack.TryPop(out start_index))
                {
                    StringBuilder statement = new StringBuilder("");
                    foreach (DataContent item in lexicals)
                    {
                        statement.Append(item.Content);
                    }
                    errorMessage.Append("syntax error near " + lexicals[start_index].Type.ToString() + " '" + lexicals[start_index].Content + "' not closed region " + terminal_sysntax.CloseRegion + " is expected near '" + statement + "'" + "\n");
                }
            }
            if (!errorMessage.Equals(string.Empty))
            {
                throw new Exception("line:(" + LineNumber + ") " + errorMessage.ToString() + "\n", new Exception(string.Join("", lexicals.Select(p => p.Content))));
            }
        }
        /// <summary>
        /// generate tree
        /// </summary>
        /// <param name="lexical"></param>
        /// <param name="paramter"></param>
        /// <param name="start_index"></param>
        /// <returns></returns>
        private CommandParameter GetTreeCommand(CommandParameter paramter, int start_index)
        {

            Command com = new Command(LexicalObjects[start_index + 2], new List<CommandParameter>(), null);
            paramter = new CommandParameter(null, LexicalObjects[start_index], com);
            com.AssignToParameter = paramter;
            List<int> parameters_indexes = GetStartIndexOfParameterInOuterLexicalList(LexicalWords, start_index);
            foreach (int parameter_index in parameters_indexes)
            {

                //اگر پارامتر های متد تمام شد0 یا دارای پارامتر دیگری است مقدار پارامتر را اختصاص بده
                if (LexicalObjects[parameter_index + 3].Type == LexicalType.CLOSE_REGION || LexicalObjects[parameter_index + 3].Type == LexicalType.PARAMETER_SEPERATO)
                {
                    DataContent lexicalAssigned = LexicalObjects[parameter_index + 2];
                    if (lexicalAssigned.Type == LexicalType.VALUE)
                    {
                        paramter.Command.Parameters.Add(new CommandParameter(lexicalAssigned, LexicalObjects[parameter_index], null));
                    }
                    else if (lexicalAssigned.Type == LexicalType.VARIABLE)
                    {
                        if (!LexicalAnalyser.AllCompiledLexicalCommand.Results.Any())
                            throw new Exception("line:(" + LineNumber + ") " + lexicalAssigned.Type.ToString() + " '" + lexicalAssigned.Content + "' undefined in " + LexicalObjects[parameter_index].Content + LexicalObjects[parameter_index + 1].Content + LexicalObjects[parameter_index + 2].Content + LexicalObjects[parameter_index + 3].Content + "\n");

                        StructureResult assignedRootParameter = LexicalAnalyser.AllCompiledLexicalCommand.Results.Where(x => x.RootVariable.Name.Content == lexicalAssigned.Content).SingleOrDefault();
                        if (assignedRootParameter == null)
                            throw new Exception("line:(" + LineNumber + ") " + lexicalAssigned.Type.ToString() + " '" + lexicalAssigned.Content + "' undefined in " + LexicalObjects[parameter_index].Content + LexicalObjects[parameter_index + 1].Content + LexicalObjects[parameter_index + 2].Content + LexicalObjects[parameter_index + 3].Content + "\n");
                        paramter.Command.Parameters.Add(assignedRootParameter.RootVariable);

                    }
                }
                //for inner statement
                else if (LexicalWords[start_index + 3] == terminal_sysntax.OpenRegion)
                {
                    var parent_parameter = GetTreeCommand(paramter, parameter_index);
                    paramter.Command.Parameters.Add(parent_parameter);
                    if (parent_parameter != null)
                        parent_parameter.Command.ParentCommand = paramter.Command;
                }
            }
            start_index += 4;
            CommandPriority.Add(com);

            return paramter;
        }
    }
}
