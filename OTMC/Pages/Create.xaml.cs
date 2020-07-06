using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Xml.Serialization;
using OTMC.Classes;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page
    {
        private Register obj;
        private BitmapImage src;
        private byte[] img;
        private byte[] buff;
        private byte[] buffer;

        public Create()
        {
            InitializeComponent();
        }

        public Create(String s)
        {
            InitializeComponent();
            if (s == "true")
            {
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                username = username.Substring(username.IndexOf(@"\") + 1, username.Length - username.IndexOf(@"\") - 1);
                src = new BitmapImage();
                using (FileStream stream = File.OpenRead(@"C:\Users\" + username + @"\Pictures\ProfilePic.png"))
                {
                    src.BeginInit();
                    src.StreamSource = stream;
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                }
                img = File.ReadAllBytes(@"C:\Users\" + username + @"\Pictures\ProfilePic.png");
                
                ProfilePic.Source = src;
                File.Delete(@"C:\Users\" + username + @"\Pictures\ProfilePic.png");
            }
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            Crop ce = new Crop();
            this.NavigationService.Navigate(ce);            
        }
        public bool i = true;
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorCheck())
            {
                i = true;
                string password = Password.Password.ToString();
                obj = new Register(name.Text, Email_Id.Text, password, img);
                StreamWriter stream = null;
                XmlSerializer xmlSerializer;
                xmlSerializer = new XmlSerializer(typeof(Register));
                MemoryStream memoryStream = new MemoryStream();
                stream = new StreamWriter(memoryStream);
                XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                xs.Add("", "");
                xmlSerializer.Serialize(stream, obj, xs);
                buffer = memoryStream.GetBuffer();
                string data = "Register$" + Encoding.ASCII.GetString(buffer) + "$0";
                buff = Encoding.ASCII.GetBytes(data);
                ServerConnection.ClientSocket.Send(buff, 0, buff.Length, SocketFlags.None);
                while (i)
                {
                    ReceiveResponse();
                }
            }
        }

        private void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ServerConnection.ClientSocket.Receive(buffer);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            i = false;
            if (text == "OK")
            {
                Login i = new Login();
                this.NavigationService.Navigate(i);
            }
            else if(text == "Used Email ID")
            {
                emailerror.Text = text;                
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Login er = new Login();
            this.NavigationService.Navigate(er);
        }

        private bool ErrorCheck()
        {
            bool noerror = true;
            nameerror.Text = "";
            passworderror.Text = "";
            cpassworderror.Text = "";
            emailerror.Text = "";
            if (Password.Password != C_Password.Password)
            {
                noerror = false;
                passworderror.Text = "Password Doesn't Match";
                cpassworderror.Text = passworderror.Text;
            }
            else
            {
                cpassworderror.Text = passworderror.Text = "";
            }
            if (name.Text == "" | Password.Password == "" | C_Password.Password == "" | Email_Id.Text == "")
            {
                noerror = false;
                if (name.Text == "")
                {
                    nameerror.Text = "Enter Name";
                }
                else
                {
                    nameerror.Text = "";
                }

                if (Password.Password == "")
                {
                    passworderror.Text = "Enter Password";
                }
                else
                {
                    passworderror.Text = "";
                }
                if (C_Password.Password == "")
                {
                    cpassworderror.Text = "Enter Confirm Password";
                }
                else
                {
                    cpassworderror.Text = "";
                }
                if (Email_Id.Text == "")
                {
                    emailerror.Text = "Enter Email ID";
                }
                else
                {
                    emailerror.Text = "";
                }
            }
            return noerror;
        }
    }
}
