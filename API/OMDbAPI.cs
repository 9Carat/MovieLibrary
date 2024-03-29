using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MovieLibrary.API
{
    public class OMDbAPI
    {
        public async Task<(string response, bool isSuccessful)> Get(string title)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://www.omdbapi.com/?i=tt3896198&apikey=bde015b0&t={title}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var responseObject = JObject.Parse(responseData);
                    bool isSuccessful = responseObject["Response"].ToString().Equals("True");
                    return (responseData, isSuccessful);
                }
                else return (null, false);
            }
        }
    }
}
