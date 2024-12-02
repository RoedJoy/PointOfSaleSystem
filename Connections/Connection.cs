using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPos.Connections
{
    public class Connection
    {
        public static string ConnectionString()
        {
            return "Data Source=LAPTOP-UBGAH14Q\\SQLEXPRESS;Initial Catalog=MyPos;Integrated Security=True;";
        }
    }
}
