namespace Tweeter_App
{
    public class DirectMessageRequestModel
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string ScreenName { get; set; }
        public string Message { get; set; }
    }
}
