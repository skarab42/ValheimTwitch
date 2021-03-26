using System;

namespace ValheimTwitch.Twitch.API.Helix
{
    public class CustomRewardException : Exception
    {
        public CustomRewardResponse response;

        public CustomRewardException(CustomRewardResponse response) : base(response.Message)
        {
            this.response = response;
        }
    }
}
