using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TwitchSlaysTheSpire.Game
{
    public class GameCommunicator
    {
        public enum Command
        {
            Start,
            Potion,
            Play,
            End,
            Choose,
            Proceed,
            Return,
            State
            
        }

        enum Character
        {
            Ironclad,
            Silent,
            Defect,
            Watcher
        }

        enum PotionAction
        {
            Use,
            Discard
        }

        public ConcurrentQueue<string> Queue { get; set; }

        private Message CurrentMessage { get; set; }
        public void Listen(){
            Console.WriteLine("ready");
            var stateUpdater = new Task(PullState);
            stateUpdater.Start();

            while(true){
                
                string action = "";
                try{
                    while(!Queue.TryDequeue(out action)){
                        Log("Failed to dequeue anything");
                        System.Threading.Thread.Sleep(1000);
                    }
                    ProcessMessage(action);
                }
                catch(Exception e){
                    Log(e.ToString());
                    Queue.Enqueue(action);
                    Console.WriteLine("STATE");
                }
            }
        }

        private void PullState() {
            while(true){
                Console.WriteLine("STATE");
                var stuff = Console.ReadLine();
                File.WriteAllText($"{Environment.GetEnvironmentVariable("HOME")}/last_message.json", stuff);
                CurrentMessage = JsonSerializer.Deserialize<Message>(stuff);
            }
        }

        private void ProcessMessage(string action){
            bool actionDone = false;
            while(!actionDone){
                Log(action);
                var parts = action.Split(' ');
                var command = (Command)Enum.Parse(typeof(Command), parts[0].Substring(1), true);
                switch(command){
                    case Command.Play:
                        int cardIndex, monsterIndex;
                        if(!int.TryParse(parts[1], out cardIndex))
                            continue;
                        int.TryParse(parts.Length > 2 ? parts[2] : "-1", out monsterIndex);
                        actionDone = HandlePlay(cardIndex, monsterIndex);
                        break;
                    case Command.Choose:
                        if(int.TryParse(parts[1], out int choice)){
                            HandleChoice(choice);
                            actionDone = true;
                        }else{
                            continue;
                        }
                        break;
                    case Command.Start:
                        if(Enum.TryParse(typeof(Character), parts[1], true, out object character)){
                            HandleStart((Character)character);
                            actionDone = true;
                        }else{
                            continue;
                        }
                        break;
                    case Command.End:
                    case Command.Proceed:
                    case Command.Return:
                    case Command.State:
                        Console.WriteLine(command);
                        actionDone = true;
                        break;
                    case Command.Potion:
                        if(Enum.TryParse(typeof(PotionAction), parts[1], true, out object potionAction)){
                            if(int.TryParse(parts[2], out int slot)){
                                int.TryParse(parts.Length >= 2 ? parts[3] : "-1", out int target);
                                actionDone = HandlePotion((PotionAction)potionAction, slot, target);
                            }
                        }
                        break;

                }
            }
            Queue.Clear();
        }

        private void HandleStart(Character character){
            // START PlayerClass [AcensionLevel] [Seed]
            Console.WriteLine($"START {character}");
        }

        private void HandleChoice(int choice){
            // CHOOSE ChoiceIndex|ChoiceName
            Console.WriteLine($"CHOOSE {CurrentMessage.State.Choices[choice -1]}");
        }

        private bool HandlePotion(PotionAction action, int slot, int target){
            // POTION Use|Discard PotionSlot [TargetIndex] 
            var potion = CurrentMessage.State.Combat.Potions[slot];
            
            if(PotionAction.Use == action && potion.CanUse){
                if(potion.RequiresTarget && target < 0){
                    return false;
                }
                Console.WriteLine($"POTION Use {slot} {target}");
                return true;
            }else if(PotionAction.Discard == action && potion.CanDiscard){
                Console.WriteLine($"POTION Use {slot}");
                return true;
            }
            return false;
        }

        private void Log(string text){
            File.AppendAllText($"{Environment.GetEnvironmentVariable("HOME")}/sts.log", text + Environment.NewLine);
        }

        private bool HandlePlay(int cardIndex, int monsterIndex){
            // PLAY CardIndex [TargetIndex]
            Log("Entered HandlePlay()");
            if(!CurrentMessage.State.Combat.Cards[cardIndex - 1].IsPlayable){
                Log($"Card at {cardIndex} isn't playable");
                return false;
            }
            if(CurrentMessage.State.Combat.Cards[cardIndex - 1].HasTarget){
                if(monsterIndex < 0){
                    return false;
                }
                Log($"Card has target, playing it");
                Console.WriteLine($"PLAY {cardIndex} {monsterIndex - 1}");
                return true;
            }else{
                Log("Card has no target, playing it");
                Console.WriteLine($"PLAY {cardIndex}");
                return true;
            }
        }
    }
}
