using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using TWIL1.Models;
using TWIL1.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

using System.Threading.Tasks;
using Google.Cloud.Language.V1;

namespace TWIL1.Controllers
{
    public class HomeController : Controller
    {
        private readonly SentimentAnalysisService _sentimentAnalysisService;
        private readonly HttpClient _httpClient;

        public HomeController (HttpClient httpClient)
        {
            //_sentimentAnalysisService = sentimentAnalysisService;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
             return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSentiment(string tweet)
        {
            if (string.IsNullOrEmpty(tweet))
            {
                var viewModel = new SentimentAnalysisViewModel
                {
                    Text = "Please enter valid text To Analyze",
                };

                return View("Index", viewModel);
            }

            // Define the API endpoint URL
            var url = "http://49.176.251.173:12345/get_sentiment/";

            // Create the request object with content
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var data = new { model_name = "mistral", tweet = tweet };
            var jsonString = JsonSerializer.Serialize(data);

            // Set the request body with JSON content
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)

                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var sentimentResult = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);

                        var sentiment = sentimentResult["sentiment"];

                        var viewModel = new SentimentAnalysisViewModel
                        {
                            Text = sentiment.ToString(),
                            SentimentScore = 0.0,
                            SentimentMagnitude = 0.0,
                            TheTweet = tweet
                        };

                        return View("Index", viewModel);
                    }
                    catch (JsonException)
                    {
                        return View("Index");
                    }
                }
                else
                {
                    return View("Error", $"API call failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                var viewModel = new SentimentAnalysisViewModel
                {
                    Text = "Error Occurred Getting Sentiment Analysis",
                    SentimentScore = 0.0,
                    SentimentMagnitude = 0.0,
                    TheTweet = tweet
                };
                return View("Index", viewModel);
            }
            


        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var sentiment = await _sentimentAnalysisService.AnalyzeSentiment(text);

                var viewModel = new SentimentAnalysisViewModel
                {
                    Text = text,
                    SentimentScore = sentiment.Score,
                    SentimentMagnitude = sentiment.Magnitude,
                };

                return View("Index", viewModel);
            }
            else
            {
                return View("Index"); 
            } 

            
        }
    }
}
