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

        

        public async Task<string> uploadToImgBb(string localOrUrl, string nome, string cognome)
        {
            byte[] imgBytes;

            // Legge immagine da URL o file locale
            if (Uri.IsWellFormedUriString(localOrUrl, UriKind.Absolute))
            {
                imgBytes = await _httpClient.GetByteArrayAsync(localOrUrl);
            }
            else
            {
                if (!File.Exists(localOrUrl))
                    throw new FileNotFoundException("Immagine non trovata", localOrUrl);

                imgBytes = await File.ReadAllBytesAsync(localOrUrl);
            }

            var base64Img = Convert.ToBase64String(imgBytes);

            // Solo il campo image nel form
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(base64Img), "image");

            // URL con la key come query string
            var url = $"https://api.imgbb.com/1/upload?key={_apiImg}&name={nome}_{cognome}";

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Errore ImgBB ({response.StatusCode}): {responseBody}");

            var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement.GetProperty("data").GetProperty("url").GetString();
        }
        public async Task<JsonDocument> GetUsersFromNamespaceAsync(string namespaceName)
        {
            var url = $"{BaseUrl}/account/users.json?{AuthQuery}&namespaces={namespaceName}";

            Console.WriteLine("GetUsersFromNamespaceAsync URL: " + url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("GetUsersFromNamespaceAsync Response:");
            Console.WriteLine(JsonSerializer.Serialize(
                JsonDocument.Parse(json),
                new JsonSerializerOptions { WriteIndented = true }
            ));

            return JsonDocument.Parse(json);
        }
        public async Task<JsonDocument> DeleteUserAsync(string nome, string cognome, string namespaceName)
        {
            nome = nome.Trim().ToLower();
            cognome = cognome.Trim().ToLower();

            var uid = $"{nome}.{cognome}@{namespaceName}";

            // 1️⃣ Recupera tutti i tag dell'utente
            var getTagsUrl = $"{BaseUrl}/tags/get.json?{AuthQuery}&uids={uid}&limit=1000";

            Console.WriteLine("DeleteUser - GetTags URL: " + getTagsUrl);

            var getResponse = await _httpClient.GetAsync(getTagsUrl);
            getResponse.EnsureSuccessStatusCode();

            var getJson = await getResponse.Content.ReadAsStringAsync();
            var getDoc = JsonDocument.Parse(getJson);

            if (!getDoc.RootElement.TryGetProperty("photos", out var photos))
                throw new Exception("Nessun tag trovato per questo utente.");

            var tids = new List<string>();

            foreach (var photo in photos.EnumerateArray())
            {
                if (!photo.TryGetProperty("tags", out var tags))
                    continue;

                foreach (var tag in tags.EnumerateArray())
                {
                    var tid = tag.GetProperty("tid").GetString();
                    tids.Add(tid);
                }
            }

            if (!tids.Any())
                throw new Exception("Nessun tag associato all'utente.");

            // 2️⃣ Rimuove tutti i tag in una sola chiamata
            var tidsString = string.Join(",", tids);

            var removeUrl = $"{BaseUrl}/tags/remove.json?{AuthQuery}&tids={tidsString}";

            Console.WriteLine("DeleteUser - RemoveTags URL: " + removeUrl);

            var removeResponse = await _httpClient.PostAsync(removeUrl, null);
            removeResponse.EnsureSuccessStatusCode();

            var removeJson = await removeResponse.Content.ReadAsStringAsync();

            Console.WriteLine("DeleteUser - Remove Response:");
            Console.WriteLine(JsonSerializer.Serialize(
                JsonDocument.Parse(removeJson),
                new JsonSerializerOptions { WriteIndented = true }
            ));

            // 3️⃣ Retrain per aggiornare il modello
            await TrainUserAsync(uid);

            return JsonDocument.Parse(removeJson);
        }


    }

    
    }
