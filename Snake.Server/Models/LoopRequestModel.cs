using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class LoopRequestModel
    {
        public Server.GameClasses.Player.TurnState Turn { get; set; }
        public bool Boost { get; set; }
        public bool Break { get; set; }
        public bool Shoot { get; set; }
    }
}
