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
            var result= statement.Append("var1=upload(p1=getData(query='select * from hr.table1',con='server=.,uid=test'),p2=openUrl(url='https://alirezakoo.github.io/'), p3=getPic(user=getUserId(username='alirezakoo')));");
            statement.Append(@"aab:getpic{usersss:getuserid{username:'55555',pic:getpic{user:getuserid{username:'55555' }} },pic:getpic{user:getuserid{username:'55555',pic:getpic{user:getuserid{username:'55555' }},pic:getpic{user:getuserid{username:getuserid{} }} }}};");
            statement.Append(@"aab2:abbba{bb:aab};");
            //statement.Append(@"good");

            DateTime dt1as = DateTime.Now;
            System.Console.WriteLine(dt1as + "-" + dt1as.Millisecond);
            var result2 = statement.ToString().FrestarCompile();
            System.Console.WriteLine(result2.CompileStatus.ExceptionResult.Message);
            DateTime dt2as = DateTime.Now;
            System.Console.WriteLine(dt2as + "-" + dt2as.Millisecond);
            System.Console.WriteLine("Sync:" + (dt2as - dt1as).Seconds + "-" + (dt2as - dt1as).Milliseconds);

            System.Console.ReadKey();
        }
    }
}
