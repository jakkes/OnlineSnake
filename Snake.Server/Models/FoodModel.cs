using Snake.Server.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class FoodModel
    {
        public TranslatedPosition Position { get; set; }
        public int RemainingTime { get; set; }
    }
}
