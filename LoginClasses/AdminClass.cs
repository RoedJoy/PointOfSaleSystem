using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPos.LoginClasses
{
    public class AdminClass
    {
        private string username;
        private string password;
        private int count;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
