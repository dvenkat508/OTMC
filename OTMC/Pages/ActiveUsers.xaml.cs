using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OTMC.Classes;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for ActiveUsers.xaml
    /// </summary>
    public class ActiveUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public BitmapImage Img { get; set; }
        public ActiveUser(string n, string e, BitmapImage i)
        {
            Name = n;
            Email = e;
            Img = i;
        }
    }
    public partial class ActiveUsers : Page
    {
        public string address = @"c:\Chat\Files\ChatFiles";
        public BitmapImage src = new BitmapImage();
        public List<ActiveUser> A = new List<ActiveUser>();
        public ActiveUsers(List<list> a)
        {
            InitializeComponent();
            foreach (list i in a)
            {
                LoadImage(i.Img);
                A.Add(new ActiveUser(i.Name, i.Email, src));
                string filename = address + @"\" + i.Email + ".dat";
                if (!File.Exists(filename))
                {
                    var fs = File.Create(filename);
                    fs.Close();
                }
            }
            As.ItemsSource = A;
            Task.Factory.StartNew(() =>
            {
                Receive();
            });
        }
        public ActiveUsers()
        { }
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
        public string test = "";
        public bool i = true;
        List<ActiveUser> At = new List<ActiveUser>();
        public List<file> b = new List<file>();
        public void Receive()
        {
            while (i)
            {
                Console.WriteLine("In Thread");
                var buffer = new byte[2048000];
                int received = ServerConnection.ClientSocket.Receive(buffer);
                if (received != 0)
                {
                    var data = new byte[received];
                    Array.Copy(buffer, data, received);
                    string text = Encoding.ASCII.GetString(data);
                    //i = false;
                    test = test + text;
                    if (test.Substring(test.Length - 2, 2) == "$0")
                    {
                        string first = test.Substring(0, test.IndexOf("$"));
                        if (first == "ACTIVE_USER_LIST")
                        {
                            At.Clear();
                            string dat = test.Substring(test.IndexOf("$") + 1, test.IndexOf("$0") - test.IndexOf("$") - 1);
                            byte[] d = Encoding.ASCII.GetBytes(dat);
                            XmlSerializer xmlSerializer;
                            MemoryStream memStream = null;
                            xmlSerializer = new XmlSerializer(typeof(List<list>));
                            memStream = new MemoryStream(d);
                            object objectFromXml = xmlSerializer.Deserialize(memStream);
                            List<list> a = (List<list>)objectFromXml;
                            foreach (list z in a)
                            {
                                LoadImage(z.Img);
                                At.Add(new ActiveUser(z.Name, z.Email, src));
                                string filename = address + @"\" + z.Email + ".dat";
                                if (!File.Exists(filename))
                                {
                                    var fs = File.Create(filename);
                                    fs.Close();
                                }
                            }
                            a.Clear();
                            //Thread.Sleep(2000);
                            this.Dispatcher.Invoke(new Action(delegate ()
                            {
                                As.ItemsSource = At;

                            }));
                            memStream.Close();
                            //return next;
                        }
                        else if (first == "TextMessage")
                        {
                            string datas = test.Substring(test.IndexOf("$") + 1, test.IndexOf("$0") - test.IndexOf("$") - 1);
                            byte[] ds = Encoding.ASCII.GetBytes(datas);
                            XmlSerializer xmlSerializer;
                            MemoryStream memStream = null;
                            xmlSerializer = new XmlSerializer(typeof(textmessage));
                            memStream = new MemoryStream(ds);
                            object objectFromXml = xmlSerializer.Deserialize(memStream);
                            textmessage ob = (textmessage)objectFromXml;
                            memStream.Close();
                            Array.Clear(ds, 0, ds.Length);
                            BinaryFormatter formatter = new BinaryFormatter();
                            file messobj = new file();
                            messobj.Message = ob.Message;
                            messobj.Sendbyme = false;
                            string file = address + @"\" + ob.Sendemail + ".dat";
                            if (File.Exists(file))
                            {
                                // Create a FileStream will gain read access to the 
                                // data file.
                                FileStream readerFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                                // Reconstruct information of our friends from file.
                                try
                                {
                                    b = (List<file>)formatter.Deserialize(readerFileStream);
                                }
                                catch
                                {
                                    b = new List<file>();
                                }
                                readerFileStream.Dispose();
                                // Close the readerFileStream when we are done
                                readerFileStream.Close();
                            }

                            b.Add(messobj);
                            // Create a FileStream that will write data to file.
                            FileStream writerFileStream = new FileStream(file, FileMode.Create, FileAccess.Write);
                            // Save our dictionary of friends to file
                            formatter.Serialize(writerFileStream, b);
                            writerFileStream.Dispose();
                            // Close the writerFileStream when we are done.
                            writerFileStream.Close();
                            if (ob.Sendemail == getcurrentuser())
                            {
                                this.Dispatcher.Invoke(new Action(delegate ()
                                {
                                    Login.page.message.Content = new MessagePage(file);
                                }));
                            }
                            memStream.Flush();
                            memStream.Close();
                            //return next;
                        }
                        else if (first == "MediaMessage")
                        {
                            string datas = test.Substring(test.IndexOf("$") + 1, test.IndexOf("$0") - test.IndexOf("$") - 1);
                            byte[] ds = Encoding.ASCII.GetBytes(datas);
                            XmlSerializer xmlSerializer;
                            MemoryStream memStream = null;
                            xmlSerializer = new XmlSerializer(typeof(media));
                            memStream = new MemoryStream(ds);
                            object objectFromXml = xmlSerializer.Deserialize(memStream);
                            media ob = (media)objectFromXml;
                            memStream.Close();
                            Array.Clear(ds, 0, ds.Length);
                            BinaryFormatter formatter = new BinaryFormatter();
                            file messobj = new file();
                            messobj.Message = ob.Type + " is stored at "+ ob.Filename;
                            messobj.Sendbyme = false;
                            File.WriteAllBytes(ob.Filename,ob.Message);
                            string file = address + @"\" + ob.Sendemail + ".dat";
                            if (File.Exists(file))
                            {
                                // Create a FileStream will gain read access to the 
                                // data file.
                                FileStream readerFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                                // Reconstruct information of our friends from file.
                                try
                                {
                                    b = (List<file>)formatter.Deserialize(readerFileStream);
                                }
                                catch
                                {
                                    b = new List<file>();
                                }
                                readerFileStream.Dispose();
                                // Close the readerFileStream when we are done
                                readerFileStream.Close();
                            }
                            b.Add(messobj);
                            // Create a FileStream that will write data to file.
                            FileStream writerFileStream = new FileStream(file, FileMode.Create, FileAccess.Write);
                            // Save our dictionary of friends to file
                            formatter.Serialize(writerFileStream, b);
                            writerFileStream.Dispose();
                            // Close the writerFileStream when we are done.
                            writerFileStream.Close();
                            if (ob.Sendemail == getcurrentuser())
                            {
                                this.Dispatcher.Invoke(new Action(delegate ()
                                {
                                    Login.page.message.Content = new MessagePage(file);
                                }));
                            }
                            memStream.Flush();
                            memStream.Close();
                            //return next;
                        }
                        test = "";
                    }
                    else
                    {
                        Receive();
                    }
                }
            }
        }
        public static string cemail = "";
        private void ListViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var var2 = As.SelectedIndex;
            try
            {
                cemail = At[var2].Email;
                Console.WriteLine(At[var2].Email);
            }
             catch
            {
                cemail = A[var2].Email;
                Console.WriteLine(A[var2].Email);
            }
            string filename = address+@"\" + cemail+".dat";
            Login.page.SendVissble.IsEnabled = true;
            Login.page.message.Content = new MessagePage(filename);
        }

        public string getcurrentuser()
        {
            return cemail;
        }
    }
}
