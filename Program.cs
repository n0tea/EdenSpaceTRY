
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EdenSpaceTRY;
using Newtonsoft.Json;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        string token = "660ce6bc4396b660ce6bc4396e"; // Замените на ваш токен
        string routeUniverse = "https://datsedenspace.datsteam.dev/player/universe";
        string routeTravel = "https://datsedenspace.datsteam.dev/player/tarvel";
        string routeCollect = "https://datsedenspace.datsteam.dev/player/collect";

        string jsonResponse;

        using (var httpClient = new HttpClient())
        {
            // Добавляем токен в заголовок каждого запроса
            httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);//$"Bearer {token}");

            using (var response = await httpClient.GetAsync(routeUniverse))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(apiResponse);

                Console.WriteLine(apiResponse);
                jsonResponse = apiResponse;
                // Обрабатываем результат...
            }
        }
        ResponseData responseData = JsonConvert.DeserializeObject<ResponseData>(jsonResponse);

        string startName = responseData.ship.planet.name;

        // Создаем словарь для хранения планет и их маршрутов
        Dictionary<string, List<Route>> planetRoutes = new Dictionary<string, List<Route>>();

        foreach (var routeData in responseData.universe)
        {
            string sourcePlanet = (string)routeData[0];
            string targetPlanet = (string)routeData[1];
            long distance = (long)routeData[2];

            if (!planetRoutes.ContainsKey(sourcePlanet))
            {
                planetRoutes[sourcePlanet] = new List<Route>();
            }

            planetRoutes[sourcePlanet].Add(new Route { TargetPlanet = targetPlanet, Distance = distance });
        }

        if (planetRoutes.ContainsKey(startName))
        {
            List<Route> routesFromStartPlanet = planetRoutes[startName];

            // Выводим список маршрутов для начальной планеты
            Console.WriteLine($"Маршруты из планеты {startName}:");
            foreach (var route in routesFromStartPlanet)
            {
                Console.WriteLine($"Планета: {route.TargetPlanet}, Расстояние: {route.Distance}");
            }
        }

        var shortestPathToEden = FindShortestPathToEden(planetRoutes, startName);

        if (shortestPathToEden != null)
        {
            Console.WriteLine("\nСамый короткий путь до планеты Eden:");
            foreach (var planet in shortestPathToEden.Planets)
            {
                Console.WriteLine(planet);
            }
        }
        else
        {
            Console.WriteLine("\nПланета Eden недоступна.");
        }

        static PlanetsPath FindShortestPathToEden(Dictionary<string, List<Route>> planetRoutes, string startPlanet)
        {
            var distances = new Dictionary<string, long>();
            var previous = new Dictionary<string, string>();
            var queue = new HashSet<string>();

            foreach (var planetName in planetRoutes.Keys)
            {
                distances[planetName] = long.MaxValue;
                previous[planetName] = null;
                queue.Add(planetName);
            }

            distances[startPlanet] = 0;

            while (queue.Count > 0)
            {
                var currentPlanet = ExtractMin(queue, distances);
                if (currentPlanet == "Eden")
                {
                    break;
                }

                foreach (var route in planetRoutes[currentPlanet])
                {
                    var alt = distances[currentPlanet] + route.Distance;
                    if (alt < distances[route.TargetPlanet])
                    {
                        distances[route.TargetPlanet] = alt;
                        previous[route.TargetPlanet] = currentPlanet;
                    }
                }
            }

            if (!previous.ContainsKey("Eden")) return null;

            var planetsPath = new PlanetsPath();
            var planet = "Eden"; // Начинаем с планеты Eden
            while (planet != null && planet != startPlanet)
            {
                planetsPath.Planets.Insert(0, planet);
                planet = previous[planet];
            }

            // Добавляем стартовую планету в начало списка планет
           // planetsPath.Planets.Insert(0, startPlanet);

            return planetsPath;
        }

        static string ExtractMin(HashSet<string> queue, Dictionary<string, long> distances)
        {
            var minDistance = long.MaxValue;
            string minPlanet = null;

            foreach (var planet in queue)
            {
                if (distances[planet] < minDistance)
                {
                    minDistance = distances[planet];
                    minPlanet = planet;
                }
            }

            queue.Remove(minPlanet);
            return minPlanet;
        }

        var pathJson = JsonConvert.SerializeObject(shortestPathToEden);

        // Создаем StringContent с JSON строкой
        //var contentRoute = new StringContent(pathJson, Encoding.UTF8, "application/json");
        
        using (var httpClient = new HttpClient())
        {
            // Добавляем токен в заголовок каждого запроса
            httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);//$"Bearer {token}");
            var content = new StringContent(pathJson, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync(routeTravel, content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(apiResponse);
                dynamic result = JsonConvert.DeserializeObject(apiResponse);
                //jsonResponse = apiResponse;
                // Обрабатываем результат...
            }
        }
    }
}