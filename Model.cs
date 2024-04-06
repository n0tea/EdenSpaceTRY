using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdenSpaceTRY
{
    public class ResponseData
    {
        public string name { get; set; }
        public string roundName { get; set; }
        public int roundEndIn { get; set; }
        public Ship ship { get; set; }
        public List<List<object>> universe { get; set; }
        public int attempt { get; set; }

        public class Ship
        {
            public int fuelUsed { get; set; }
            public Planet planet { get; set; }
            public int capacityX { get; set; }
            public int capacityY { get; set; }
            public object garbage { get; set; }
        }
        public class Planet
        {
            public string name { get; set; }
            public object garbage { get; set; }
        }
    }
    public class PlanetsPath
    {
        [JsonProperty("planets")]
        public List<string> Planets { get; set; }
    }

    public class Route
    {
        public string TargetPlanet { get; set; }
        public long Distance { get; set; }
    }
}
