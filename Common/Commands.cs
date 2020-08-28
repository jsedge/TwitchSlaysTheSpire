using System;
using System.Collections.Generic;

namespace TwitchSlaysTheSpire.Common
{
   
    public class Command
    {
        public Verb Action { get; }

        public Command(Verb action){
            Action = action;
        }

        public override string ToString(){
            return $"{Action}{Environment.NewLine}";
        }
    }

    public class ChooseCommand : Command
    {
        public ChooseCommand(Verb action) : base(action)
        {

        }
        public uint Index { get; set; }

        public override string ToString() {
            return $"CHOOSE {Index}{Environment.NewLine}";
        }
    }

    public class PlayCommand : Command
    {
        public PlayCommand(Verb action) : base(action)
        {

        }
        public uint CardIndex { get; set; } 
        public uint? MonsterIndex { get; set; }

        public override string ToString(){
            return $"PLAY {CardIndex} {MonsterIndex}{Environment.NewLine}";
        }
    }

    public class PotionCommand : Command
    {
        public PotionCommand(Verb action) : base(action)
        {

        }
        public PotionAction PotionAction { get; set; }
        public uint PotionIndex { get; set; }

        public uint? TargetIndex { get; set; }

        public override string ToString(){
            return $"POTION {PotionAction} {PotionIndex} {TargetIndex}{Environment.NewLine}";
        }
    }

    public class StartCommand : Command
    {
        public StartCommand(Verb action) : base(action){

        }

        public Character CharacterToPlay { get; set; }

        public uint? AcensionLevel { get; set; }

        public string Seed { get; set; }

        public override string ToString(){
            return $"START {CharacterToPlay} {AcensionLevel} {Seed}{Environment.NewLine}";
        }
    }

    public static class CommandFactory
    {
        public static Command CreateCommand(string commandInfo){
            var parts = new Queue<string>(commandInfo.Split(' '));
            var action = (Verb)Enum.Parse(typeof(Verb), parts.Dequeue(), true);

            switch(action){
                case Verb.Choose:
                    if(parts.Count == 0){
                        // Choose requires one argument
                        throw new Exception("Missing required arguments");
                    }
                    return new ChooseCommand(action){
                        Index = uint.Parse(parts.Dequeue())
                    };
                case Verb.Potion:
                    if(parts.Count < 2){
                        // Potion requires 2 to 3 arguments
                        throw new Exception("Missing required arguments");
                    }
                    return new PotionCommand(action){
                        PotionAction = (PotionAction)Enum.Parse(typeof(PotionAction), parts.Dequeue()),
                        PotionIndex = uint.Parse(parts.Dequeue()),
                        TargetIndex = parts.TryPeek(out string _) ? (uint?)uint.Parse(parts.Dequeue()) : null
                    };
                case Verb.Play:
                    if(parts.Count == 0){
                        // Potion requires 1 to 2 arguments
                        throw new Exception("Missing required arguments");
                    }
                    return new PlayCommand(action){
                        CardIndex = uint.Parse(parts.Dequeue()),
                        MonsterIndex = parts.TryPeek(out string _) ? (uint?)uint.Parse(parts.Dequeue()) : null
                    };
                case Verb.Start:
                    if(parts.Count == 0){
                        throw new Exception("Missing required arguments");
                    }
                    return new StartCommand(action){
                        CharacterToPlay = (Character)Enum.Parse(typeof(Character), parts.Dequeue()),
                        AcensionLevel = parts.TryPeek(out string _) ? (uint?)uint.Parse(parts.Dequeue()) : null,
                        Seed = parts.TryPeek(out string _) ? parts.Dequeue() : null
                    };
                default:
                    return new Command(action);
            }
        }
    }
}
