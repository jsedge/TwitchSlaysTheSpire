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

        public ConcurrentQueue<string> Queue { get; set; }

        public void Connect(string channel, string username, string password){
            Client = new TcpClient(Server, Port);
            Stream = Client.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);
            Channel = channel; // Need this for future messages, so keep it around
            //WriteMessage($"USER {username}");
            WriteMessage($"PASS {password}");
            WriteMessage($"NICK {username}");
            //Console.WriteLine("Connected probably");
            //Console.WriteLine(GetMessage());
            WriteMessage($"JOIN #{channel}");
            //Console.WriteLine(GetMessage());
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
                    var parts = message.Split(' ');
                    var command = parts[1];
                    if(command == "PING"){
                        WriteMessage("PONG :tmi.twitch.tv");
                    }else if(command == "PRIVMSG"){
                        // Message, do a thing with it
                        var enteredText = string.Join(' ', parts[3..parts.Length]).Substring(1);
                        if(enteredText[0] == '!')
                            Queue.Enqueue(enteredText);
                    }else {
                        // Not expected to happen.
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
