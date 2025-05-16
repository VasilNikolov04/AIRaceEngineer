using Microsoft.AspNetCore.Http;

namespace AIRaceEngineer.Core.ViewModels
{
    public class AudioInputModel
    {
        public IFormFile AudioFile { get; set; } = null!;
        public string RaceJson { get; set; } = null!;
    }
}
