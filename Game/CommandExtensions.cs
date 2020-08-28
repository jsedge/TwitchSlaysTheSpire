using TwitchSlaysTheSpire.Common;

namespace TwitchSlaysTheSpire.Game
{
    public static class CommandExtensions
    {
        public static bool Validate(this Command command, GameState state){
            return true;
        }

        public static bool Validate(this ChooseCommand command, GameState state){
            return state.Choices.Count >= command.Index;
        }

        public static bool Validate(this PlayCommand command, GameState state){
            if(state.Combat.Cards.Count < command.CardIndex)
                return false;
            var card = state.Combat.Cards[(int)command.CardIndex];
            if(card.HasTarget && 
                (!command.MonsterIndex.HasValue || state.Combat.Monsters.Count < command.MonsterIndex.Value))
                return false;
            return card.IsPlayable;
        }

        public static bool Validate(this PotionCommand command, GameState state){
            if(state.Combat.Potions.Count < command.PotionIndex)
                return false;
            var potion = state.Combat.Potions[(int)command.PotionIndex];

            if(command.PotionAction == PotionAction.Use){
                if(potion.RequiresTarget &&
                    (!command.TargetIndex.HasValue || state.Combat.Monsters.Count < command.TargetIndex.Value))
                    return false;
                return potion.CanUse;
            }else{
                return potion.CanDiscard;
            }            
        }
    }
}