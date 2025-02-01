using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Utilities
{
    public class Calcs
    {
        public bool IsPrime(int number)
        {
            if (number <= 1) return false; // 1以下は素数ではない
            if (number == 2) return true;  // 2は素数
            if (number % 2 == 0) return false; // 2以外の偶数は素数ではない

            int boundary = (int)Math.Floor(Math.Sqrt(number)); // 平方根を計算
            for (int i = 3; i <= boundary; i += 2) // 奇数のみをチェック
            {
                if (number % i == 0) return false; // 割り切れたら素数ではない
            }
            return true; // 割り切れなければ素数
        }
        public List<int> Get約数(int number)
        {
            List<int> divisors = new List<int>();
            for (int i = 1; i <= number; i++)
            {
                if (number % i == 0)
                {
                    divisors.Add(i);
                }
            }
            return divisors;
        }
        public List<int> Get公約数(int number1, int number2)
        {
            List<int> divisors1 = Get約数(number1);
            List<int> divisors2 = Get約数(number2);

            List<int> commonDivisors = new List<int>();

            foreach (int divisor in divisors1)
            {
                if (divisors2.Contains(divisor))
                {
                    commonDivisors.Add(divisor);
                }
            }
            return commonDivisors;
        }
        public List<int> Get公約数(int number1, int number2, int number3)
        {
            List<int> divisors1 = Get約数(number1);
            List<int> divisors2 = Get約数(number2);
            List<int> divisors3 = Get約数(number3);

            List<int> commonDivisors = new List<int>();

            foreach (int divisor in divisors1)
            {
                if (divisors2.Contains(divisor) && divisors3.Contains(divisor))
                {
                    commonDivisors.Add(divisor);
                }
            }
            return commonDivisors;
        }
        public int Get最大公約数(int number1, int number2)
        {
            while (number2 != 0)
            {
                int temp = number2;
                number2 = number1 % number2;
                number1 = temp;
            }
            return number1;
        }
        public int Get最大公約数(int number1, int number2, int number3)
        {
            int gcdOfTwo = Get最大公約数(number1, number2);
            return Get最大公約数(gcdOfTwo, number3);
        }
        public List<int> Get倍数(int number, int lowerBound, int upperBound)
        {
            List<int> multiples = new List<int>();

            for (int i = lowerBound; i <= upperBound; i++)
            {
                if (i % number == 0)
                {
                    multiples.Add(i);
                }
            }

            return multiples;
        }
        public List<int> Get公倍数(int number1, int number2, int max)
        {
            List<int> multiples = new List<int>();
            for (int i = 1; i <= max; i++)
            {
                if (i % number1 == 0 && i % number2 == 0)
                {
                    multiples.Add(i);
                }
            }

            return multiples;
        }

        public List<int> Get公倍数(int number1, int number2, int number3, int max)
        {
            List<int> multiples = new List<int>();
            for (int i = 1; i <= max; i++)
            {
                if (i % number1 == 0 && i % number2 == 0 && i % number3 == 0)
                {
                    multiples.Add(i);
                }
            }

            return multiples;
        }

        public int Get最小公倍数(int number1, int number2)
        {
            return (number1 * number2) / Get最大公約数(number1, number2);
        }

        public int Get最小公倍数(int number1, int number2, int number3)
        {
            int lcmOfTwo = Get最小公倍数(number1, number2);
            return Get最小公倍数(lcmOfTwo, number3);
        }



    }
}
