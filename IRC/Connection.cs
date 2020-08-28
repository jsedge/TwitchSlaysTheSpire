using TwitchSlaysTheSpire.Common;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;

namespace TwitchSlaysTheSpire.IRC
{
    public class Connection
    {
        internal const string Server = "irc.twitch.tv";
        internal const int Port = 6667;

        private TcpClient Client { get; set; }

        private NetworkStream Stream { get; set; }

        private StreamReader Reader { get; set; }
        private StreamWriter Writer { get; set; }
        private string Channel { get; set; }
        private bool StopRequested { get; set; }

        private bool InChannel { get; set; }

        public ConcurrentQueue<Command> Queue { get; set; }

        public void Connect(string channel, string username, string password){
            Client = new TcpClient(Server, Port);
            Stream = Client.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);
            Channel = channel; // Need this for future messages, so keep it around, don't need user/pass though
            WriteMessage($"PASS {password}");
            WriteMessage($"NICK {username}");
            WriteMessage($"JOIN #{channel}");
        }

        public void Listen(){
            // :kingguppie!kingguppie@kingguppie.tmi.twitch.tv PRIVMSG #kingguppie :hello bot
            while(!StopRequested){
                var message = GetMessage();
                //Console.WriteLine(message);
                if(message.Contains("/NAMES")){
                    // Confirms we're in the channel, so say something i guess?
                    WriteMessage($"PRIVMSG #{Channel} :Hello world!");
                    InChannel = true;
                }

                if(InChannel){
                    // Start parsing out stuff
                    var command = message.Split(' ')[1];
                    if(command == "PING"){
                        WriteMessage("PONG :tmi.twitch.tv");
                    }else if(command == "PRIVMSG"){
                        // Message, do a thing with it
                        HandleMessage(message.Remove(message.LastIndexOf(':')));
                        
                    }else {
                        // Not expected to happen.
                    }
                }
            }
        }

        public void HandleMessage(string message){
            if(message.StartsWith('!')){
                message = message.Remove(1); // Pull the ! out 
                if(message.StartsWith("help")){
                    // Help message
                }else{
                    try{
                        var command = CommandFactory.CreateCommand(message);
                        Queue.Enqueue(command);
                    }
                    catch (Exception){
                        // ¯\_(ツ)_/¯
                    }
                }

            }
        }

        public void Stop(){
            StopRequested = true;
        }

        private void WriteMessage(string message){
            Writer.WriteLine(message);
            Writer.Flush();
        }

        private string GetMessage(){
            return Reader.ReadLine();
        }
    }
}
