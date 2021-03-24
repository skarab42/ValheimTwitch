namespace ValheimTwitch.Twitch.Auth
{
    public class Credentials
    {
        public string clientId;
        public string accessToken;
        public string refreshToken;

        public Credentials(string clientId, string accessToken = "", string refreshToken = "")
        {
            this.clientId = clientId;
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }
    }
}
