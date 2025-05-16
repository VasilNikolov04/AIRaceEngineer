using System.Text.Json.Serialization;

namespace AIRaceEngineer.Core.ViewModels
{
    public class RaceEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("team")]
        public string Team { get; set; } = string.Empty;
        [JsonPropertyName("abr")]
        public string Abbreviation { get; set; } = string.Empty;
        [JsonPropertyName("gap")]
        public string Gap { get; set; } = string.Empty;
    }
}