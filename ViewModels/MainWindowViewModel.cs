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
            Dispatcher.UIThread.Post(async () => CwOutput = await Converter.PlaySound(Phrase), DispatcherPriority.Background);
        }

    }
}
