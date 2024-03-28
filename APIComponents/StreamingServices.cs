using MovieLibrary.Models;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieLibrary.APIComponents
{
    public class StreamingServices
    {
        public static List<StreamingService> DeserializeJSON(string jsonString)
        {
            // Deserialize response as custom response object
            StreamingAPIResponse response = JsonConvert.DeserializeObject<StreamingAPIResponse>(jsonString);

            if (response != null)
            {
                Result firstResult = response.result[0];

                List<StreamOption> streamOptions = firstResult.streamingInfo.se;

                if (streamOptions != null)
                {
                    var streamingServices = new List<StreamingService>();

                    foreach (var service in streamOptions)
                    {
                        // Create StreamingService object based on custom respone object
                        var streamingService = new StreamingService
                        {
                            Id = Guid.NewGuid(),
                            ServiceName = service.service,
                            Type = service.streamingType,
                            Link = service.link,
                        };
                        if (service.price != null)
                        {
                            streamingService.Price = Convert.ToString(service.price.formatted);
                        }
                        streamingServices.Add(streamingService);
                    }
                    return streamingServices;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}

