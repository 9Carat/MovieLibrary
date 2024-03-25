namespace MovieLibrary.API
{
    public class StreamingAPI
    {
        public async Task<string> Get(string title)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://streaming-availability.p.rapidapi.com/search/title?country=us&title={title}&output_language=en&show_type=all"),
                Headers =
                {
                    { "X-RapidAPI-Key", "727cc5bb1fmsha7fc3d23c88aa1cp1fc535jsnf00e48d27119" },
                    { "X-RapidAPI-Host", "streaming-availability.p.rapidapi.com" },
                },
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
