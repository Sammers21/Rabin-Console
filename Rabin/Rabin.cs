using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
namespace RabinLib
{
    delegate bool SignatyreVert(BigInteger key);

    public static class Rabin
    {
        static Random rnd = new Random();

        #region Rabin Classic System
        //Криптосистема Рабина
        //page 156
        //
        /// <summary>
        /// Шифрование по обычной схеме Рабина
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="OpenKey">Открытый ключ</param>
        /// <returns>Число представлющее текст</returns>
        public static BigInteger Encryption(string text, BigInteger OpenKey)
        {
            BigInteger result = ConvToBigIntWithBit(text);

            Console.WriteLine("результат числового представления: " + result);

            result = MX(result);

            Console.WriteLine("результат применения хеш функции: " + result);


            if (result > OpenKey)
                throw new Exception("Слишком большое сообщение");

            BigInteger C = BigInteger.ModPow(result, 2, OpenKey);

            Console.WriteLine("Шифротекст: " + C);


            return C;

        }

        /// <summary>
        /// Деширование
        /// </summary>
        /// <param name="TextC">Зашифрованный текст</param>
        /// <param name="q">Простое число q. Один из закрытых ключей</param>
        /// <param name="p">Простое число p. Один из закрытых ключей</param>
        /// <returns>Расшифрованный текст</returns>
        public static string Decryption(BigInteger TextC, BigInteger q, BigInteger p)
        {

            if (!(Miller_Rabin_Test(q)&&Miller_Rabin_Test(p)))
                throw new Exception("Один из ключей не простой");


            BigInteger r, _r;
            BigInteger s, _s;

            BigInteger n = p * q;
            //step 1
            Get_Sqare(out r, out _r, p, TextC);
            //step 2
            Get_Sqare(out s, out _s, q, TextC);

            //step 3
            BigInteger D, c, d;
            do
            {
                ShareAlgoryeOfEyclid(out D, out c, out d, p, q);
            } while (D != 1);

            BigInteger x = BigInteger.ModPow((r * d * q + s * c * p), 1, n);
            while (x < 0)
                x += n;
            BigInteger minusX = n - x;
            BigInteger y = BigInteger.ModPow((r * d * q - s * c * p), 1, n);
            while (y < 0)
                y += n;
            BigInteger minusY = n - y;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("При взятии квадратного корня было найено 4 решения");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" x1= " + x);
            Console.WriteLine(" x2= " + minusX);
            Console.WriteLine(" x3= " + y);
            Console.WriteLine(" x4= " + minusY);

            List<string> roots = new List<string>() {x.ToString(),minusX.ToString(),
         y.ToString(),minusY.ToString()};
            List<string> Analized = Analyze(roots);
            Console.ForegroundColor = ConsoleColor.Green;
            BigInteger message = BigInteger.Parse(Analized[0]);
            message /= 100;
            Console.WriteLine("полученно " + message);
            return (ConvToStringWithBit(message));

        }

        #endregion

        #region Rabin Signatyre
        //Электронно цифровая подпись Рабина с Извлечением сообщения
        //page158

        /// <summary>
        /// Вычисление подписи
        /// </summary>
        /// <param name="text">Подписываемый текс</param>
        /// <param name="p">Однин из закрытых ключей</param>
        /// <param name="q">Одни из закрытых ключей</param>
        /// <param name="II">Получаемый сдвиг</param>
        /// <returns>Получаемая подпись</returns>
        public static BigInteger RabinSignatyre(string text, BigInteger p, BigInteger q, out BigInteger II)
        {

            if (!(Miller_Rabin_Test(q)&&Miller_Rabin_Test(p)))
                throw new Exception("Один из ключей не простой");

            BigInteger OpenKey = p * q;
           

            BigInteger result = ConvToBigIntWithBit(text);



            Console.WriteLine("результат числового представления : " + result);

            result = MX(result);

            Console.WriteLine("результат применения хеш функции: " + result);


            if (result > OpenKey)
                throw new Exception("Слишком большое сообщение");

            BigInteger s;
            funcR(result, p, q, out s, out II);
            return s;

        }

        /// <summary>
        /// Проверка подписи с извлечением сообщения
        /// </summary>
        /// <param name="OpenKey">Открытый ключ</param>
        /// <param name="S">Подпись</param>
        /// <param name="I">Сдвиг</param>
        /// <returns>Результат извлечения сообщения </returns>
        public static string DecryptionWithVertif(BigInteger OpenKey, BigInteger S, BigInteger I)
        {
            BigInteger M;
            if (SignatyreVertif(S, I, OpenKey, out M))
                Console.WriteLine("Подпись принимается");
            else
                Console.WriteLine("Подпись не принимается");
            return (ConvToStringWithBit(M));
        }

        #endregion

        #region Rabin Modif Signatyre
        //Модифицированная цифровая подпись Рабина с извелением сообщения

        /// <summary>
        /// Вычисление закрытого ключа
        /// </summary>
        /// <param name="p">p эквивалентно 3 mod 8</param>
        /// <param name="q">q эквивалентно 7 mod 8</param>
        /// <returns>Закртый ключ</returns>
        public static BigInteger CalcOfSecretKey(BigInteger p, BigInteger q)
        {
            if (p % 8 == 3 && q % 8 == 7)
                return ((p * q - p - q + 5) / 8);
            else
                throw new Exception("Ошибка в Вычислении ключей");

        }

        /// <summary>
        /// Вычисление подписи
        /// </summary>
        /// <param name="st">Подписываемый текст</param>
        /// <param name="OpenKey">Открытый ключ</param>
        /// <param name="SecretKey">Закрытый ключ</param>
        /// <returns>Подпись S</returns>
        public static BigInteger ModifCalcSignatyre(string st, BigInteger OpenKey, BigInteger SecretKey)
        {
            //step 1 page 160
            BigInteger T = ConvToBigIntWithBit(st);

            if (T > ((OpenKey - 6) / 16))
                throw new Exception("не удалось вычислить подпись так как m=>(n-6)/16");
            //step 2
            BigInteger w = 16 * T + 6;
            //step 3
            BigInteger Jack = Jacobi(w, OpenKey);

            //step 4
            BigInteger S;

            if (Jack == 1)
                S = BigInteger.ModPow(w, SecretKey, OpenKey);

            else if (Jack == -1)
                S = BigInteger.ModPow((w / 2), SecretKey, OpenKey);

            else
                throw new Exception("Требуется факторизация числа n");

            return S;
        }

        /// <summary>
        /// Расшифровка и извелечение сообщения 
        /// </summary>
        /// <param name="S">Подпись</param>
        /// <param name="OpenKey">Открытый ключ</param>
        /// <param name="res">Подтвержедние подписи</param>
        /// <returns>Извлеченное сообщение</returns>
        public static string DecryptSign(BigInteger S, BigInteger OpenKey, out bool res)
        {
            BigInteger u = BigInteger.ModPow(S, 2, OpenKey), U = BigInteger.ModPow(u, 1, 8);
            Console.WriteLine("u= " + u + " U=" + U);
            BigInteger w;
            if (U == 6)
                w = u;

            else if (U == 3)
                w = 2 * u;

            else if (U == 7)
                w = OpenKey - u;

            else if (U == 2)
                w = 2 * (OpenKey - u);

            else
                throw new Exception("Ошибка в проверке подписи");
            Console.WriteLine("проверка w=" + w);
            SignatyreVert Vetif = delegate (BigInteger si)
              {
                  if ((si - 6) % 16 == 0)
                      return true;
                  else return false;
              };


            res = Vetif(w);

            if (res == false)
                Console.WriteLine("Подпись не принята");

            BigInteger m = (w - 6) / 16;

            return ConvToStringWithBit(m);
        }

        #endregion

        #region ConvertMethods

        /// <summary>
        /// Преобразование в число текста
        /// </summary>
        /// <param name="Text">текст</param>
        /// <returns>число</returns>
        static BigInteger ConvToBigIntWithBit(string Text)
        {
            Console.WriteLine("Текст на входе " + Text + "\n");

            byte[] data = Encoding.UTF8.GetBytes(Text);
            Console.Write("массив байt \n");


            for (int i = 0; i < data.Length; i++)
            {
                string rees = "";
                if (i > 0)
                    rees = i % 5 == 0 ? "\n" : "";
                Console.Write(data[i] + "\t" + rees);
            }
            Console.WriteLine("\n");

            BigInteger res = 0;

            for (int i = 0; i < data.Length; i++)
                res += data[i] * (BigInteger)Math.Pow(2, 8 * i);

            Console.WriteLine("\n Число на выходе " + res);

            return res;
        }

        /// <summary>
        /// Метод преобразующий число в текст
        /// </summary>
        /// <param name="textnumb">Число</param>
        /// <returns>Текст</returns>
        public static string ConvToStringWithBit(BigInteger textnumb)
        {


            byte[] data = new byte[0];


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
                        Array.Resize(ref data, data.Length + 1);
                        data[data.Length - 1] = numb;

                        break;
                    }
                    if (i == 7)
                    {
                        Array.Resize(ref data, data.Length + 1);
                        data[data.Length - 1] = numb;
                    }
                }
            }


            string result = Encoding.UTF8.GetString(data);

            return result;
        }



        #endregion

        #region CoreMethods

        /// <summary>
        /// Проверка подписи
        /// </summary>
        /// <param name="S">Подпись</param>
        /// <param name="I">Сдвиг</param>
        /// <param name="opKey">Открытый ключ</param>
        /// <param name="?"></param>
        /// <returns>True в случае удлевостворения подписи хеш-функции и false в обратном</returns>
        static bool SignatyreVertif(BigInteger S, BigInteger I, BigInteger opKey, out BigInteger M)
        {
            BigInteger w = BigInteger.ModPow(S, 2, opKey);

            BigInteger m = w - I;

            List<string> lst = new List<string>() { m.ToString() };

            List<string> analyzed = Analyze(lst);

            if (analyzed.Count == 1)
            {
                m /= 100;
                M = m;
                return true;
            }
            else
            {
                M = 0;
                return false;
            }
        }



        /// <summary>
        /// Вычисление сдвига и подписи
        /// </summary>
        /// <param name="m">Число для которого мы ищем ближешее имеющее квадратные вычеты</param>
        /// <param name="p">Один из закртых ключей</param>
        /// <param name="q">Один из закртых ключей</param>
        /// <param name="S">Вычесленная подпись</param>
        /// <param name="i">Сдвиг подписи</param>
        static void funcR(BigInteger m, BigInteger p, BigInteger q, out BigInteger S, out BigInteger i)
        {
            BigInteger k = 0, n = p * q;

            while (Jacobi(m + k, n) == -1 || Jacobi(m + k, p) == -1 || Jacobi(m + k, q) == -1)
            {
                k++;
            }

            m = m + k;

            BigInteger r, _r;
            BigInteger s, _s;
            //step 1


            Get_Sqare(out r, out _r, p, m);
            //step 2
            Get_Sqare(out s, out _s, q, m);


            BigInteger D, c, d;
            do
            {
                ShareAlgoryeOfEyclid(out D, out c, out d, p, q);
            } while (D != 1);

            BigInteger x = BigInteger.ModPow((r * d * q + s * c * p), 1, n);
            while (x < 0)
                x += n;
            S = x;
            i = k;
        }

        /// <summary>
        /// Анализирует удлевотворяет ли сообщение хеш функции
        /// </summary>
        /// <param name="sqares">квадратные вычеты</param>
        /// <returns>Список удвелотворяющей хеш функции строк</returns>
        static List<string> Analyze(List<string> sqares)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < sqares.Count; i++)
            {
                char c1 = sqares[i][sqares[i].Length - 1], c2 = sqares[i][sqares[i].Length - 3],
                    c3 = sqares[i][sqares[i].Length - 2], c4 = sqares[i][sqares[i].Length - 4];
                if (c1 == c2 && c3 == c4)
                {
                    result.Add(sqares[i]);
                }
            }

            Console.WriteLine("При расшифровке был найдено {0} элементов, удлевотворящих заданной хеш функции", result.Count);

            if (result.Count() == 1)
            {
                Console.WriteLine("Было успешно определено сходное сообщение");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Исходное сообщение :" + result[0]);
            }
            else
                Console.WriteLine("Не удалось однозначно установить исходное сообщение");

            return result;

        }
        static BigInteger Rand(BigInteger p)
        {

            BigInteger result;
            string str = "";
            bool flag = true;

            int[] pio = (p + "").ToCharArray().Select(k => int.Parse(k + "")).ToArray();

            for (int i = 0; i < pio.Length; i++)
            {

                int x;
                if (flag)
                {
                    x = rnd.Next(1, pio[i] + 1);
                    if (x < pio[i])
                        flag = false;

                }
                else
                {
                    x = rnd.Next(1, 10);
                }

                str += x;
            }

            result = BigInteger.Parse(str);
            return result;
        }
        /// <summary>
        /// Выбор рандомного числа B
        /// </summary>
        /// <param name="p">модуль числа по которому мы вычисляем</param>
        /// <returns>Выбранное число </returns>
        static BigInteger ChooseRandB(BigInteger p)
        {
            BigInteger result;
            do
            {
                result = Rand(p);
                if (Jacobi(result, p) == -1)
                    return result;

            } while (true);
        }

        /// <summary>
        /// Шаг второй. Нахождение S и T
        /// </summary>
        /// <param name="P">Число </param>
        /// <param name="T">Конечный остаток</param>
        /// <param name="S">Степень двойки</param>
        static void Step2PowSumnT(BigInteger P, out BigInteger T, out BigInteger S)
        {
            BigInteger Pminus = P - 1;

            int Some2Pow = 0;

            do
            {
                if (Pminus % 2 == 0)
                {
                    Some2Pow++;
                    Pminus /= 2;
                }
                else
                {
                    T = Pminus;
                    S = Some2Pow;
                    return;
                }

            } while (true);

        }


        /// <summary>
        /// Сивол якоби
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns>Значение символа Якоби</returns>
        static int Jacobi(BigInteger a, BigInteger n)
        {
            BigInteger d, v, u;
            ShareAlgoryeOfEyclid(out d, out v, out u, a, n);
            if (d != 1)
                return 0;
            if (a < 0)     //1
                return (((n - 1) / 2) == 0 ? 1 : -1) * Jacobi(-a, n);

            if (a % 2 == 0)//2
            {
                return ((n * n - 1) / 8 % 2 == 0 ? 1 : -1) * Jacobi((a / 2), n);
            }

            if (a == 1)    //3
                return 1;

            if (a < n)     //4
                return ((((a - 1) / 2) * ((n - 1) / 2)) % 2 == 0 ? 1 : -1) * Jacobi(n, a);

            return Jacobi(a % n, n);//5

            throw new Exception("Encryption exception");
        }


        /// <summary>
        /// Нахождение обратного элемента
        /// </summary>
        /// <param name="a">Число обратный элемент которого мы ищем</param>
        /// <param name="n">Число по модулю которого идет вычисление обратного элемента</param>
        /// <returns></returns>
        static BigInteger FindA_1ModN(BigInteger a, BigInteger n)
        {
            BigInteger d, x, y;
            ShareAlgoryeOfEyclid(out d, out y, out x, n, a);

            if (d > 1)
                throw new Exception("Обратного элемента не существует");
            while (x < 0)
                x += n;
            return x;

        }

        /// <summary>
        /// Расширенный Алгоритм  Евклида.
        /// </summary>
        /// <param name="d"> d=НОД(a,b)</param>
        /// <param name="u">d=ua+vb</param>
        /// <param name="v">d=ua+vb</param>
        /// <param name="a">A>=B</param>
        /// <param name="b">A>=B</param>
        static void ShareAlgoryeOfEyclid(out BigInteger d, out BigInteger u, out BigInteger v, BigInteger a, BigInteger b)
        {
            if (b == 0)
            {
                d = a;
                u = 1;
                v = 0;
                return;
            }

            BigInteger u2 = 1, u1 = 0, v2 = 0, v1 = 1, q, r;

            while (b > 0)
            {
                q = a / b;
                r = a - q * b;
                u = u2 - q * u1;
                v = v2 - q * v1;
                a = b;
                b = r;
                u2 = u1;
                u1 = u;
                v2 = v1;
                v1 = v;
            }
            d = a;
            u = u2;
            v = v2;
        }

        /// <summary>
        /// проверка эквивалентно ли d=-1(mod p)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        static bool Eq_1(BigInteger d, BigInteger p)
        {
            if (d + 1 == p)
                return true;
            else
                return false;
        }

        /// <summary>
        /// вычисление одноо из квадратных корней
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        static BigInteger Get_Sqare1(BigInteger p, BigInteger a)
        {
            BigInteger Rr, _R, P = p, A = a;
            Get_Sqare(out Rr, out _R, P, A);

            return Rr;
        }

        /// <summary>
        /// Получение квадратных корней(двух)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="_r"></param>
        /// <param name="p"></param>
        /// <param name="a"></param>
        static void Get_Sqare(out BigInteger r, out BigInteger _r, BigInteger p, BigInteger a)
        {
            // a = BigInteger.ModPow(a, 1, p);

            //step 1
            if (Jacobi(a, p) == -1)
            {

                throw new Exception("корня а по модулю п не существует");
            }
            //step 2
            BigInteger b = ChooseRandB(p), t, s;

            //step 3
            Step2PowSumnT(p, out t, out s);

            //step 4
            BigInteger a_1 = FindA_1ModN(a, p);

            //step 5
            BigInteger c = BigInteger.ModPow(b, t, p);
            r = BigInteger.ModPow(a, (t + 1) / 2, p);

            //step 6
            for (BigInteger i = 1; i <= s - 1; i++)
            {
                BigInteger d = BigInteger.ModPow((r * r * a_1), BigInteger.Pow(2, (int)(s - i - 1)), p);
                if (Eq_1(d, p))
                    r = BigInteger.ModPow(r * c, 1, p);
                c = BigInteger.ModPow(c, 2, p);
            }
            _r = -r;


        }

        /// <summary>
        /// Хеш функция удваивания двух последних цифр
        /// </summary>
        /// <param name="CryptTEXT"></param>
        /// <returns></returns>
        static BigInteger MX(BigInteger CryptTEXT)
        {
            return CryptTEXT % 100 + CryptTEXT * 100;
        }
        #endregion

        #region Miller_Rabin_Tests
        public static bool Miller_Rabin_Test(BigInteger Number, BigInteger Rounds)
        {
            if (Number <= 2)
                throw new Exception("На тест подано чилос меньше 3");

            if (BigInteger.ModPow(Number, 1, 2) == 0)
                return false;

 

            BigInteger S, T;

            Step2PowSumnT(Number, out T, out S);

            //цикл А
            for (int i = 0; i < Rounds; i++)
            {
                bool flagtoCycleA = false;
                BigInteger a = Rand(Number - 1);
                BigInteger x = BigInteger.ModPow(a, T, Number);
                if (x == 1 || x == Number - 1)
                    continue;
                //цикл Б
                for (int k = 0; k < (S - 1); k++)
                {
                    x = BigInteger.ModPow(x, 2, Number);
                    if (x == 1)
                        return false;
                    if (x == Number - 1)
                    {
                        flagtoCycleA = true;
                        break;
                    }


                }
                if (flagtoCycleA)
                    continue;
                return false;

            }

            return true;
        }
        public static bool Miller_Rabin_Test(BigInteger Number)
        {
            if (Number <= 2)
                throw new Exception("На тест подано чилос меньше 3");

            if (BigInteger.ModPow(Number, 1, 2) == 0)
                return false;

            int X = 1;
            BigInteger pow = 2;
            do
            {
                if (X < pow * 2 && pow >= X)
                    break;
                pow *= 2;
                X++;
            } while (true);

            BigInteger S, T;

            Step2PowSumnT(Number, out T, out S);

            //цикл А
            for (int i = 0; i < X; i++)
            {
                bool flagtoCycleA = false;
                BigInteger a = Rand(Number - 1);
                BigInteger x = BigInteger.ModPow(a, T, Number);
                if (x == 1 || x == Number - 1)
                    continue;
                //цикл Б
                for (int k = 0; k < (S - 1); k++)
                {
                    x = BigInteger.ModPow(x, 2, Number);
                    if (x == 1)
                        return false;
                    if (x == Number - 1)
                    {
                        flagtoCycleA = true;
                        break;
                    }


                }
                if (flagtoCycleA)
                    continue;
                return false;

            }

            return true;
        }

        #endregion

        #region Inactive methods
        /// <summary>
        /// Преобразует строку в число
        /// </summary>
        /// <param name="text">Входная строка </param>
        /// <returns>Число</returns>
        static BigInteger ConvToString(string text)
        {


            char[] newSystemMod = text.ToCharArray();
            Array.Reverse(newSystemMod);

            BigInteger result = 0;

            for (int i = 0; i < newSystemMod.Length; i++)
                result += (BigInteger)Math.Pow(27, i) * ((int)newSystemMod[i] - (int)'A' + 1);
            return result;
        }

        /// <summary>
        /// Конвертировние в строку из числа по основанию 27
        /// </summary>
        /// <param name="numDEX">Число для перевода</param>
        /// <returns>тТекст</returns>
        static string ConvertTo27System(BigInteger numDEX)
        {


            string result = "";
            BigInteger initNumb = numDEX;
            while (initNumb > 26)
            {
                result += (char)('A' - 1 + initNumb % 27);
                initNumb /= 27;
            }
            result += (char)('A' - 1 + initNumb);
            char[] ch = result.ToCharArray();
            Array.Reverse(ch);
            result = "";
            foreach (char c in ch)
                result += c + "";
            return result;
        }
        #endregion
    }

}
