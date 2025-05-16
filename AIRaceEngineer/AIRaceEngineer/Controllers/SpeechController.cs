using AIRaceEngineer.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIRaceEngineer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string apiKey;
        public SpeechController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            apiKey = configuration.GetValue<string>("AppSettings:ApiKey");
        }

        [HttpPost("transcribe")]
        public async Task<IActionResult> Transcribe([FromForm] AudioInputModel model)
        {
            if (model.AudioFile == null || model.AudioFile.Length == 0)
                return BadRequest("No audio file provided.");

            if (string.IsNullOrEmpty(model.RaceJson))
            {
                return BadRequest("Driver data is missing.");
            }


            var uploadsPath = Path.Combine(webHostEnvironment.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, "audio.wav");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.AudioFile.CopyToAsync(stream);
            }

            string root = Path.GetDirectoryName(Path.GetDirectoryName(webHostEnvironment.ContentRootPath));

            string pythonExecutable = Path.Combine(root, "PythonScripts", ".venv", "Scripts", "python.exe");
            string scriptPath = Path.Combine(root, "PythonScripts", "whisper_transcribe.py");

            var psi = new ProcessStartInfo
            {
                FileName = pythonExecutable,
                Arguments = $"\"{scriptPath}\" \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string resultText;
            string errorText = "";
            using (var process = Process.Start(psi))
            {
                resultText = await process.StandardOutput.ReadToEndAsync();
                errorText = await process.StandardError.ReadToEndAsync();

            }


            if (!string.IsNullOrWhiteSpace(errorText))
            {
                return StatusCode(500, new { error = "Error running Python script", detail = errorText });
            }

            try
            {

                var json = JsonSerializer.Deserialize<Dictionary<string, string>>(resultText.Trim());

                if (json == null || !json.ContainsKey("text"))
                {
                    return StatusCode(500, new { error = "Invalid response format", rawOutput = resultText });
                }

                string transcript = json["text"];

                RaceJsonDto? raceEntries;
                try
                {
                    raceEntries = JsonSerializer.Deserialize<RaceJsonDto>(model.RaceJson);
                    if (raceEntries == null || raceEntries.Drivers == null)
                        throw new Exception("Deserialized race data is null.");
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Invalid race data JSON", detail = ex.Message });
                }

                string raceContext = FormatRaceContextForAI(raceEntries.Drivers);
                string? aiResponse = await GetRaceEngineerResponse(transcript, raceContext);

                return Ok(new { transcript, aiResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to parse transcription result", detail = ex.Message, rawOutput = resultText });
            }
        }

        private async Task<string?> GetRaceEngineerResponse(string transcript, string raceContext)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.DefaultRequestHeaders.Add("Referer", "https://localhost");

            var requestBody = new
            {
                model = "qwen/qwen3-4b:free",
                messages = new[]
                {
            new { role = "system", content = "You are a professional F1 race engineer. The user is the driver, and you communicate with them during the race. Keep your responses as short as possible (1 sentence if possible), clear, and focused solely on race-related matters. Use technical racing terminology and refer to the driver by their name if provided; otherwise, simply address them as 'driver' or remain neutral. If the user’s message is unrelated to racing or contains unclear requests, ask them to repeat or clarify. Always provide concise, actionable feedback based on the current race conditions, including but not limited to lap times, tire status, gaps to competitors, fuel levels, and track conditions. Each request will include a JSON with the current driver positions. The driver’s position in the race should always be denoted as 'YOU.' Focus entirely on race-related communication, and refrain from offering advice or responses outside of this context. For more realism you can refer the drivers as only their first or last name." },
            new { role = "user", content = raceContext + "\n\n" + transcript }
        }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                content
            );

            var resultJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(resultJson);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }

        private string FormatRaceContextForAI(List<RaceEntry> race)
        {
            var sb = new StringBuilder("Race Situation:\n");

            for (int i = 0; i < race.Count; i++)
            {
                var entry = race[i];
                string gap = entry.Gap.ToLower() == "first" ? "First" : $"+{entry.Gap}s";
                sb.AppendLine($"p{i + 1}. {entry.Name} ({entry.Abbreviation}) - {gap}, ");
            }

            return sb.ToString();
        }
    }
}
