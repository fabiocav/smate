using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester.Echo
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                Console.WriteLine(input);
                input = Console.ReadLine();
            }
        }
    }
}
