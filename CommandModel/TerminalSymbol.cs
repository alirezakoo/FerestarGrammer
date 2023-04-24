using System;
using System.Collections.Generic;
using System.Text;

namespace Ferestar.Lib.LexicalCompiler.CommandModel
{
    internal class TerminalSymbol
    {
        public Dictionary<string, string> SimilarSymbol;
        public string OpenRegion{ get; set; }
        public string CloseRegion { get; set; }
        public string Assign { get; set; }
        public string Seprator { get; set; }
        public string EndStatement { get; set; }
        public string StringRegion { get; set; }
        internal static TerminalSymbol DefaultTerminalSymbols
        {
            get
            {
                TerminalSymbol symbol= new TerminalSymbol()
                {

                    Assign = "=",
                    CloseRegion = ")",
                    OpenRegion = "(",
                    EndStatement = ";",
                    Seprator = ",",
                    StringRegion = "'",

                };
                symbol.SimilarSymbol = new Dictionary<string, string>();
                symbol.SimilarSymbol.Add(":", symbol.Assign);
                symbol.SimilarSymbol.Add("==", symbol.Assign);
                symbol.SimilarSymbol.Add("===", symbol.Assign);
                symbol.SimilarSymbol.Add("}", symbol.CloseRegion);
                symbol.SimilarSymbol.Add("]", symbol.CloseRegion);
                symbol.SimilarSymbol.Add(">", symbol.CloseRegion);
                symbol.SimilarSymbol.Add("{", symbol.OpenRegion);
                symbol.SimilarSymbol.Add("[", symbol.OpenRegion);
                symbol.SimilarSymbol.Add("<", symbol.OpenRegion);
                symbol.SimilarSymbol.Add("-", symbol.Seprator);
                return symbol;
            }
        }



    }
}
