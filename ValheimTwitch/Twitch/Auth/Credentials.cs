namespace ValheimTwitch.Twitch.Auth
{
    public class Credentials
    {
        public string clientId;
        public string accessToken;

        public Credentials(string clientId, string accessToken)
        {
            this.clientId = clientId;
            this.accessToken = accessToken;
        }
    }
}
