using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Capstone.Repositories.Interfaces;

namespace Capstone.Repositories.Implementations
{
    public class ImageService : IImageService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public ImageService(IConfiguration configuration)
        {
            _apiKey = configuration["Imgbb:ApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile.Length == 0)
                return null;

            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(_apiKey), "key");

                using (var stream = imageFile.OpenReadStream())
                {
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                    form.Add(streamContent, "image", imageFile.FileName);

                    var response = await _httpClient.PostAsync("https://api.imgbb.com/1/upload", form);
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var parsedResponse = JObject.Parse(jsonResponse);

                    string imageUrl = parsedResponse["data"]?["url"].ToString();
                    return imageUrl;
                }
            }
        }
    }
}