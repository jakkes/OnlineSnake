namespace Snake.Server
{
    public class Config
    {
        public static Config data { get; set; }

        public int SERVER_PORT { get; set; }
        public int BASE_BOARD_HEIGHT { get; set; }
        public int BASE_BOARD_WIDTH { get; set; }

        public int BASE_LENGTH { get; set; }
        public int BASE_MOVEMENT_SPEED { get; set; }
        public int BASE_MOVEMNET_LENGTH { get; set; }
        public int BASE_MOVEMENT_LENGTH { get; set; }
        public int BASE_TURN_SPEED { get; set; }
        public int SNAKE_RADIUS { get; set; }
        public int BOOST_CAP { get; set; }

        public int SHOT_RADIUS { get; set; }
        public int SHOT_MOVEMENT_SPEED { get; set; }
        public int SHOT_MOVEMENT_LENGTH { get; set; }

        public double DEAD_SNAKE_TURN_PROBABILITY { get; set; }

        public int AMMO_DURATION { get; set; }
        public int AMMO_SPAWN_TIME { get; set; }
        public int AMMO_RADIUS { get; set; }

        public int FOOD_DURATION { get; set; }
        public int FOOD_SPAWN_TIME { get; set; }
        public int FOOD_RADIUS { get; set; }
        public int FOOD_GROW { get; set; }

        public int ARMOR_DURATION { get; set; }
        public int ARMOR_SPAWN_TIME { get; set; }
        public int ARMOR_RADIUS { get; set; }

        public int LOOP_TIMER { get; set; }

        public int VISIBILITY_RADIUS { get; set; }

        public int SCREEN_WIDTH { get; set; }
        public int SCREEN_HEIGHT { get; set; }

        public string[] COLORS { get; set; }
        public string DEAD_SNAKE_COLOR { get; set; }

        //public static string BaseFilePath = @"C:\Users\Jakob\Documents\Visual Studio 2013\Projects\OnlineSnake\Snake.Server\";
        public static string BaseFilePath = @"C:\inetpub\wwwroot\Snake.Server\";
        //public static string BaseFilePath = @"C:\Users\jakostig\Documents\GitHub\OnlineSnake\Snake.Server\";
        
        public static string LeaderboardFile = BaseFilePath + @"Data\Leaderboard.json";
        public static string PersonalHighscoreFile = BaseFilePath + @"Data\PersonalHighscore.json";
    }
}
