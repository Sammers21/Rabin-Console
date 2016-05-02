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
                BigInteger k = 20979403;
                k *= 3004913;
                Console.WriteLine(Rabin.Miller_Rabin_Test(k));
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            
        }
       

    }
}
