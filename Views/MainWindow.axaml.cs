using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ConWerter.Models;
using System.Diagnostics;

namespace ConWerter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Phrase(object? sender, RoutedEventArgs e)
        {
            if (Phrase.Text == null) return;
            Converter.PlaySound(Phrase.Text, CwOutput);
        }

        private void Button_Cw(object? sender, RoutedEventArgs e)
        {
            if (CW.Text == null) return;
            string value = Converter.InvertMorse(CW.Text);
            Debug.WriteLine(value);
            TextOutput.Text = value;
        }
    }
}
