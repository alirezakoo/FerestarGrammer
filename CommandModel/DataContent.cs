using System;
using System.Collections.Generic;
using System.Text;

namespace Ferestar.Lib.LexicalCompiler.CommandModel
{
    public enum LexicalType
    {
        VARIABLE=0,FUNCTION_NAME=2,OPEN_REGION=4,CLOSE_REGION=6,PARAMETER_SEPERATO=8,VALUE=10,ASSYGN=12, END_LINE = 14
    }
    public class DataContent
    {
        private string content = "";
        public DataContent(string word, LexicalType type)
        {
            this.content = word.ToLower();
            Type = type;
        }

        public string Content { get { return content; } set { content=value.ToLower(); } }
        public LexicalType Type { get; set; }
    }
}
