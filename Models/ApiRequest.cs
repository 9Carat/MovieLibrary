using MovieLibrary.Utility;
using static MovieLibrary.Utility.SD;

namespace MovieLibrary.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string ApiUrl { get; set; }
        public object Data { get; set; }
    }
}
