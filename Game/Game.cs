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
        public Bridge CommunicationBridge { get; set; }

        private Message CurrentMessage { get; set; }

        internal readonly int SecondsBetweenStatePoll = 5;
        public void Listen(){
            Console.WriteLine("ready");
            var stateUpdater = new Timer(Poll, null, 0, 60 * SecondsBetweenStatePoll);

            while(true){
                
                Command command = null;
                try{
                    if(!CommunicationBridge.Queue.TryDequeue(out command)){
                        Log("Failed to dequeue anything, entering wait.");
                        CommunicationBridge.QueueFlag.WaitOne();
                    }
                    if(command.Validate(CurrentMessage.State)){
                        Console.WriteLine(command);
                        PullState();
                        CommunicationBridge.Queue.Clear(); // Nothing in the queue is valid after we take an action, flush it
                    }
                }
                catch(Exception e){
                    Log(e.ToString());
                    CommunicationBridge.Queue.Enqueue(command);
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
            if(temp?.State?.Combat != null){
                CurrentMessage = temp; // Only save it if it actually has a state
                CommunicationBridge.BossName = CurrentMessage.State.BossName;
                CommunicationBridge.TimeStamp = DateTime.Now;
            }
        }

        private void Log(string text){
            File.AppendAllText($"{Environment.GetEnvironmentVariable("HOME")}/sts.log", text + Environment.NewLine);
        }
    }
}
