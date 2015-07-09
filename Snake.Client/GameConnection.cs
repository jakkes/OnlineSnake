using Newtonsoft.Json;
using Snake.Client.Models;
using Snake.Server;
using Snake.Server.Models;
using System;
using System.Net.Sockets;

namespace Snake.Client
{
    public static class GameConnection
    {

        public static Game Game { get; set; }

        public static GameLoopModel Loop(string Token, LoopRequestModel model)
        {
            try
            {
                return Game.GetLoopData(Token, model);
            }
            catch (GameException e)
            {
                if (e.Type == GameException.ExceptionType.InvalidToken)
                    return new GameLoopModel() { ConnectionCode = "404", ConnectionString = "Snake does not exist" };
                return new GameLoopModel() { ConnectionCode = "0", ConnectionString = "Unknown error" };
            }
        }

        public static ConnectionModel Join(string Token, string Name)
        {
            Game.Join(Token, Name);
            return new ConnectionModel();
        }

        public static void ReloadSettings()
        {
            Game.ReloadSettings();
        }

        public static ConnectionModel Shoot(string Token)
        {
            try
            {
                Game.Shoot(Token);
                return new ConnectionModel();
            }
            catch (GameException e)
            {
                if (e.Type == GameException.ExceptionType.InvalidToken)
                    return new GameLoopModel() { ConnectionCode = "404", ConnectionString = "Snake does not exist" };
                return new GameLoopModel() { ConnectionCode = "0", ConnectionString = "Unknown error" };
            }
        }

        public static OnlineDataModel GetOnlineData(string Token)
        {
            try
            {
                return Game.GetOnlineData(Token);
            }
            catch (GameException e)
            {
                if (e.Type == GameException.ExceptionType.InvalidToken)
                    return new OnlineDataModel() { ConnectionCode = "404", ConnectionString = "Snake does not exist" };
                return new OnlineDataModel() { ConnectionCode = "0", ConnectionString = "Unknown error" };
            }
        }

        public static Server.Config GetSettings()
        {
            return Game.Settings;
        }

        public class GameConnectionException : Exception
        {
            public ExceptionType Type { get; set; }

            public enum ExceptionType
            {
                Timeout
            }
        }
    }
}