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

        public static double Parse(string str)
        {
            str = str.Replace(',', '.');

            Match regexMatch = Regex.Match(str, string.Format($@"\(({RegexMatch})\)"));

            if (regexMatch.Groups.Count > 1)
            {
                string middle = regexMatch.Groups[0].Value.Substring(1, regexMatch.Groups[0].Value.Trim().Length - 2);
                string left = str.Substring(0, regexMatch.Index);
                string right = str.Substring(regexMatch.Index + regexMatch.Length);

                return Parse(left + Parse(middle) + right);
            }

            Match matchMultDiv = Regex.Match(str, string.Format($@"({RegexNumber})\s?({RegexMultDiv})\s?({RegexNumber})\s?"));
            Match matchPlusMinus = Regex.Match(str, string.Format($@"({RegexNumber})\s?({RegexPlusMinus})\s?({RegexNumber})\s?"));

            var match = (matchMultDiv.Groups.Count > 1) ? matchMultDiv : (matchPlusMinus.Groups.Count > 1) ? matchPlusMinus : null;

            if (match != null)
            {
                string left = str.Substring(0, match.Index);
                string right = str.Substring(match.Index + match.Length);
                string val = ParseAction(match).ToString(CultureInfo.InvariantCulture);

                return Parse(string.Format($"{left}{val}{right}"));
            }
            
            try
            {
                return double.Parse(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Syntax error: {str}");
            }

            return 0;
        }

        private static double ParseAction(Match match)
        {
            double leftNum = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double rightNum = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            switch (match.Groups[2].Value)
            {
                case "+":
                    return leftNum + rightNum;
                case "-":
                    return leftNum - rightNum;
                case "*":
                    return leftNum * rightNum;
                case "/":
                    return leftNum / rightNum;
                case "^":
                    return Math.Pow(leftNum, rightNum);
                case "%":
                    return leftNum % rightNum;
                default:
                    return 0;
            }
        }

        public static void Start()
        {
            Console.WriteLine("Enter your expression or enter \"exit\" to close the program");

            bool exit = false;
            while (!exit)
            {
                Console.Write("Expression: ");
                string expression = Console.ReadLine();

                if (expression != "exit")
                {
                    Console.WriteLine($"Answer: {Parse(expression)}");
                    Console.WriteLine("____________________\n");
                }
                else
                {
                    exit = true;
                }
            }
        }
    }
}