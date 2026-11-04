using System;
using System.Globalization;

namespace SimpleCalculator
{
    internal class Program
    {
        // A small ring buffer that remembers the last few results of a session
        // so the user can type "history" to see them again. Only the last
        // HistorySize results are kept; older ones are overwritten. A plain
        // array is used (no List<T>) to stay at the same beginner level as the
        // rest of the program. (Issue #19)
        const int HistorySize = 3;
        static readonly string[] history = new string[HistorySize];
        static int historyCount = 0;   // How many results are stored (at most HistorySize).
        static int historyNext = 0;    // Where the next result is written.

        // Remember a result string so ShowHistory can print it. Once the
        // buffer is full the oldest entry is overwritten. (Issue #19)
        static void RememberResult(string result)
        {
            history[historyNext] = result;
            historyNext = (historyNext + 1) % HistorySize;
            if (historyCount < HistorySize)
            {
                historyCount++;
            }
        }

        // Prints the remembered results, oldest first, for the "history"
        // command. (Issue #19)
        static void ShowHistory()
        {
            if (historyCount == 0)
            {
                Console.WriteLine("No results yet.");
                return;
            }
            Console.WriteLine("Recent results (oldest first):");
            int first = (historyNext - historyCount + HistorySize) % HistorySize;
            for (int i = 0; i < historyCount; i++)
            {
                Console.WriteLine(history[(first + i) % HistorySize]);
            }
        }

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

        // Prints the result of an operation — or a friendly message if the
        // arithmetic produced a non-finite value. The input validation above
        // rejects NaN/Infinity inputs, but a result can still be non-finite
        // even from perfectly valid numbers, in two distinct ways:
        //  - Infinity: the answer is too large for a double. 1e308 * 10,
        //    1e308 + 1e308, 1e308 - -1e308 and 1e308 / 1e-308 all overflow
        //    double.MaxValue (~1.8e308). Report "try smaller numbers".
        //  - NaN: the answer is undefined, not too large. Only Math.Pow can
        //    produce this from valid finite inputs — a fractional power of a
        //    negative base, e.g. (-8) ^ 0.5. Telling the user their numbers
        //    are too large here would be wrong, so it gets its own message.
        //    (Issue #14, refined by #22)
        static void PrintOperationResult(double numResult)
        {
            if (double.IsNaN(numResult))
            {
                Console.WriteLine("This power is not defined for the given numbers.");
            }
            else if (double.IsInfinity(numResult))
            {
                Console.WriteLine("Result overflowed - the answer is too large for this calculator. Try smaller numbers.");
            }
            else
            {
                Console.WriteLine(numResult.ToString(CultureInfo.InvariantCulture));
                RememberResult(numResult.ToString(CultureInfo.InvariantCulture));
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

                // The exit keyword is matched case-insensitively ("Exit",
                // "EXIT", " exit " all work), which a string switch on the
                // raw operator can't express, so it is checked first.
                if (string.Equals(op, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;                                                             // Leave the loop and end the session.
                }

                // "history" is a command, not an operator: it prints the
                // results of the last few calculations and continues the
                // session. Like exit it is matched case-insensitively. (Issue #19)
                if (string.Equals(op, "history", StringComparison.OrdinalIgnoreCase))
                {
                    ShowHistory();
                    continue;                                                          // Loop back and ask for the next calculation.
                }

                // switch statement

                switch (op)
                {
                    case "+":
                    {
                        double numResult = num1 + num2;                                     // A double is useful for storing decimals.
                        PrintOperationResult(numResult);
                        break;
                    }
                    case "-":
                    {
                        double numResult = num1 - num2;
                        PrintOperationResult(numResult);
                        break;
                    }
                    case "/":
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
                        break;
                    }
                    case "*":
                    {
                        double numResult = num1 * num2;
                        PrintOperationResult(numResult);
                        break;
                    }
                    case "^":
                    {
                        // C#'s '^' operator is bitwise XOR and is not defined for
                        // doubles, so exponentiation uses Math.Pow instead. The
                        // result can overflow (1e308 ^ 2 exceeds double.MaxValue),
                        // so it is routed through the same guard as the other
                        // operations. (Issue #17)
                        double numResult = Math.Pow(num1, num2);
                        PrintOperationResult(numResult);
                        break;
                    }
                    case "%":
                    {
                        // The double '%' operator returns NaN when the second
                        // operand is zero, so it is guarded the same way division
                        // is. (Issue #17)
                        if (num2 == 0)
                        {
                            Console.WriteLine("Cannot take the modulo of zero.");
                        }
                        else
                        {
                            double numResult = num1 % num2;
                            PrintOperationResult(numResult);
                        }
                        break;
                    }
                    default:                                                                // Executes if the operator is none of "+", "-", "/", "*", "^", or "%".
                    {
                        Console.WriteLine("Invalid input. The possible inputs were: Add (+), Subtract (-), Multiply (*), Divide (/), Power (^), Modulo (%), or Exit (exit).");
                        break;
                    }
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
