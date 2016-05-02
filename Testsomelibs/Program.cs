using System;
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
               
                Console.WriteLine(BigInteger.ModPow(BigInteger.Parse(Console.ReadLine()),1,8));
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            
        }
       

    }
}
