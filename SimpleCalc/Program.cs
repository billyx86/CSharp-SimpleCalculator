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
            Console.WriteLine("Enter your first number: "); 
            if (double.TryParse(Console.ReadLine(), out double num1) == false)      // Converts the string input into the double "num1". The if statment checks if it is false. 
            {
                Console.WriteLine("Invalid number. Press enter to exit.");         
                Console.ReadLine();
                Environment.Exit(1);                                                // Tells the user that the number they input is invalid, and ends the program with code 1.
            }

            Console.WriteLine("Enter your second number: ");
            if (double.TryParse(Console.ReadLine(), out double num2) == false)      // Same process, but with the second number.
            {
                Console.WriteLine("Invalid number. Press enter to exit.");
                Console.ReadLine();
                Environment.Exit(1);
            }

            Console.WriteLine("Enter your operator type's symbol (or type exit): ");
            string op = Console.ReadLine();

            // if and else statements

            if (op == "+")
            {
                double numResult = num1 + num2;                                     // A double is useful for storing decimals.
                Console.WriteLine(numResult);
            } 
            else if (op == "-")
            {
                double numResult = num1 - num2;                         
                Console.WriteLine(numResult);
            }
            else if (op == "/")
            {
                double numResult = num1 / num2;                         
                Console.WriteLine(numResult);
            }
            else if (op == "*")
            {
                double numResult = num1 * num2;                         
                Console.WriteLine(numResult);
            }
            else if (op == "exit")
            {
                Environment.Exit(0);
            }
            else                                                                    // Executes if the operator input is neither "+", "-", "/", "*", or "exit." 
            {
                Console.WriteLine("Invalid input. The possible inputs were: Add (+), Subtract (-), Multiply (*), Divide (/), or Exit (exit).");
            }

            Console.WriteLine("Press enter to exit.");
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
