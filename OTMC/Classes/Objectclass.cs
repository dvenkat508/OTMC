using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMC.Classes
{
    class Objectclass
    {
    }
    [Serializable]
    public class list
    {
        private string name;
        private string email;
        private byte[] img;
        public list(string name, string email, byte[] img)
        {
            this.name = name;
            this.email = email;
            this.img = img;
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public byte[] Img
        {
            get { return img; }
            set { img = value; }
        }
        public list()
        {

        }
    }
    public class textmessage
    {
        private string message;
        private string sendemail;
        private string toemail;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public string Sendemail
        {
            get { return sendemail; }
            set { sendemail = value; }
        }
        public string Toemail
        {
            get { return toemail; }
            set { toemail = value; }
        }
        public textmessage()
        { }
        public textmessage(string mes, string smil, string tmil)
        {
            message = mes;
            sendemail = smil;
            toemail = tmil;
        }
    }
    [Serializable]
    public class file
    {
        private string message;
        private bool sendbyme;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public bool Sendbyme
        {
            get { return sendbyme; }
            set { sendbyme = value; }
        }
        public file()
        {
            message = "";
            sendbyme = true;
        }
    }
    public class Register
    {
        private string name;
        private string emailid;
        private string password;
        private byte[] img;
        public Register()
        {

        }
        public string Name
        {
            get { return name;}
            set { name = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string Emailid
        {
            get { return emailid; }
            set { emailid = value; }
        }
        public byte[] Image
        {
            get { return img; }
            set { img = value; }
        }
        public Register(string name,string emailid,string password,byte[] img)
        {
            this.name = name;
            this.emailid = emailid;
            this.password = password;
            this.img = img;
        }
    }
    public class Loginob
    {
        private string emailid;
        private string password;
        public Loginob()
        { }
        public Loginob(string Emailid, string Password)
        {
            this.emailid = Emailid;
            this.password = Password;
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string Emailid
        {
            get { return emailid; }
            set { emailid = value; }
        }
    }
    [Serializable]
    public class media
    {
        private byte[] message;
        private string sendemail;
        private string toemail;
        private string filename;
        private string type;
        public byte[] Message
        {
            get { return message; }
            set { message = value; }
        }
        public string Sendemail
        {
            get { return sendemail; }
            set { sendemail = value; }
        }
        public string Toemail
        {
            get { return toemail; }
            set { toemail = value; }
        }
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public media()
        { }
        public media(byte[] mes, string smil, string tmil, string fname, string typ)
        {
            message = mes;
            sendemail = smil;
            toemail = tmil;
            filename = fname;
            type = typ;
        }
    }
    public class buffer
    {
        public byte[] buff;
        public buffer()
        {
        }
        ~buffer()
        {
        }
    }
}
