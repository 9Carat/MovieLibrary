using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace MovieLibrary.API
{
    public class YouTubeAPI
    {
        private readonly YouTubeService _youTubeService;

        public YouTubeAPI()
        {
            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDaUEKEiZKOIo-i36f1qOyF6TykhZ4o2WU",
                ApplicationName = "MovieLibrary"
            });
        }

        public string FetchYouTubeTrailer(string movieTitle)
        {
            // Construct the request to search for trailers on YouTube
            var searchListRequest = _youTubeService.Search.List("snippet");
            searchListRequest.Q = movieTitle + " trailer";
            searchListRequest.Type = "video";
            searchListRequest.MaxResults = 1;

            // Execute the request
            var searchListResponse = searchListRequest.Execute();

            // Parse the response
            if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
            {
                // Extract the video ID of the first search result
                var videoId = searchListResponse.Items[0].Id.VideoId;
                // Construct the YouTube video URL
                var youTubeUrl = "https://www.youtube.com/watch?v=" + videoId;
                return youTubeUrl;
            }
            else
            {
                return null;
            }
        }
    }
}
