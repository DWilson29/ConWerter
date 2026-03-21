using System.Collections.Generic;

namespace ConWerter.Models
{
    internal static class Converter
    {
        private static readonly Dictionary<char, string> MorseCode = new()
        {
            ['a'] = ".-",
            ['b'] = "-...",
            ['c'] = "-.-.",
            ['d'] = "-..",
            ['e'] = ".",
            ['f'] = "..-.",
            ['g'] = "--.",
            ['h'] = "....",
            ['i'] = "..",
            ['j'] = ".---",
            ['k'] = "-.-",
            ['l'] = ".-..",
            ['m'] = "--",
            ['n'] = "-.",
            ['o'] = "---",
            ['p'] = ".--.",
            ['q'] = "--.-",
            ['r'] = ".-.",
            ['s'] = "...",
            ['t'] = "-",
            ['u'] = "..-",
            ['v'] = "...-",
            ['w'] = ".--",
            ['x'] = "-..-",
            ['y'] = "-.--",
            ['z'] = "--..",
            ['1'] = ".----",
            ['2'] = "..---",
            ['3'] = "...--",
            ['4'] = "....-",
            ['5'] = ".....",
            ['6'] = "-....",
            ['7'] = "--...",
            ['8'] = "---..",
            ['9'] = "----.",
            ['0'] = "-----",
            ['.'] = ".-.-.-",
            [','] = "--..--",
            ['?'] = "..--..",
            ['\''] = ".----.",
            ['/'] = "-..-.",
            ['('] = "-.--.",
            [')'] = "-.--.-",
            [':'] = "---...",
            ['='] = "-...-",
            ['+'] = ".-.-.",
            ['-'] = "-...-",
            ['"'] = ".-..-.",
            ['@'] = ".--.-.",
            ['!'] = "-.-.--",
            ['&'] = ".-...",
            [';'] = "-.-.-.",
            ['_'] = "..--.-",
            ['$'] = "...-..-",
        };

        static public bool isPlaying = false;

        static public string CharToMorseCode(char input)
        {
            MorseCode.TryGetValue(input, out string? code);
            return code ?? "";
        }

        static public string InvertMorse(string cw)
        {
            string phrase = "";
            foreach (string c in cw.ToLower().Split(" "))
            {
                foreach ((char key, string encoded) in MorseCode)
                {
                    if (encoded == c)
                    {
                        phrase += key.ToString();
                    }
                }
                phrase += ' ';
            }
            return phrase;
        }
    }
}
