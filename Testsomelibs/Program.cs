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

                string text = Console.ReadLine();

                BigInteger p = 3004913 /*1699*/, q = 20979403, n = p * q;
                BigInteger[] arr = Rabin.EncryptionBigText(text, n);

                Console.WriteLine("BigInt Массив :\n");
                foreach(BigInteger b in arr)
                {
                    Console.WriteLine("\t "+b);
                }

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }


    }
}
