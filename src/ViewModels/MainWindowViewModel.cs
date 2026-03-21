using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConWerter.Models;
using System;
using System.Threading.Tasks;

namespace ConWerter.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _phrase;

        [ObservableProperty]
        private string? _cw;

        [ObservableProperty]
        private string? _cwOutput;

        [ObservableProperty]
        private string? _phraseOutput;

        [ObservableProperty]
        private double _volume = 50;

        [ObservableProperty]
        private double _speed = 50;

        [RelayCommand]
        private async Task ConvertPhrase()
        {
            if (Phrase == null) return;
            await Task.Run(() => PlaySound(Phrase));
        }

        private float GetVolume()
        {
            return (float)(Volume / 100);
        }

        private float GetSpeed()
        {
            return (float)(100 - Speed) / 100;
        }

        public void PlaySound(string text)
        {
            if (Converter.isPlaying)
            {
                return;
            }

            Converter.isPlaying = true;
            CwOutput = "";

            foreach (char c in text.ToLower())
            {
                string code = Converter.CharToMorseCode(c);
                foreach (char symbol in code.ToCharArray())
                {
                    CwOutput += symbol;

                    if (symbol == '.')
                    {
                        Player.Beep(false, GetVolume(), GetSpeed());
                    }
                    else if (symbol == '-')
                    {
                        Player.Beep(true, GetVolume(), GetSpeed());
                    }
                    System.Threading.Thread.Sleep(200); // Pause between letters
                }
                CwOutput += ' ';
            }
            Converter.isPlaying = false;
        }

        [RelayCommand]
        private async Task ConvertCw()
        {
            if (Cw == null) return;
            string value = Converter.InvertMorse(Cw);
            PhraseOutput = value;
        }
    }
}
