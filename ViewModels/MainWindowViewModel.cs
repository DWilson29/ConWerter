using ConWerter.Models;
using System.Text;

namespace ConWerter.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private string _cwOutput = string.Empty;

        public string CwOutput
        {
            get => _cwOutput;
            set => this.SetProperty(ref _cwOutput, value);
        }
    }
}
