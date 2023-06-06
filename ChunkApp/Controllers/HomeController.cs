using ChunkApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChunkApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // get random joke from the api
            var url = "https://api.chucknorris.io/jokes/random";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    var jsonContent = await result.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(jsonContent);
                    JokeDetail jokeDetail = new JokeDetail();
                    jokeDetail.DateModified = data.updated_at;
                    jokeDetail.Joke = data.value;
                    jokeDetail.OriginalId = data.id;
                    ViewBag.Joke = jokeDetail.Joke;

                    ViewBag.Actual = jokeDetail;
                    //storing joke details to pass to the next action method
                    TempData["OriginalId"] = jokeDetail.OriginalId;
                    TempData["Detail"] = jokeDetail.Joke;
                    TempData["Time"] = jokeDetail.DateModified;
                    return View(jokeDetail);
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult AddOrUpdateRecord(string button)
        {
            //receive the kept variables for saving to the database
            var id = TempData.Peek("OriginalId").ToString();
            var time = TempData.Peek("Time").ToString();
            var detail = TempData.Peek("Detail").ToString();
            int returnVal = 0;
            using (var db = new ChunkNorrisContext())
            {
                var result = db.JokeDetails.SingleOrDefault(b => b.OriginalId == id);
                if (result != null)
                { 
                    //update count of the existing joke
                    result.JokeCount++;
                    returnVal = result.Id;
                    db.SaveChanges();
                }
                else
                {
                    //create a new joke record in the database
                    var topId = db.JokeDetails.OrderByDescending(a => a.Id).FirstOrDefault();
                    JokeDetail jd = new JokeDetail();
                    jd.JokeCount = 1;
                    jd.OriginalId = id;
                    jd.Joke = detail;
                    jd.Id = topId.Id + 1;
                    jd.DateModified = Convert.ToDateTime(time);
                    jd.DateCreated = DateTime.Now;
                    returnVal = jd.Id;
                    db.Add(jd);
                    db.SaveChanges();
                }
                if (button == "true")
                { 
                    //make the saved joke your favourite
                    var joke = db.JokeDetails.Where(a => a.OriginalId == id).SingleOrDefault();
                    if (joke != null)
                    {
                        var favourite = db.Favourites.ToList();
                        if (favourite.Count() > 0)
                        {
                            var favour = db.Favourites.FirstOrDefault();
                            favour.JokeId = returnVal; // joke.Id;
                            db.SaveChanges();
                        }
                        else
                        {
                            //create new record if table is empty
                            Favourite f = new Favourite();
                            f.JokeId = returnVal;
                            db.Add(f);
                            db.SaveChanges();
                        }
                    }
                }
                    var favour1 = db.Favourites.FirstOrDefault();
                    if(favour1 != null)
                    {
                        var fav = db.JokeDetails.FirstOrDefault(a => a.Id == favour1.JokeId);
                        if (fav != null)
                        {
                            ViewBag.Favourite = fav.Joke;
                        }
                    }

                return new JsonResult(Ok(ViewBag.Favourite));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
