using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;


namespace TWIL1
{
    public class SentimentAnalysisService
    {
        private readonly LanguageServiceClient _service;
        private readonly string _credentialsPath = "App_Data/credentials.json";

        public SentimentAnalysisService()
        {
            byte[] credentialData = File.ReadAllBytes(_credentialsPath);

            using (var stream = new MemoryStream(credentialData))
            {
                var credential = GoogleCredential.FromStream(stream);

                LanguageServiceClientBuilder builder = new LanguageServiceClientBuilder
                {
                    Credential = credential
                };

                _service = builder.Build();
            }
        }

        //public SentimentAnalysisService(IConfiguration configuration)
        //{
        //    _credentialsPath = configuration["App_Data/credentials.json"]; // Replace with actual key

        //    LanguageServiceClientBuilder builder = new LanguageServiceClientBuilder
        //    {
        //        CredentialsPath = _credentialsPath
        //    };

        //    _service = builder.Build();
        //}

        //public SentimentAnalysisService()
        //{
        //    LanguageServiceClientBuilder builder = new LanguageServiceClientBuilder
        //    {
        //        CredentialsPath = _credentialsPath
        //    };

        //    _service = builder.Build();
        //}

        //public SentimentAnalysisService(string credentialsPath)
        //{
        //    var credentials = GoogleCredential.FromJsonString(File.ReadAllText(credentialsPath));
        //    _service = LanguageServiceClientBuilder.FromServiceAccount(credentials).Build();
        //}

        public async Task<Sentiment> AnalyzeSentiment(string text)
        {
            var document = new Document
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            };

            var analyzeSentimentRequest = new AnalyzeSentimentRequest
            {
                Document = document
            };

            var response = await _service.AnalyzeSentimentAsync(analyzeSentimentRequest);
            return response.DocumentSentiment;
        }
    }


}
