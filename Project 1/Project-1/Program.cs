// Alex Miller
// 9/15/2022
// Log base 2 finder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Fib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Log Base 2");
            while (true)
            {
                Console.Write("\nEnter N: ");
                long n = long.Parse(Console.ReadLine());
                //long fib = Fib(n);
                long fib = lg(lg(n));
                Console.WriteLine("Floor of lg(lg({0})) = {1}.", n, fib);
            }
        }
        static long Fib(long n)
        {
            if (n <= 2)
                return 1;
            else
                return Fib(n - 1) + Fib(n - 2);
        }

        static long lg(long n)
        {
            if (n > 1)
                return 1 + lg(n / 2);
            else
                return 0;
        }
    }
}