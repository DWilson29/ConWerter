using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConWerter.Models
{
    internal static class Converter
    {
        static readonly Dictionary<char, string> MorseCode = new()
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

        static public void PlaySound(string text)
        {
            foreach (char c in text.ToLower())
            {
                if (MorseCode.TryGetValue(c, out string code))
                {
                    foreach (char symbol in code.ToCharArray())
                    {
                        if (symbol == '.')
                        {
                            Console.Beep(800, 200); // Dot: short beep
                        }
                        else if (symbol == '-')
                        {
                            Console.Beep(800, 600); // Dash: long beep
                        }
                    }
                    System.Threading.Thread.Sleep(200); // Pause between letters
                }
                else
                {
                    Debug.WriteLine("Failed to convert character: " + c);
                }
            }
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
