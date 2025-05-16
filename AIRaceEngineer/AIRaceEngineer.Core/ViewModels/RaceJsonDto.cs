using System.Text.Json.Serialization;

namespace AIRaceEngineer.Core.ViewModels
{
    public class RaceJsonDto
    {
        [JsonPropertyName("drivers")]
        public List<RaceEntry> Drivers { get; set; } = new();
    }
}
