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
            Console.WriteLine("Blah");
            string input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                Console.WriteLine(input);
                Console.Out.WriteLine(input);
                input = Console.ReadLine();
            }
        }
    }
}
