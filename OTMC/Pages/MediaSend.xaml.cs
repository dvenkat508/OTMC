using System;
using System.Collections.Generic;
using System.IO;
using OTMC.Classes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Text;
using System.Net.Sockets;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for MediaSend.xaml
    /// </summary>
    public partial class MediaSend : Page
    {
        public string address = @"c:\Chat\Files\ChatFiles";
        public string Folder;
        public string filters;
        public static string[] files = { };
        public string send;
        public MediaSend(int type)
        {
            InitializeComponent();
            switch(type)
            {
                case 0:send = "Image";
                    title.Text = send +"s " + title.Text;
                    filters = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif";
                    Folder = @"c:\Chat\Images\";
                    Select(filters);
                    break;
                case 1:send = "Video";
                    title.Text = send + "s " + title.Text;
                    filters = "Video Files(*.mp4; *.mkv; *.vob; *.avi; *.3gp; *.flv; *.mov)|*.mp4; *.mkv; *.vob; *.avi; *.3gp; *.flv; *.mov";
                    Folder = @"c:\Chat\Videos\";
                    Select(filters);
                    break;
                case 2:send = "Audio";
                    title.Text = send + "s " + title.Text;
                    filters = "Audio Files(*.mp3; *.wav)|*.mp3; *.wav";
                    Folder = @"c:\Chat\Audio\";
                    Select(filters);
                    break;
                case 3:send = "Document";
                    title.Text = send + "s " + title.Text;
                    filters = "All files (*.*)|*.*";
                    Folder = @"c:\Chat\Files\";
                    Select(filters);
                    break;
            }            
        }

        public void Select(string filter)
        {
            Array.Clear(files, 0, files.Length); 
            files = ListFiles(filter);
            foreach(string str in files)
            {
                Selected_Item_List.Items.Add(str);                
            }            
        }
        public void sendmess(media Mess)
        {
            StreamWriter stream = null;
            if (stream == null)
            {
                XmlSerializer xmlSerializer;
                xmlSerializer = new XmlSerializer(typeof(media));
                MemoryStream memoryStream = new MemoryStream();
                stream = new StreamWriter(memoryStream);
                XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                xs.Add("", "");
                xmlSerializer.Serialize(stream, Mess, xs);
                byte[] buffer = memoryStream.GetBuffer();
                string data = "MediaMessage$" + Encoding.ASCII.GetString(buffer) + "$0";
                buffer a = new buffer();
                a.buff = Encoding.ASCII.GetBytes(data);
                ServerConnection.ClientSocket.Send(a.buff, 0, a.buff.Length, SocketFlags.None);
                stream.Dispose();
                memoryStream.Dispose();
                Array.Clear(buffer, 0, buffer.Length);
            }
        }
        public List<file> b = new List<file>();
        public void add()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string file = address + @"\" + ActiveUsers.cemail + ".dat";
            if (File.Exists(file))
            {
                FileStream readerFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);                
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
            file messobj = new file();
            foreach (string str in files)
            {
                messobj.Message = send + " \'" + str + "\'" + " is sent";
                messobj.Sendbyme = true;
                b.Add(messobj);
            }
            FileStream writerFileStream = new FileStream(file, FileMode.Create, FileAccess.Write);            
            formatter.Serialize(writerFileStream, b);
            writerFileStream.Dispose();            
            writerFileStream.Close();
            Login.page.message.Content = new MessagePage(file);
        }

        public string[] ListFiles(string filter)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = filter;
            file.ShowDialog();
            return file.FileNames;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login.page.sendgrid.Height = new GridLength(61);
            foreach (string str in files)
            {
                byte[] data = File.ReadAllBytes(str);
                string newfname = Folder + Path.GetFileName(str);
                media media = new media(data, Login.page.User_Email.Text, ActiveUsers.cemail, newfname, send);
                sendmess(media);
            }
            add();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ActiveUsers a = new ActiveUsers();
            string temp = a.getcurrentuser();
            string filename = @"c:\Chat\Files\ChatFiles" + @"\" + temp + ".dat";
            Login.page.sendgrid.Height = new GridLength(61);
            Login.page.message.Content = new MessagePage(filename);
        }
    }
}
