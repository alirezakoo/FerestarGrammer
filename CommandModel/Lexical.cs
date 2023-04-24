using System;
using System.Collections.Generic;
using System.Text;

namespace Ferestar.Lib.LexicalCompiler.CommandModel
{
    public enum LexicalType
    {
        VARIABLE=0,FUNCTION_NAME=2,OPEN_REGION=4,CLOSE_REGION=6,PARAMETER_SEPERATO=8,VALUE=10,ASSYGN=12, END_LINE = 14
    }
    public class Lexical
    {
        string word = "";
        public Lexical(string word, LexicalType lexicalType)
        {
            this.word = word.ToLower();
            LexicalType = lexicalType;
        }

        public string Word { get { return word; } set { word=value.ToLower(); } }
        public LexicalType LexicalType { get; set; }
    }
}
