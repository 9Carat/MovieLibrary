using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace MovieLibrary.API
{
    public class OMDbAPI
    {
        public async Task<string> Get(string title)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://www.omdbapi.com/?i=tt3896198&apikey=bde015b0&t={title}");
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
