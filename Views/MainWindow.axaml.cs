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

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if (Phrase.Text == null) return;
            Converter.PlaySound(Phrase.Text);
        }
    }
}