using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConWerter.Models
{
    internal static class Converter
    {
        static readonly Dictionary<char, List<char>> MorseCode = new()
        {
            ['a'] = [ '.', '-' ],
            ['b'] = [ '-', '.', '.', '.' ],
            ['c'] = [ '-', '.', '-', '.' ],
            ['d'] = [ '-', '.', '.' ],
            ['e'] = [ '.' ],
            ['f'] = [ '.', '.', '-', '.' ],
            ['g'] = [ '-', '-', '.' ],
            ['h'] = [ '.', '.', '.', '.' ],
            ['i'] = [ '.', '.' ],
            ['j'] = [ '.', '-', '-', '-' ],
            ['k'] = [ '-', '.', '-' ],
            ['l'] = [ '.', '-', '.', '.' ],
            ['m'] = [ '-', '-' ],
            ['n'] = [ '-', '.' ],
            ['o'] = [ '-', '-', '-' ],
            ['p'] = [ '.', '-', '-' ],
            ['q'] = [ '-', '-', '.', '-' ],
            ['r'] = [ '.', '-', '.' ],
            ['s'] = [ '.', '.', '.' ],
            ['t'] = [ '-' ],
            ['u'] = [ '.', '.', '-' ],
            ['v'] = [ '.', '.', '.', '-' ],
            ['w'] = [ '.', '-', '-' ],
            ['x'] = [ '-', '.', '.', '-' ],
            ['y'] = [ '-', '.', '-', '-' ],
            ['z'] = [ '-', '-', '.', '.' ],
            ['1'] = [ '.', '-', '-', '-', '-' ],
            ['2'] = [ '.', '.', '-', '-', '-' ],
            ['3'] = [ '.', '.', '.', '-', '-' ],
            ['4'] = [ '.', '.', '.', '.', '-' ],
            ['5'] = [ '.', '.', '.', '.', '.' ],
            ['6'] = [ '-', '.', '.', '.', '.' ],
            ['7'] = [ '-', '-', '.', '.', '.' ],
            ['8'] = [ '-', '-', '-', '.', '.' ],
            ['9'] = [ '-', '-', '-', '-', '.' ],
            ['0'] = [ '-', '-', '-', '-', '-' ],
        };

        static public void PlaySound(string text)
        {
            foreach (char c in text.ToLower())
            {
                if (MorseCode.TryGetValue(c, out List<char> code))
                {
                    foreach (char symbol in code)
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
                foreach ((char key, List<char> encoded) in MorseCode)
                {
                    string encodedStr = new string(encoded.ToArray());
                    if (encodedStr == c)
                    {
                        phrase += key.ToString();
                    }
                }
            }
            return phrase;
        }
    }
}
