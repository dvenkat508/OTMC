using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OTMC.Classes;
using System.IO;
using System.Net.Sockets;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OTMC.Pages;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>


    public partial class Chat : Page
    {
        
        public string filepath;
        public BitmapImage src = new BitmapImage();
        public Chat(List<list> a, list a1 )
        {
            InitializeComponent();            
            message.Content = new MessagePage();
            Users.Content = new ActiveUsers(a);
            LoadImage(a1.Img);
            User_Profile.Source = src;
            User_name.Text = a1.Name;
            User_Email.Text = a1.Email;            
        }

        public Chat()
        {
            InitializeComponent();
        }

        public void LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                src = new BitmapImage(new Uri(@"C:\Users\dvenk\Project\Code\Client\OTMC\OTMC\Images\Default_Profile_Pic.png"));
            else
            {
                BitmapImage sr = new BitmapImage();
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                username = username.Substring(username.IndexOf(@"\") + 1, username.Length - username.IndexOf(@"\") - 1);
                File.WriteAllBytes(@"C:\Users\" + username + @"\Pictures\ProfilePic.png", imageData);
                using (FileStream stream = File.OpenRead(@"C:\Users\" + username + @"\Pictures\ProfilePic.png"))
                {
                    sr.BeginInit();
                    sr.StreamSource = stream;
                    sr.CacheOption = BitmapCacheOption.OnLoad;
                    sr.EndInit();
                    sr.Freeze();
                }
                src = sr;
                File.Delete(@"C:\Users\" + username + @"\Pictures\ProfilePic.png");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

       
        public string address = @"c:\Chat\Files\ChatFiles";
        public List<file> b = new List<file>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            ActiveUsers active = new ActiveUsers();
            string a = Inputblock.Text;
            string to = active.getcurrentuser();          
            string from = User_Email.Text;
            textmessage mesage = new textmessage(a, from, to);
            sendmess(mesage);
            BinaryFormatter formatter = new BinaryFormatter();
            file messobj = new file();
            messobj.Message = a;
            messobj.Sendbyme = true;
            string filename = address + @"\" + to + ".dat";
            if (File.Exists(filename))
            {
                FileStream readerFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                try
                {
                    b = (List<file>)formatter.Deserialize(readerFileStream);
                }
                catch
                {
                    b = new List<file>();
                }
                readerFileStream.Dispose();
                readerFileStream.Close();
            }
            b.Add(messobj);
            FileStream writerFileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            formatter.Serialize(writerFileStream, b);
            writerFileStream.Dispose();
            writerFileStream.Close();
            message.Content = new MessagePage(filename);
            Inputblock.Foreground = new SolidColorBrush(Colors.DarkGray);
            Inputblock.Text = "Enter Message";
        }

        public void sendmess(textmessage Mess)
        {            
            StreamWriter stream = null;
            if(stream==null)
            {
                XmlSerializer xmlSerializer;
                xmlSerializer = new XmlSerializer(typeof(textmessage));
                MemoryStream memoryStream = new MemoryStream();
                stream = new StreamWriter(memoryStream);
                XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                xs.Add("", "");
                xmlSerializer.Serialize(stream, Mess, xs);
                byte[] buffer = memoryStream.GetBuffer();
                string data = "TextMessage$" + Encoding.ASCII.GetString(buffer) + "$0";
                buffer a = new buffer();
                a.buff = Encoding.ASCII.GetBytes(data);
                ServerConnection.ClientSocket.Send(a.buff, 0, a.buff.Length, SocketFlags.None);
                stream.Dispose();
                memoryStream.Dispose();
                Array.Clear(buffer, 0, buffer.Length);
            }            
        }

        private void Inputblock_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Inputblock.Text == "Enter Message")
            {
                Inputblock.Foreground = new SolidColorBrush(Colors.Black);
                Inputblock.Text = "";
            }            
        }

        private void Inputblock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Inputblock.Text == "")
            {
                Inputblock.Foreground = new SolidColorBrush(Colors.DarkGray);
                Inputblock.Text = "Enter Message";
            }
        }
        private void Attach_Document_Click(object sender, RoutedEventArgs e)
        {
            sendgrid.Height = new GridLength(0);
            message.Content = new MediaSend(3);
        }

        private void Attach_Audio_Click(object sender, RoutedEventArgs e)
        {
            sendgrid.Height = new GridLength(0);
            message.Content = new MediaSend(2);
        }

        private void Attach_Video_Click(object sender, RoutedEventArgs e)
        {
            sendgrid.Height = new GridLength(0);
            message.Content = new MediaSend(1);
        }

        private void Attach_Image_Click(object sender, RoutedEventArgs e)
        {
            sendgrid.Height = new GridLength(0);
            message.Content = new MediaSend(0);            
        }
    }
}
