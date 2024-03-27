using System.Net.Http.Headers;
namespace MovieLibrary.API
{
    public class EdenAI
    {
        public async Task<string> Get(string title)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.edenai.run/v2/image/generation"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiNDRlYTg1OWItOWU0MS00MjExLWExNTctZTRkZjk0Y2JlOTIzIiwidHlwZSI6ImFwaV90b2tlbiJ9.dGqqKRL5scZwaTW1JUEvqnOABUWF6FtUASlFieFmj4o" },
                },
                Content = new StringContent($"{{\"response_as_dict\":false,\"attributes_as_list\":false,\"show_original_response\":false,\"resolution\":\"1024x1024\",\"num_images\":1,\"providers\":\"replicate\",\"text\":\"{title} movie poster\",\"fallback_providers\":\"amazon\"}}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }
    }
}
