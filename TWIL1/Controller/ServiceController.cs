using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using TWIL1.ViewModel;
using Google.Cloud.Language.V1;

namespace TWIL1.Controllers
{
    public class ServiceController : Controller
    {
        private readonly HttpClient _httpClient;

        public ServiceController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSentiment(IFormFile File)
        {
            if (File == null || File.Length == 0)
            {
                var viewModel = new SentimentAnalysisListView
                {
                    error = "Please select a file to upload."
                };
                return View("Index", viewModel);
            }

            if (!File.ContentType.StartsWith("text/"))
            {
                var viewModel = new SentimentAnalysisListView
                {
                    error = "Invalid file type. Only text files are allowed."
                };
                return View("Index", viewModel);
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "analyze.txt");


            //upload file
            string filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", File.FileName);
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Create uploads folder if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var stream = new FileStream(filePath2, FileMode.Create))
            {
                await
                File.CopyToAsync(stream);
            }

            List<SentimentResult> results = new List<SentimentResult>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath2);

                foreach (string line in lines)
                {
                    var url = "http://49.176.251.173:12345/get_sentiment/";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var data = new { model_name = "mistral", tweet = line };
                    var jsonString = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await _httpClient.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = await response.Content.ReadAsStringAsync();
                            var sentimentResult = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
                            var sentiment = sentimentResult["sentiment"];
                            results.Add(new SentimentResult { Text = line, Sentiment = sentiment });
                        }
                        else
                        {
                        }
                    }
                    catch {
                        results.Add(new SentimentResult { Text = line, Sentiment = "Error Connecting Api" });
                    }
           
                }

                var viewModel = new SentimentAnalysisListView
                {
                    Results = results,
                    error = "none"
                };
                System.IO.File.Delete(filePath2);
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                var viewModel = new SentimentAnalysisListView
                {
                    error = ex.Message
                };
                return View("Index", viewModel);
            }
        }


    }
}
