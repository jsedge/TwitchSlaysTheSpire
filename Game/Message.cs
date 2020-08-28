using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TwitchSlaysTheSpire.Game
{
    public class Message
    {
        [JsonPropertyName("available_commands")]
        public List<string> Commands { get; set; }

        [JsonPropertyName("ready_for_command")]
        public bool Ready { get; set; }

        [JsonPropertyName("game_state")]
        public GameState State { get; set; }
    }

    public class GameState
    {
        [JsonPropertyName("choice_list")]
        public List<string> Choices { get; set; }

        [JsonPropertyName("combat_state")]
        public CombatState Combat { get; set; }
    }

    public class CombatState
    {
        [JsonPropertyName("monsters")]
        public List<object> Monsters { get; set; }

        [JsonPropertyName("hand")]
        public List<Card> Cards { get; set; }

        [JsonPropertyName("potions")]
        public List<Potion> Potions { get; set; }
    }

    public class Card
    {
        [JsonPropertyName("has_target")]
        public bool HasTarget { get; set; }

        [JsonPropertyName("is_playable")]
        public bool IsPlayable { get; set; }
    }

    public class Potion
    {
        [JsonPropertyName("requires_target")]
        public bool RequiresTarget { get; set; }
        [JsonPropertyName("can_use")]
        public bool CanUse { get; set; }
        [JsonPropertyName("can_discard")]
        public bool CanDiscard { get; set; }
    }
}