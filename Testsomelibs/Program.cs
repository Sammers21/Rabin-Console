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


        }
        public static BigInteger ConvWithBitToString(string Text)
        {
            Console.WriteLine("Текст на входе " + Text + "\n");

            byte[] data = Encoding.UTF8.GetBytes(Text);
            Console.Write("массив байt");


            for (int i = 0; i < data.Length; i++)
            {
                string rees = "";
                if (i > 0)
                    rees = i % 5 == 0 ? "\n" : "";
                Console.Write(data[i] + "\t" + rees);
            }


            BigInteger res = 0;

            for (int i = 0; i < data.Length; i++)
                res += data[i] * (BigInteger)Math.Pow(2, 8 * i);

            Console.WriteLine("\n Число на выходе " + res);

            return res;
        }
        public static string ConvIntoStringWithBit(BigInteger textnumb)
        {

            
            List<byte> data = new List<byte>();

            for (int j = 0; textnumb != 0; j++)
            {
                byte numb = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (textnumb != 0)
                    {
                        numb += (byte)((textnumb % 2) * (byte)Math.Pow(2, i));
                        textnumb /= 2;
                    }
                    else
                    {
                        data.Add(numb);
                        Console.Write(data + " " + (i));
                        break;
                    }
                    if (i==7)
                        data.Add(numb);
                }
            }
            byte[] input = data.ToArray();

            string result= Encoding.UTF8.GetString(input);
           
            return result;
        }

    }
}
