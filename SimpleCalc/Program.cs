using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your first numer: "); 
            double.TryParse(Console.ReadLine(), out double num1);       // Converts string input to double, (in this case, named num1.)
            Console.WriteLine("Enter your second numer: ");
            double.TryParse(Console.ReadLine(), out double num2);       // Does the same but for num2.
            Console.WriteLine("Enter your operation (add/subtract/divide/multiply/exit): ");
            string operation = Console.ReadLine();
            double numResult;                                           // A double is useful for storing decimals.

            // if and else statements

            if (operation == "add")
            {
                numResult = num1 + num2;
                Console.WriteLine(numResult);
            } 
            else if (operation == "subtract")
            {
                numResult = num1 - num2;
                Console.WriteLine(numResult);
            }
            else if (operation == "divide")
            {
                numResult = num1 / num2;
                Console.WriteLine(numResult);
            }
            else if (operation == "multiply")
            {
                numResult = num1 * num2;
                Console.WriteLine(numResult);
            }
            else if (operation == "exit")
            {
                Environment.Exit(1);
            }

            Console.ReadLine();                                         // Prevents the program from closing.
        }
    }
}

/*
 * A simple text-based calculator. This is my first full program and repository on GitHub, and although basic it has taught me
 * the basics of C# and how to create a working program.
 * 
 * If you happen to find this repository somehow while you are trying to learn C#, keep in mind that I am a beginner as of writing
 * this and there's most probably ways to simplify this code. Feel free to use anything I have written here to teach yourself the
 * basics of C#, and I wish you luck on your goal to learn this wonderful language. */