using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome6024();
            Welcome2498();
            Console.ReadKey();
        }

        static partial void Welcome2498();
        private static void Welcome6024()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}
