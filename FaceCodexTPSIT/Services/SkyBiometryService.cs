using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FaceCodexTPSIT.Services
{
    public class SkyBiometryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _apiImg;

        private const string BaseUrl = "https://api.skybiometry.com/fc";

        public SkyBiometryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["SkyBiometry:ApiKey"];
            _apiSecret = configuration["SkyBiometry:ApiSecret"];
            _apiImg = configuration["SkyBiometry:ImgBB"];
        }

        private string AuthQuery =>
            $"api_key={_apiKey}&api_secret={_apiSecret}";

        public async Task<JsonDocument> DetectFacesAsync(string imageUrl)
        {
            var url = $"{BaseUrl}/faces/detect.json?{AuthQuery}&urls={imageUrl}&attributes=all";
            Console.WriteLine("DetectFacesAsync URL: " + url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Stampa JSON formattato
            Console.WriteLine("DetectFacesAsync Response:");
            Console.WriteLine(JsonSerializer.Serialize(
                JsonDocument.Parse(json),
                new JsonSerializerOptions { WriteIndented = true }
            ));

            return JsonDocument.Parse(json);
        }

        public async Task<JsonDocument> SaveTagAsync(string uid, string tid)
        {
            var url = $"{BaseUrl}/tags/save.json?{AuthQuery}&uid={uid}&tids={tid}";

            var response = await _httpClient.PostAsync(url, null);
            Console.WriteLine("Url " + url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Stampa JSON formattato
            Console.WriteLine("SaveTagAsync Response:");
            Console.WriteLine(JsonSerializer.Serialize(
                JsonDocument.Parse(json),
                new JsonSerializerOptions { WriteIndented = true }
            ));

            return JsonDocument.Parse(json);
        }

        public async Task<JsonDocument> TrainUserAsync(string uid)
        {
            var url = $"{BaseUrl}/faces/train.json?{AuthQuery}&uids={uid}";

            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Stampa JSON formattato
            Console.WriteLine("TrainUserAsync Response:");
            Console.WriteLine(JsonSerializer.Serialize(
                JsonDocument.Parse(json),
                new JsonSerializerOptions { WriteIndented = true }
            ));

            return JsonDocument.Parse(json);
        }

        public async Task<JsonDocument> RecognizeAsync(string imageUrl, string namespaceName)
        {
            var uids = $"all@{namespaceName}";
            var url = $"{BaseUrl}/faces/recognize.json?{AuthQuery}&urls={imageUrl}&uids={uids}";
            Console.WriteLine(url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Stampa JSON formattato
            Console.WriteLine("RecognizeAsync Response:");
            Console.WriteLine(JsonSerializer.Serialize(
                JsonDocument.Parse(json),
                new JsonSerializerOptions { WriteIndented = true }
            ));

            return JsonDocument.Parse(json);
        }

        public async Task<String> uploadToImgBb(string localUrl, string nome, string cognome)
        {
            if (File.Exists(localUrl))
            {
                var imgBytes = await System.IO.File.ReadAllBytesAsync(localUrl);
                var url = $"https://api.imgbb.com/1/upload?key={_apiKey}&image={Convert.ToBase64String(imgBytes)}&name={nome}_{cognome}";

                var response = await _httpClient.PostAsync(url, null);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(json);

                return jsonDoc.RootElement.GetProperty("data").GetProperty("url").GetString();
            }

            else return null;
            

        }
    }
}
