using System;
using System.Collections.Generic;
using System.Globalization;
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
        // Parses a number the same way no matter which locale the program is
        // running under. The default double.TryParse(input) follows the
        // current culture, so under e.g. de_DE "5.5" is read as 55 (the dot is
        // a thousands separator there) and "5,5" is read as 5.5. Forcing the
        // invariant culture means the decimal point is always a dot, as
        // documented in the README (issue #8).
        static bool TryParseNumber(string input, out double number)
        {
            number = 0;
            return double.TryParse(input, NumberStyles.Float | NumberStyles.AllowLeadingSign,
                                   CultureInfo.InvariantCulture, out number);
        }

        // Prints the result of an operation — or a friendly overflow message if
        // the arithmetic produced a non-finite value. The input validation
        // above rejects NaN/Infinity inputs, but a result can still overflow
        // even from perfectly valid numbers: 1e308 * 10, 1e308 + 1e308 or
        // 1e308 - -1e308 all overflow double.MaxValue (~1.8e308), and 1e308 /
        // 1e-308 overflows too. Rather than printing "Infinity" (which the
        // program itself treats as an invalid number), report it the same
        // way division by zero is reported. (Issue #14)
        static void PrintOperationResult(double numResult)
        {
            if (double.IsNaN(numResult) || double.IsInfinity(numResult))
            {
                Console.WriteLine("Result overflowed - the answer is too large for this calculator. Try smaller numbers.");
            }
            else
            {
                Console.WriteLine(numResult.ToString(CultureInfo.InvariantCulture));
            }
        }

        static void Main(string[] args)
        {
            // Loop so the user can do as many calculations as they like in one session.
            // The loop keeps going until the user types "exit" as the operator, or until
            // there is no more input to read (for example, when the program is run from a
            // pipe and the input runs out).
            while (true)
            {
                Console.WriteLine("Enter your first number: ");
                string firstInput = Console.ReadLine();
                if (firstInput == null)                          // A null means the input ran out (end of file), e.g. when running from a pipe.
                {
                    Console.WriteLine("No more input - goodbye.");  // Friendly message instead of silently doing nothing.
                    break;
                }
                double num1;
                if (TryParseNumber(firstInput, out num1) == false || double.IsNaN(num1) || double.IsInfinity(num1))   // Converts the string into the double "num1". It also rejects "NaN", "Infinity" and "-Infinity", which TryParse would otherwise happily accept as numbers.
                {
                    Console.WriteLine("Invalid number. The possible inputs were real numbers, for example 5 or -3.5.");  // Tells the user the number is not a real number.
                    continue;                                               // Skip to the next calculation rather than ending the whole session.
                }

                Console.WriteLine("Enter your second number: ");
                string secondInput = Console.ReadLine();
                if (secondInput == null)                            // Same end-of-file check, for the second number.
                {
                    Console.WriteLine("No more input - goodbye.");
                    break;
                }
                double num2;
                if (TryParseNumber(secondInput, out num2) == false || double.IsNaN(num2) || double.IsInfinity(num2))   // Same process, but with the second number.
                {
                    Console.WriteLine("Invalid number. The possible inputs were real numbers, for example 5 or -3.5.");
                    continue;
                }

                Console.WriteLine("Enter your operator type's symbol (or type exit): ");
                string op = Console.ReadLine();
                if (op == null)                                      // The input ran out before an operator was given.
                {
                    Console.WriteLine("No more input - goodbye.");
                    break;
                }
                op = op.Trim();                                      // A stray space around the operator (e.g. "+ ") shouldn't count as invalid input.

                // if and else statements

                if (op == "+")
                {
                    double numResult = num1 + num2;                                     // A double is useful for storing decimals.
                    PrintOperationResult(numResult);
                }
                else if (op == "-")
                {
                    double numResult = num1 - num2;
                    PrintOperationResult(numResult);
                }
                else if (op == "/")
                {
                    if (num2 == 0)                                                     // Guards against division by zero, which would otherwise print "Infinity".
                    {
                        Console.WriteLine("Cannot divide by zero.");                   // Tells the user the operation is invalid, in the same style as the other error messages.
                    }
                    else
                    {
                        double numResult = num1 / num2;                                // A tiny divisor (1 / 1e-308) can still overflow, so route through the same guard.
                        PrintOperationResult(numResult);
                    }
                }
                else if (op == "*")
                {
                    double numResult = num1 * num2;
                    PrintOperationResult(numResult);
                }
                else if (string.Equals(op, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;                                                             // Leave the loop and end the session. "Exit", "EXIT" and " exit " all work too.
                }
                else                                                                    // Executes if the operator input is neither "+", "-", "/", "*", or "exit."
                {
                    Console.WriteLine("Invalid input. The possible inputs were: Add (+), Subtract (-), Multiply (*), Divide (/), or Exit (exit).");
                }
            }

            Console.WriteLine("Goodbye.");
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
