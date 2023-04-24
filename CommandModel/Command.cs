using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ferestar.Lib.LexicalCompiler.CommandModel
{
    public class Command
    {
        

        public string Id { get; set; }
        private object returnValue;
        public Type ReturnType { get; set; }
        public Command ParentCommand { get; set; }
        public Lexical LexicalName{ get;set;}

        public Command(Lexical LexicalName, List<CommandParameter> parameters, CommandParameter assignToParameter)
        {
            
            this.LexicalName = LexicalName;
            this.Parameters = parameters;
            this.AssignToParameter = assignToParameter;
        }


        public object ReturnValue
        {
            get
            {
                return returnValue;
            }

            set
            {
                if (ReturnType == value.GetType())
                    returnValue = value;
                else
                    throw new InvalidCastException("you can't cast (" + value.GetType().FullName + ") to (" + ReturnType.FullName + ") in parameter name " + LexicalName.Word);
            }
        }
        public List<CommandParameter> Parameters
        {
           get;
            set;
        }
        
        public CommandParameter AssignToParameter { get; set; }

        public string GetID()
        {
            return Id;
        }
    }
    public class CommandParameter
    {
        private Lexical value;
        public string Id { get; set; }
        public Command Command { get; set; }
        public Lexical ParameterName { get; set; }

        public CommandParameter(Lexical ParameterValue, Lexical ParameterName, Command command)
        {

            this.value = ParameterValue;
            this.ParameterName = ParameterName;
            this.Command = command;
            if (this.ParameterValue != null)
                this.ParameterType = LexicalTools.Utility.GetType(this.ParameterValue.Word);
        }

        public Lexical ParameterValue
        {
            get { return value; }

            set
            {

                if (value != null)
                {
                    string val = value.Word.ToString();
                    if (val.ToString().Length >= 2)
                        if (val.Substring(0, 1) == TerminalSymbol.DefaultTerminalSymbols.StringRegion && val.Substring(val.Length - 1) == TerminalSymbol.DefaultTerminalSymbols.StringRegion)
                        {
                            string removedval;

                            removedval = val.Remove(val.Length - 1);
                            removedval = removedval.Remove(0, 1);
                            value.Word = removedval;
                        }

                    //if (parameterType == value.GetType())
                    this.value = value;
                    //else
                    //    throw new InvalidCastException("Can not cast (" + value.GetType().FullName + ") to (" + parameterType.FullName + ") in parameter name " + Name);
                }
            }
        }

        public Type ParameterType { get; }
        public string GetID()
        {
            return Id;
        }
    }

}
