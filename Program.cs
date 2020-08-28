using System;
using TwitchSlaysTheSpire.Common;
using TwitchSlaysTheSpire.IRC;
using TwitchSlaysTheSpire.Game;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TwitchSlaysTheSpire
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfig();
            var queue = new ConcurrentQueue<Command>();
            var conn = new Connection(){
                Queue = queue
            };
            conn.Connect(config.Channel, config.Username, config.Password);
            var listener = new Task(conn.Listen);
            listener.Start();
            var game = new GameCommunicator(){
                Queue = queue
            };
            game.Listen();
            //System.Threading.Thread.Sleep(60 * 1000);
        }

        private static AuthenticationConfig GetConfig(){
            var text = File.ReadAllText($"{Environment.GetEnvironmentVariable("HOME")}/config.json");
            return JsonSerializer.Deserialize<AuthenticationConfig>(text);
        }
    }

    internal class AuthenticationConfig
    {
        public string Channel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
