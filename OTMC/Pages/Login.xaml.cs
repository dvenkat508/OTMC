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
using OTMC.Classes;
using OTMC.Dialog_Boxes;
using System.Xml.Serialization;
using System.IO;
using System.Net.Sockets;
using OTMC.Pages;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        //private bool IsConnected;
        
        public Login()
        {
            InitializeComponent();
            if (!File.Exists(@"c:\Chat"))
            {
                string headfolder =@"c:\Chat";
                String images = headfolder + @"\Images";
                String audio = headfolder + @"\Audio";
                String videos = headfolder + @"\Videos";
                String files = headfolder + @"\Files";
                String subfiles = headfolder + @"\Files\ChatFiles";
                Directory.CreateDirectory(headfolder);
                if(!File.Exists(images))
                {
                    Directory.CreateDirectory(images);
                }
                if (!File.Exists(videos))
                {
                    Directory.CreateDirectory(videos);
                }
                if (!File.Exists(audio))
                {
                    Directory.CreateDirectory(audio);
                }
                if (!File.Exists(files))
                {
                    Directory.CreateDirectory(files);
                }
                if (!File.Exists(subfiles))
                {
                    Directory.CreateDirectory(subfiles);
                }
            }
        }
        public bool i = true;
        private void Create_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ServerConnection.ConnectToServer())
            {
                Create ne = new Create();
                this.NavigationService.Navigate(ne);
            }
            else
            {
                double x = Application.Current.MainWindow.Left;
                double y = Application.Current.MainWindow.Top;
                NoServer diag = new NoServer(x,y);
                diag.ShowDialog();
            }                
        }

        private void Create_MouseEnter(object sender, MouseEventArgs e)
        {
            Create.TextDecorations = TextDecorations.Underline;
        }

        private void Create_MouseLeave(object sender, MouseEventArgs e)
        {
            Create.TextDecorations = null;
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            if (ServerConnection.ConnectToServer())
            {
                if (ErrorCheck())
                {
                    i = true;
                    Loginob a = new Loginob(email_id.Text, pass.Password.ToString());
                    StreamWriter stream = null;
                    XmlSerializer xmlSerializer;
                    xmlSerializer = new XmlSerializer(typeof(Loginob));
                    MemoryStream memoryStream = new MemoryStream();
                    stream = new StreamWriter(memoryStream);
                    XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                    xs.Add("", "");
                    xmlSerializer.Serialize(stream, a, xs);
                    byte[] buffer = memoryStream.GetBuffer();
                    string data = "Login$" + Encoding.ASCII.GetString(buffer) + "$0";
                    byte[] buff = Encoding.ASCII.GetBytes(data);
                    ServerConnection.ClientSocket.Send(buff, 0, buff.Length, SocketFlags.None);
                    while (i)
                    {
                        ReceiveResponse();
                    }
                }                
            }
            else
            {
                double x = Application.Current.MainWindow.Left;
                double y = Application.Current.MainWindow.Top;
                NoServer diag = new NoServer(x, y);
                diag.ShowDialog();
            }
        }
        public string test = "";
        public static Chat page = new Chat();
        private void ReceiveResponse()
        {
            var buffer = new byte[204800];
            int received = ServerConnection.ClientSocket.Receive(buffer);
            if (received != 0)
            {
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                string text = Encoding.ASCII.GetString(data);
                i = false;
                test = test + text;
                if (test.Substring(test.Length - 2, 2) == "$0")
                {
                    if (text == "Email or Password is Incorrect$0")
                    {
                        double x = Application.Current.MainWindow.Left;
                        double y = Application.Current.MainWindow.Top;
                        Incorrect_Details diag = new Incorrect_Details(x, y);
                        diag.ShowDialog();
                    }
                    else
                    {
                        string first = test.Substring(0, test.IndexOf("$"));                        
                        if (first == "OK")
                        {
                            string dat = test.Substring(test.IndexOf("$") + 1, test.IndexOf("$1") - test.IndexOf("$") - 1);
                            string dat1 = test.Substring(test.IndexOf("$1") + 2, test.IndexOf("$0") - test.IndexOf("$1") - 2);
                            XmlSerializer xmlSerializer1;
                            MemoryStream memStream1 = null;
                            byte[] d1 = Encoding.ASCII.GetBytes(dat);
                            xmlSerializer1 = new XmlSerializer(typeof(list));
                            memStream1 = new MemoryStream(d1);
                            object objectFromXml1 = xmlSerializer1.Deserialize(memStream1);
                            list a1 = (list)objectFromXml1;

                            byte[] d = Encoding.ASCII.GetBytes(dat1);
                            XmlSerializer xmlSerializer;
                            MemoryStream memStream = null;
                            xmlSerializer = new XmlSerializer(typeof(List<list>));
                            memStream = new MemoryStream(d);
                            object objectFromXml = xmlSerializer.Deserialize(memStream);
                            List<list> a = (List<list>)objectFromXml;
                            
                            //ActiveUsers pag = new ActiveUsers();
                            page = new Chat(a,a1);
                            this.NavigationService.Navigate(page);
                        }
                    }
                    test = "";
                }
                else
                {
                    ReceiveResponse();
                }
            }

        }

        private bool ErrorCheck()
        {
            bool noerror = true;
            pass_error.Text = "";
            email_id_error.Text = "";
            if (pass.Password == ""| email_id.Text == "")
            {
                noerror = false;
                if (pass.Password == "")
                {
                    pass_error.Text = "Enter Password";
                }
                else
                {
                    pass_error.Text = "";
                }
                if (email_id.Text == "")
                {
                    email_id_error.Text = "Enter Email ID";
                }
                else
                {
                    email_id_error.Text = "";
                }
            }
            return noerror;
        }

        private void email_id_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            email_id_error.Text = "";
        }

        private void pass_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            pass_error.Text = "";
        }
    }
}
