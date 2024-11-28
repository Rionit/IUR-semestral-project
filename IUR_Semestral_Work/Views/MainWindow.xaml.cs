using IUR_Semestral_Work.ViewModels;
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

namespace IUR_Semestral_Work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainViewModel)DataContext).IsMouseDown = true;
            e.Handled = true;
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((MainViewModel)DataContext).IsMouseDown = false;
            e.Handled = true;
        }


    }
}