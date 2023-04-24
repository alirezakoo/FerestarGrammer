using Ferestar.Lib.LexicalCompiler.CommandModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ferestar.Lib.LexicalCompiler.LexicalTools
{
    internal class Utility
    {
        public static Type GetType(object value)
        {
            string val = Convert.ToString(value);
            if (int.TryParse(val, out int inte))
                return typeof(int);
            else if (float.TryParse(val, out float flo))
                return typeof(float);
            else if (double.TryParse(val, out double dou))
                return typeof(double);
            else if (Guid.TryParse(val, out Guid gui))
                return typeof(Guid);
            else if (System.Net.IPAddress.TryParse(val, out System.Net.IPAddress ip))
                return typeof(System.Net.IPAddress);
            else if (Uri.IsWellFormedUriString(val, UriKind.RelativeOrAbsolute))
                return typeof(Uri);
            else
                return typeof(string);
        }
    }


    /// <summary>
    ///for compile all statements 
    /// statements should have the below format
    /// variable_name = command_name ( parameter_name_1 = parameter_value_1 , parameter_name_2 = parameter_value_2 , ... ) ;
    /// Commands can be nested and spaces and blank spaces will not be meaningful
    /// for example
    /// variable_name = command_name ( parameter_name_1 = command_name_2(parameter_name_x='hello')) ;
    /// all of the string and values should be between ' '
    /// </summary>
    public class LexicalAnalyser
    {
        internal static CompileResult AllCompiledLexicalCommand;
        /// <summary>
        /// for seprate and prepare to compile
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="err"></param>
        /// <returns>retarn a list of the head of the tree's statements</returns>
        internal static CompileResult Compile(string statements)
        {



            // seprate statement and inserted in array
            string[] commandLines = statements.Split(TerminalSymbol.DefaultTerminalSymbols.EndStatement, StringSplitOptions.RemoveEmptyEntries);
            int line = 0;
            foreach (string command in commandLines)
            {


                if (command.Trim() == "")
                    continue;

                StructureResult result = null;
                try
                {
                    result = new StructureResult(command, line);
                    result.Compile();
                    result.CompileLineStatus = new OperationResult()
                    {
                        CompileStatus = true
                    };
                }
                catch (Exception err)
                {
                    result.CompileLineStatus = new OperationResult()
                    {
                        ExceptionResult = err,
                        CompileStatus = false
                    };

                }

                if (!result.CompileLineStatus.CompileStatus)
                {
                    LexicalAnalyser.AllCompiledLexicalCommand.CompileStatus.CompileStatus = false;
                    LexicalAnalyser.AllCompiledLexicalCommand.CompileStatus.ExceptionResult = new Exception(result.CompileLineStatus.ExceptionResult.Message, LexicalAnalyser.AllCompiledLexicalCommand.CompileStatus.ExceptionResult);
                    System.Console.WriteLine(result.CommandText);
                    System.Console.WriteLine("Error:" + result.CompileLineStatus.ExceptionResult.Message);
                    var next = result.CompileLineStatus.ExceptionResult.InnerException;
                    while (next != null)
                    {
                        System.Console.WriteLine(next.ToString());
                        next = next.InnerException;
                    }
                    System.Console.WriteLine("-------------------------------------------------------");
                    break;
                }
                line++;
            }

            return AllCompiledLexicalCommand;
        }



    }
}
