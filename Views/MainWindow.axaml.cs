using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ConWerter.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConWerter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnSelectAudioFileClick(object? sender, RoutedEventArgs e)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.StorageProvider == null) return;

            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select an audio file",
                AllowMultiple = false,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("Audio Files (*.mp3, *.wav)")
                    {
                        Patterns = new[] { "*.mp3", "*.wav" }
                    }
                }
            });

            IStorageFile? selected = files.FirstOrDefault();
            string? path = selected?.TryGetLocalPath();
            if (path == null) return;

            if (DataContext is MainWindowViewModel vm)
            {
                await vm.LoadAndConvertAudioFileAsync(path);
            }
        }
    }
}
