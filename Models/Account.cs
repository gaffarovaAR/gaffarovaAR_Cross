using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaffarovaAlbina.Models
{
    public class Account
    {
        public long ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
