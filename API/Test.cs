using System;
using System.Text.RegularExpressions;

namespace API
{
    public class Test
    {
        public static void Main()
        {
            //DocumentProcessHelper.Instance.InitiateProcesses();
            string input = Console.ReadLine();
            while (input != "q")
            {
                Regex regex = new Regex(@"(0(\d{8}|\d{10})$)");
                var output = regex.IsMatch(input);
                Console.WriteLine(output);
                input = Console.ReadLine();
            }
            Console.ReadKey();
        }
    }
}
