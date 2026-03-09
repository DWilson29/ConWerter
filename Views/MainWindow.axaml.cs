using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using ConWerter.Models;

namespace ConWerter.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Phrase(object? sender, RoutedEventArgs e)
        {
            if (Phrase.Text == null) return;
            Converter.PlaySound(Phrase.Text);
        }

        private void Button_Cw(object? sender, RoutedEventArgs e)
        {
            if (CW.Text == null) return;
            string value = Converter.InvertMorse(CW.Text);
            Debug.WriteLine(value);
        }
    }
}