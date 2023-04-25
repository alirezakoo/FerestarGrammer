using Ferestar.Lib.LexicalCompiler.CommandModel;
using Ferestar.Lib.LexicalCompiler.LexicalTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ferestar.Lib.LexicalCompiler
{
    /// <summary> 
    /// format of commands:
    /// variable_name = command_name ( parameter_name_1 = parameter_value_1 , parameter_name_2 = parameter_value_2 , ... ) ;
    /// 
    /// variable_name = command_name ( parameter_name_1 = command_name_2(parameter_name_x='hello')) ;

    /// </summary>
    public static class CompileEngine
    {
        public static CompileResult FrestarCompile(this string statements)
        {
            return LexicalAnalyser.Compile(statements);
        }

        //----------------------------------for test statement
        static void Main(string[] args)
        {
            LexicalAnalyser.AllCompiledLexicalCommand = new CompileResult();
            StringBuilder statement = new StringBuilder();
            statement.Append("var1=upload(p1=getData(query='select * from hr.table1',con='server=.,uid=test'),p2=openUrl(url='https://alirezakoo.github.io/'), p3=getPic(user=getUserId(username='alirezakoo')));");
            statement.Append(@"aab:getpic{usersss:getuserid{username:'55555',pic:getpic{user:getuserid{username:'55555' }} },pic:getpic{user:getuserid{username:'55555',pic:getpic{user:getuserid{username:'55555' }},pic:getpic{user:getuserid{username:getuserid{} }} }}};");
            statement.Append(@"aab:abbba{bb:aab};");

            var result = statement.ToString().FrestarCompile();
            System.Console.ReadKey();
        }
    }
}
