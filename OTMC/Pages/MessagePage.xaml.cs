using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Xml.Serialization;
using OTMC.Classes;
using OTMC.Pages;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Interaction logic for MessagePage.xaml
    /// </summary>
    public partial class MessagePage : Page
    {
        public static List<MessageItem> Items = new List<MessageItem>();
        public MessagePage(string filename)
        {
            InitializeComponent();
            Items = GetItems(filename);
            As.ItemsSource = Items;
            cat.ScrollToEnd();
        }
        
        public  MessagePage()
        {
            InitializeComponent();
        }

        public static void Update(MessagePage at)
        {            
            ActiveUsers a = new ActiveUsers();
            string username = a.getcurrentuser();
            string filepath = @"c:\Chat\Files\ChatFiles" + @"\" + username + ".dat";
            
            Items = at.GetItems(filepath);
            at.As.ItemsSource = Items;
        }

        public List<MessageItem> GetItems(string s)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            List<MessageItem> it = new List<MessageItem>();
            List<file> c = new List<file>();
            if (File.Exists(s))
            {
                try
                {
                    // Create a FileStream will gain read access to the 
                    // data file.
                    FileStream readerFileStream = new FileStream(s, FileMode.Open, FileAccess.Read);
                    // Reconstruct information of our friends from file.
                    c = (List<file>)formatter.Deserialize(readerFileStream);
                    // Close the readerFileStream when we are done
                    readerFileStream.Close();
                    foreach (file d in c)
                    {
                        it.Add(new MessageItem(d.Message, d.Sendbyme));
                    }
                }
                catch (Exception)
                {
                    
                } // end try-catch
            }
            return it;
        }
    }
}
