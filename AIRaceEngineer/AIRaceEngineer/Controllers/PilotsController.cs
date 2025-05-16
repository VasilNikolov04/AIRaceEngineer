using AIRaceEngineer.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIRaceEngineer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PilotsController : ControllerBase
    {
        private static readonly List<Pilot> Drivers = new()
    {
    new Pilot { Id = 1, Name = "Max Verstappen", Abbreviation = "VER", Team = "Red Bull Racing" },
    new Pilot { Id = 2, Name = "Yuki Tsunoda", Abbreviation = "TSU", Team = "Red Bull Racing" },
    new Pilot { Id = 3, Name = "Charles Leclerc", Abbreviation = "LEC", Team = "Ferrari" },
    new Pilot { Id = 4, Name = "Lewis Hamilton", Abbreviation = "HAM", Team = "Ferrari" },
    new Pilot { Id = 5, Name = "Kimi Antonelli", Abbreviation = "ANT", Team = "Mercedes" },
    new Pilot { Id = 6, Name = "George Russell", Abbreviation = "RUS", Team = "Mercedes" },
    new Pilot { Id = 7, Name = "Lando Norris", Abbreviation = "NOR", Team = "McLaren" },
    new Pilot { Id = 8, Name = "Oscar Piastri", Abbreviation = "PIA", Team = "McLaren" },
    new Pilot { Id = 9, Name = "Fernando Alonso", Abbreviation = "ALO", Team = "Aston Martin" },
    new Pilot { Id = 10, Name = "Lance Stroll", Abbreviation = "STR", Team = "Aston Martin" },
    new Pilot { Id = 11, Name = "Franco Colapinto", Abbreviation = "COL", Team = "Alpine" },
    new Pilot { Id = 12, Name = "Pierre Gasly", Abbreviation = "GAS", Team = "Alpine" },
    new Pilot { Id = 13, Name = "Liam Lawson", Abbreviation = "LAW", Team = "RB (Visa Cash App RB)" },
    new Pilot { Id = 14, Name = "Isaac Hadjar", Abbreviation = "RIC", Team = "RB (Visa Cash App RB)" },
    new Pilot { Id = 15, Name = "Gabriel Bortoleto", Abbreviation = "GAB", Team = "Kick Sauber" },
    new Pilot { Id = 16, Name = "Nico Hülkenberg", Abbreviation = "ZHO", Team = "Kick Sauber" },
    new Pilot { Id = 17, Name = "Esteban Ocon", Abbreviation = "OCO", Team = "Haas" },
    new Pilot { Id = 18, Name = "Oliver Bearman", Abbreviation = "BEA", Team = "Haas" },
    new Pilot { Id = 19, Name = "Alexander Albon", Abbreviation = "ALB", Team = "Williams" },
    new Pilot { Id = 20, Name = "Carlos Sainz", Abbreviation = "SAI", Team = "Williams" }
};

        [HttpGet]
        public IActionResult GetDrivers()
        {
            return Ok(Drivers);
        }
    }
}
