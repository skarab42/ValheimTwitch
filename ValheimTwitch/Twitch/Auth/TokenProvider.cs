using Newtonsoft.Json;
using System;
using System.Net;

namespace ValheimTwitch.Twitch.Auth
{
    public class TokenArgs : EventArgs
    {
        public Token Token { get; set; }
        public string Error { get; set; }
    }

    public class TokenErrorArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public delegate void TokenHandler(object sender, TokenArgs e);
    public delegate void TokenErrorHandler(object sender, TokenErrorArgs e);

    public class TokenProvider
    {
        public event TokenHandler OnToken;
        public event TokenErrorHandler OnError;

        private readonly CodeProvider provider;

        private const string GET_TOKEN_URL = "https://us-central1-valheim-twitch-mod.cloudfunctions.net/getTwitchTokenFromCode";
        //private const string REFRESH_TOKEN_URL = "https://us-central1-valheim-twitch-mod.cloudfunctions.net/refreshTwitchToken";

        public TokenProvider(CodeProvider provider)
        {
            this.provider = provider;
            this.provider.OnCode += OnCode;
        }

        public TokenProvider(string clientId, string redirectHost, int redirectPort, string[] scopes)
        {
            provider = new CodeProvider(clientId, redirectHost, redirectPort, scopes);
            provider.OnCode += OnCode;
        }

        private void OnCode(object sender, CodeArgs e)
        {
            using (WebClient client = new WebClient())
            {
                var url = $"{GET_TOKEN_URL}?code={e.Code}";
                var json = client.DownloadString(url);

                var aResponse = JsonConvert.DeserializeObject<AbstractResponse>(json);

                if (aResponse is Response response)
                {
                    var message = $"Error {response.Status}: {response.Message}";

                    OnError?.Invoke(this, new TokenErrorArgs { Message = message });
                }
                else
                {
                    var token = aResponse as Token;

                    OnToken?.Invoke(this, new TokenArgs { Token = token });
                }
            }
        }

        public void GetToken()
        {
            provider.GetCode();
        }
    }
}
