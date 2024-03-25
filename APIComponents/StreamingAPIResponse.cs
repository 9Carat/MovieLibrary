namespace MovieLibrary.APIComponents
{
    public class StreamingAPIResponse
    {
        public List<Result> result {  get; set; }
    }
    public class Result
    {
        public StreamingInfo streamingInfo { get; set; }
    }
    public class StreamingInfo
    {
        public List<StreamOption> us { get; set; }
    }
    public class StreamOption
    {
        public string service { get; set; }
        public string streamingType { get; set; }
        public string link { get; set; }
        public Price price { get; set; }
    }
    public class Price
    {
        public string formatted { get; set; }
    }
}
