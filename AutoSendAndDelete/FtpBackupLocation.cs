using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSendAndDelete
{
    public class FtpBackupLocation
    {
        public FtpBackupLocation(string name, string server, string user, string password)
        {
            Name = name;
            Server = server;
            User = user;
            Password = password;
        }
        public string Name { get; }
        public string Server { get; }
        public string User { get; }
        public string Password { get; }
    }

}

