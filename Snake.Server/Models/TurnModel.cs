using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class TurnModel
    {
        public Server.GameClasses.Player.TurnState Turn { get; set; }
        public string Id { get; set; }
    }
}
