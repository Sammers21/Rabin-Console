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

                BigInteger p = 20979403 /*1699*/, q = 20985857, n = p * q;
                BigInteger[] arr = Rabin.EncryptionBigText(text, n);

                Console.WriteLine("\nBigInt Массив :\n");
                foreach(BigInteger b in arr)
                {
                    Console.WriteLine("\t "+b);
                }
                string decrText = Rabin.DecryptionBigText(arr, p, q);
                Console.WriteLine("\n"+decrText);


            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }


    }
}
