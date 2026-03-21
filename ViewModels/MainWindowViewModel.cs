using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConWerter.Models;
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

        [RelayCommand]
        private async Task ConvertPhrase()
        {
            if (Phrase == null) return;
            await Task.Run(() => PlaySound(Phrase));
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
                        Player.Beep(false, 1);
                    }
                    else if (symbol == '-')
                    {
                        Player.Beep(true, 1);
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
