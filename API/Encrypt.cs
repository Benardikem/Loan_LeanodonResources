using System;

namespace API
{
    class Encrypt
    {
        static void Main(string[] args)
        {
            Console.Write("D: Decrypt, E: Encrypt? Default {E}>>");
            string op = Console.ReadLine();
            Console.Write("Input: ");
            string input = Console.ReadLine();
            if (op.ToUpper() == "D")
            {
                Console.WriteLine(Utilities.Crypto.Decrypt(input));
            }
            else
                Console.WriteLine(Utilities.Crypto.Encrypt(input));
            Console.ReadKey();
        }
    }
}
