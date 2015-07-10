using Snake.Server.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class GameLoopModel : ConnectionModel
    {
        public List<SnakeModel> Snakes { get; set; }
        public List<List<TranslatedPosition>> Borders { get; set; }
        public List<TranslatedPosition> Food { get; set; }
        public List<TranslatedPosition> Shots { get; set; }
        public List<TranslatedPosition> Ammo { get; set; }
        public List<TranslatedPosition> Armor { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public int AmmoCount { get; set; }
        public int BoostStored { get; set; }
        public bool PlayerArmor { get; set; }
        public int BreakStored { get; set; }

        public GameLoopModel()
        {
            Snakes = new List<SnakeModel>();
            Borders = new List<List<TranslatedPosition>>();
            Food = new List<TranslatedPosition>();
            Shots = new List<TranslatedPosition>();
            Ammo = new List<TranslatedPosition>();
            Armor = new List<TranslatedPosition>();
        }
    }
}
