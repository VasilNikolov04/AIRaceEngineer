using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRaceEngineer.Infrastructure.Data.Models
{
    public class Pilot
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Abbreviation { get; set; } = null!;

        public string Team { get; set; } = null!;
    }
}
