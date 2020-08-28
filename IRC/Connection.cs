using TwitchSlaysTheSpire.Common;

using System;
using System.Text;
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

        public Bridge CommunicationBridge { get; set; }

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
                }else if(message.StartsWith("boss")){
                    WriteMessage($"PRIVMSG #{Channel} :The boss of this zone is: {CommunicationBridge.BossName}");
                }else if(message.StartsWith("debug")){
                    var sb = new StringBuilder();
                    sb.Append($"PRIVMSG #{Channel} :");
                    sb.Append("I'm alive and listening for Twitch messages. ");
                    sb.Append($"Last message from the game was processed at {CommunicationBridge.TimeStamp}");
                    sb.Append($"Current Time is {CommunicationBridge.TimeStamp}");
                    WriteMessage(sb.ToString());
                }else{
                    try{
                        var command = CommandFactory.CreateCommand(message);
                        CommunicationBridge.Queue.Enqueue(command);
                        CommunicationBridge.QueueFlag.Set();
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
