using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OTMC.Dialog_Boxes
{
    /// <summary>
    /// Interaction logic for NoServer.xaml
    /// </summary>
    public partial class NoServer : Window
    {
        public NoServer(double x,double y)
        {            
            InitializeComponent();
            Dialog.Margin = new Thickness(x,y,0,0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
