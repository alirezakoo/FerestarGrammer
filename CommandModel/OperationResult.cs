
using Ferestar.Lib.LexicalCompiler.LexicalTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ferestar.Lib.LexicalCompiler.CommandModel
{
    /// <summary>
    /// The overall compiled result is placed in this class
    /// </summary>
    public class CompileResult
    {
        public CompileResult()
        {
            Results = new List<StructureResult>();
            CompileStatus = new OperationResult() {CompileStatus=true,ExceptionResult=new Exception("compile finished") };
        }
        public OperationResult CompileStatus { get; }
        public List<StructureResult> Results { get; }
        internal void AddResult(StructureResult result)
        {
            Results.Add(result);
        }
        
    }
    public class OperationResult
    {
        private bool success = false;
        private Exception exception_result;
        public bool CompileStatus
        {
            get
            {
                return success;
            }

            set
            {
                success = value;
            }
        }

        public Exception ExceptionResult { get => exception_result; set => exception_result = value; }
    }
}
