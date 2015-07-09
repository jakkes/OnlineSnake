using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class OnlineDataModel : ConnectionModel
    {
        public LeaderModel CurrentLeader { get; set; }
        public List<LeaderModel> Leaderboard { get; set; }
        public int OnlineCount { get; set; }
        public List<PlayerModel> Players { get; set; }
        public int PersonalHighscore { get; set; }
    }
}