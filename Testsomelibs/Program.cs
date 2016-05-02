using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabinLib;
using System.Numerics;

namespace Testsomelibs
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine(Rabin.Miller_Rabin_Test(int.Parse(Console.ReadLine()),5));
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            
        }
       

    }
}
