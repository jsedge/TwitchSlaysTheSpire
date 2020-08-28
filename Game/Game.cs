using TwitchSlaysTheSpire.Common;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace TwitchSlaysTheSpire.Game
{
    public class GameCommunicator
    {
        public ConcurrentQueue<Command> Queue { get; set; }

        private Message CurrentMessage { get; set; }

        internal readonly int SecondsBetweenStatePoll = 5;
        public void Listen(){
            Console.WriteLine("ready");
            var stateUpdater = new Timer(Poll, null, 0, 60 * SecondsBetweenStatePoll);

            while(true){
                
                Command command = null;
                try{
                    while(!Queue.TryDequeue(out command)){
                        Log("Failed to dequeue anything");
                        System.Threading.Thread.Sleep(1000);
                    }
                    if(command.Validate(CurrentMessage.State)){
                        Console.WriteLine(command);
                        PullState();
                        Queue.Clear(); // Nothing in the queue is valid after we take an action, flush it
                    }
                }
                catch(Exception e){
                    Log(e.ToString());
                    Queue.Enqueue(command);
                    Console.WriteLine("STATE");
                }
            }
        }

        private void Poll(object _){
            PullState();
        }

        private void PullState() {
            Console.WriteLine("STATE");
            var stuff = Console.ReadLine();
            File.WriteAllText($"{Environment.GetEnvironmentVariable("HOME")}/last_message.json", stuff);
            var temp = JsonSerializer.Deserialize<Message>(stuff);
            if(temp?.State?.Combat != null)
                CurrentMessage = temp; // Only save it if it actually has a state
        }

        private void Log(string text){
            File.AppendAllText($"{Environment.GetEnvironmentVariable("HOME")}/sts.log", text + Environment.NewLine);
        }
    }
}
