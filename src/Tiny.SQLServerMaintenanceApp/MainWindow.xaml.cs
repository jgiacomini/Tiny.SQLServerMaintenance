using System.Windows;

namespace Tiny.SQLServerMaintenanceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChooseServerViewModel();
        }
    }
}
