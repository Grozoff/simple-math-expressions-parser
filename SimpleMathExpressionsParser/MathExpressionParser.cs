using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SimpleMathExpressionsParser
{
    public static class MathExpressionParser
    {
        private const string RegexMatch = @"[1234567890\.\+\-\*\/^%]*";
        private const string RegexNumber = @"[-]?\d+\.?\d*";
        private const string RegexMultDiv = @"[\*\/^%]";
        private const string RegexPlusMinus = @"[\+\-]";

        public static void Start()
        {
            Console.WriteLine("Enter your expression or enter \"exit\" to close the program");

            while (true)
            {
                Console.Write("Expression: ");
                var expression = Console.ReadLine();

                if (expression == "exit")
                {
                    break;
                }
                else if (expression == "help")
                {
                    Console.WriteLine("This is a simple calculator that supports the following operations:");
                    Console.WriteLine("+ (addition)");
                    Console.WriteLine("- (subtraction)");
                    Console.WriteLine("* (multiplication)");
                    Console.WriteLine("/ (division)");
                    Console.WriteLine("^ (exponentiation)");
                    Console.WriteLine("% (Integer remainder)");
                    Console.WriteLine("Use parentheses to group expressions together.");
                    Console.WriteLine("Examples: 2+2, (2+2)*3, 2^(3-1)");
                    Console.WriteLine("Type \"exit\" to quit the program.");
                    Console.WriteLine("____________________\n");
                    continue;
                }

                Console.WriteLine($"Answer: {Parse(expression)}\n");
                Console.WriteLine("____________________\n");
            }
        }

        public static double Parse(string str)
        {
            str = str.Replace(',', '.');

            var regexMatch = Regex.Match(str, $@"\(({RegexMatch})\)");

            if (regexMatch.Groups.Count > 1)
            {
                var middle = regexMatch.Groups[1].Value;
                var left = str.Substring(0, regexMatch.Index);
                var right = str.Substring(regexMatch.Index + regexMatch.Length);

                return Parse(left + Parse(middle) + right);
            }

            var matchMultDiv = Regex.Match(str, $@"({RegexNumber})\s?({RegexMultDiv})\s?({RegexNumber})\s?");
            var matchPlusMinus = Regex.Match(str, $@"({RegexNumber})\s?({RegexPlusMinus})\s?({RegexNumber})\s?");

            var match = matchMultDiv.Groups.Count > 1 ? matchMultDiv : matchPlusMinus.Groups.Count > 1 ? matchPlusMinus : null;

            if (match != null)
            {
                var left = str.Substring(0, match.Index);
                var right = str.Substring(match.Index + match.Length);
                var val = ParseAction(match).ToString(CultureInfo.InvariantCulture);

                return Parse($"{left}{val}{right}");
            }

            if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            Console.WriteLine($"Syntax error: {str}");
            return 0;
        }

        private static double ParseAction(Match match)
        {
            var leftNum = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var rightNum = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            return match.Groups[2].Value switch
            {
                "+" => leftNum + rightNum,
                "-" => leftNum - rightNum,
                "*" => leftNum * rightNum,
                "/" => leftNum / rightNum,
                "^" => Math.Pow(leftNum, rightNum),
                "%" => leftNum % rightNum,
                _ => 0
            };
        }
    }
}