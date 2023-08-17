using Microsoft.AspNetCore.Mvc;
using CoreTweet;

namespace Tweeter_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TweeterController : ControllerBase
    {
        private readonly string ConsumerKey = "2HkFeJ5NGvCcvZhwYgHykIy9O";
        private readonly string ConsumerSecret = "jrOlCuxvzedxICrOLHSwGWFFSA3Xlk8dvA038UM0Cuyar4WVjA";

        [HttpGet("authorize")]
        public IActionResult Authorize()
        {
            var session = OAuth.Authorize(ConsumerKey, ConsumerSecret);

            // Redirect the user to the authorization URL
            var authorizeUrl = session.AuthorizeUri.ToString();
            return Redirect(authorizeUrl);
        }

        [HttpGet("callback")]
        public IActionResult Callback(string oauth_token, string oauth_verifier)
        {
            var session = OAuth.Authorize(ConsumerKey, ConsumerSecret);
            var tokens = session.GetTokens(oauth_verifier);

            return Ok(new { AccessToken = tokens.AccessToken, AccessTokenSecret = tokens.AccessTokenSecret });
        }

        [HttpGet("profile")]
        public IActionResult GetProfile(string accessToken, string accessTokenSecret)
        {
            var tokens = Tokens.Create(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret);

            try
            {
                var user = tokens.Account.VerifyCredentials();
                var userProfile = new 
                {
                    user.Id,
                    user.Name,
                    user.ScreenName,
                    user.ProfileImageUrl,
                    // Add more properties as needed
                };

                return Ok(userProfile);
            }
            catch (TwitterException ex)
            {
                return BadRequest($"Error retrieving profile: {ex.Message}");
            }
        }

        [HttpPost("send-direct-message")]
        public async Task<IActionResult> SendDirectMessage([FromBody] DirectMessageRequestModel model)
        {
            var tokens = Tokens.Create(ConsumerKey, ConsumerSecret, model.AccessToken, model.AccessTokenSecret);

            try
            {
                DirectMessage message = await tokens.DirectMessages.NewAsync(
                    screen_name => model.ScreenName,
                    text => model.Message);

                return Ok($"Direct message sent to {model.ScreenName}");
            }
            catch (TwitterException ex)
            {
                return BadRequest($"Error sending direct message: {ex.Message}");
            }
        }
    }
}
