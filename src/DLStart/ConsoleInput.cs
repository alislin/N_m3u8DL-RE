using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLStart
{
    internal class ConsoleHelper
    {
        /// <summary>
        /// 读取控制台输入
        /// </summary>
        /// <param name="infos">提示信息</param>
        /// <param name="filter">检查输入值是合法检测</param>
        /// <returns></returns>
        public static string? Read(string[] infos, int? emptyLine = 0, Func<string?, bool>? filter = null, ConsoleColor? color = null)
        {
            Print(infos, emptyLine, color);
            var line = Console.ReadLine();
            while (!(filter?.Invoke(line) ?? true))
            {
                Print(infos, emptyLine, color);
                line = Console.ReadLine();
            }

            return line;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="infos">提示信息</param>
        /// <param name="emptyLine">打印前插入的空行数（默认为：0）</param>
        public static void Print(string[] infos, int? emptyLine = 0, ConsoleColor? color = null)
        {
            var count = emptyLine ?? 0;
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("");
            }
            var orgColor = Console.ForegroundColor;
            Console.ForegroundColor = color ?? orgColor;
            foreach (var item in infos)
            {
                Console.WriteLine(item);
            }
            Console.ForegroundColor = orgColor;
        }

        public static int GetInt(string s, int defaultValue = 0)
        {
            if (int.TryParse(s, out var k))
            {
                return k;
            }
            return defaultValue;
        }
    }
}
