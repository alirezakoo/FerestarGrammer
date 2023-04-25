using System;
using System.Collections.Generic;

namespace Ferestar.Lib.LexicalCompiler.CommandModel
{
    public class Command
    {
        
        private object returnValue;
        public Type ReturnType { get; set; }
        public Command ParentCommand { get; set; }
        public DataContent Function{ get;set;}

        public Command(DataContent Function, List<CommandParameter> parameters, CommandParameter assignToParameter)
        {
            
            this.Function = Function;
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
                    throw new InvalidCastException("you can't cast (" + value.GetType().FullName + ") to (" + ReturnType.FullName + ") in parameter name " + Function.Content);
            }
        }
        public List<CommandParameter> Parameters
        {
           get;
            set;
        }
        
        public CommandParameter AssignToParameter { get; set; }

    }
    public class CommandParameter
    {
        private DataContent value;
        public Command Command { get; set; }
        public DataContent Name { get; set; }

        public CommandParameter(DataContent ParameterValue, DataContent Name, Command command)
        {

            this.value = ParameterValue;
            this.Name = Name;
            this.Command = command;
            if (this.Value != null)
                this.DataType = LexicalTools.Utility.GetType(this.Value.Content);
        }

        public DataContent Value
        {
            get { return value; }

            set
            {

                if (value != null)
                {
                    string val = value.Content.ToString();
                    if (val.ToString().Length >= 2)
                        if (val.Substring(0, 1) == TerminalSymbol.DefaultTerminalSymbols.StringRegion && val.Substring(val.Length - 1) == TerminalSymbol.DefaultTerminalSymbols.StringRegion)
                        {
                            string removedval;

                            removedval = val.Remove(val.Length - 1);
                            removedval = removedval.Remove(0, 1);
                            value.Content = removedval;
                        }

                    //if (parameterType == value.GetType())
                    this.value = value;
                    //else
                    //    throw new InvalidCastException("Can not cast (" + value.GetType().FullName + ") to (" + parameterType.FullName + ") in parameter name " + Name);
                }
            }
        }

        public Type DataType { get; }
    }

}
