using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class RequestModel
    {
        public string Action { get; set; }
        public object Data { get; set; }
    }
}
