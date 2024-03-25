using MovieLibrary.Models;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieLibrary.APIComponents
{
    public class StreamingServices
    {
        public static List<StreamingService> FromJson(string jsonString)
        {
            StreamingAPIResponse response = JsonConvert.DeserializeObject<StreamingAPIResponse>(jsonString);

            Result firstResult = response.result[0];

            List<StreamOption> streamOptions = firstResult.streamingInfo.us;

            var streamingServices = new List<StreamingService>();

            foreach (var service in streamOptions)
            {
                var streamingService = new StreamingService
                {
                    Id = Guid.NewGuid(),
                    ServiceName = service.service,
                    Type = service.streamingType,
                    Link = service.link,
                    //Fk_MovieId = movieId

                };
                if (service.price != null )
                {
                    streamingService.Price = Convert.ToString(service.price.formatted);
                }
                streamingServices.Add(streamingService);
            }

            return streamingServices;


            //var services = response["result"] as List<object>;

            // Select the first entry in the result array
            //var streamingInfo = services?.FirstOrDefault() as Dictionary<string, object>;
            //var streamingServices = new List<StreamingService>();

            //// Check the streamingInfo prop for relevant info
            //if (streamingInfo != null && streamingInfo.ContainsKey("streamingInfo"))
            //{
            //    var streamingRegion = streamingInfo["streamingInfo"] as Dictionary<string, object>;
            //    var servicesRegion = streamingRegion?["us"] as List<object>;

            //    if (servicesRegion != null)
            //    {
            //        foreach (var service in servicesRegion)
            //        {
            //            var serviceData = service as Dictionary<string, object>;

            //            if (serviceData != null)
            //            {
            //                // Add general info about streaming availabilty
            //                var streamingService = new StreamingService
            //                {
            //                    Id = Guid.NewGuid(),
            //                    Fk_MovieId = movieId,
            //                    ServiceName = Convert.ToString(serviceData["service"]),
            //                    Type = Convert.ToString(serviceData["streamingType"]),
            //                    Link = Convert.ToString(serviceData["link"])
            //                };

            //                // Add price if the streaming type is buy or rent
            //                if (serviceData.ContainsKey("price"))
            //                {
            //                    var priceData = serviceData["price"] as Dictionary<string, object>;
            //                    streamingService.Price = Convert.ToString(priceData["formatted"]);
            //                }

            //                streamingServices.Add(streamingService);
            //            }
            //        }
            //    }
            //}

            //Console.WriteLine(streamingServices.FirstOrDefault());

            //// Return the first service
            //return streamingServices.FirstOrDefault();
        }
    }
}

