using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projectoFinalV2.Models;
using Newtonsoft.Json;

namespace projectoFinalV2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Index()
        {
            const string apiUrl = "https://ghibliapi.vercel.app/films";

            var client = new HttpClient();
            var response = client.GetAsync(apiUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            List<Film> model = JsonConvert.DeserializeObject<List<Film>>(content);
            return View(model);
        }

        public IActionResult Privacy()
        {
            var viewModel = new FilmPeople
            {
                Films = GetFilmsData(),
                People = GetPeopleData()
            };

            return View(viewModel);
        }

        public IActionResult Final(string filmId)
        {
            var film = GetFilmById(filmId);
            var people = GetPeopleByFilmId(filmId);

            var viewModel = new FilmPeople
            {
                Films = new List<Film> { film },
                People = people
            };

            return View(viewModel);
        }

        private Film GetFilmById(string filmId)
        {
            var filmsApiUrl = $"https://ghibliapi.vercel.app/films/{filmId}";
            using var client = new HttpClient();
            var response = client.GetAsync(filmsApiUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Film>(content);
        }

        private List<People> GetPeopleByFilmId(string filmId)
        {
            var allPeople = GetPeopleData();
            return allPeople.Where(p => p.films.Contains($"https://ghibliapi.vercel.app/films/{filmId}")).ToList();
        }

        private List<Film> GetFilmsData()
        {
            var filmsApiUrl = "https://ghibliapi.vercel.app/films";
            using var client = new HttpClient();
            var filmsResponse = client.GetAsync(filmsApiUrl).Result;

            if (filmsResponse.IsSuccessStatusCode)
            {
                var filmsContent = filmsResponse.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<Film>>(filmsContent);
            }
            else
            {
                return new List<Film>();
            }
        }

        private List<People> GetPeopleData()
        {
            var peopleApiUrl = "https://ghibliapi.vercel.app/people";
            using var client = new HttpClient();
            var peopleResponse = client.GetAsync(peopleApiUrl).Result;

            if (peopleResponse.IsSuccessStatusCode)
            {
                var peopleContent = peopleResponse.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<People>>(peopleContent);
            }
            else
            {
                return new List<People>();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
