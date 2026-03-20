using Avalonia.Threading;
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
        private string? _cwOutput;

        [RelayCommand]
        private async Task ConvertPhrase()
        {
            if (Phrase == null) return;
            var data = await Task.Run(() => Converter.PlaySound(Phrase));
            CwOutput = data;
        }

    }
}
