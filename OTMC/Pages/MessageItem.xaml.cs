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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for MessageItem.xaml
    /// </summary>


    public partial class MessageItem : UserControl
    {
        public MessageItem()
        {
            InitializeComponent();
        }
        public MessageItem(string mess, bool sendbyme)
        {
            InitializeComponent();
            message.Text = mess;
            if (sendbyme)
            {
                border.HorizontalAlignment = HorizontalAlignment.Left;
                anchor.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                border.HorizontalAlignment = HorizontalAlignment.Right;
                anchor.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
        
    }
}
