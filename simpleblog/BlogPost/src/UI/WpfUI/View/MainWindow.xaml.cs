using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI.ViewModel;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Costruttore: riceve il ViewModel tramite dependency injection.
        /// Il DataContext viene impostato qui per collegare la View al ViewModel (pattern MVVM).
        /// </summary>
        /// <param name="viewModel">ViewModel principale iniettato dal container DI</param>
        public MainWindow(MainViewModel viewModel)
        {
            // Inizializza i componenti XAML (generato automaticamente)
            InitializeComponent();
            
            // Imposta il ViewModel come DataContext per abilitare il binding
            // Tutti i binding nel XAML si riferiscono alle proprietà di questo ViewModel
            DataContext = viewModel;
        }
    }
}