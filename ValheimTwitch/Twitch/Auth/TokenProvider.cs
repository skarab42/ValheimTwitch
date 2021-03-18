using Newtonsoft.Json;
using System;
using System.Net;
using ValheimTwitch.Twitch.API;

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
        private const string REFRESH_TOKEN_URL = "https://us-central1-valheim-twitch-mod.cloudfunctions.net/refreshTwitchToken";

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

        private Token RequestToken(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var json = client.DownloadString(url);
                    var aResponse = JsonConvert.DeserializeObject<AbstractResponse>(json);

                    if (aResponse is Response response)
                    {
                        var message = $"Error {response.Status}: {response.Message}";
                        OnError?.Invoke(this, new TokenErrorArgs { Message = message });
                        return null;
                    }
                    else
                    {
                        var token = aResponse as Token;
                        OnToken?.Invoke(this, new TokenArgs { Token = token });
                        return token;
                    }
                }
                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    OnError?.Invoke(this, new TokenErrorArgs { Message = response.StatusCode.ToString() });
                    return null;
                }
            }
        }

        private Token RequestTokenFromCode(string code)
        {
            return RequestToken($"{GET_TOKEN_URL}?code={code}");
        }

        private Token RequestRefreshToken(string refreshToken)
        {
            return RequestToken($"{REFRESH_TOKEN_URL}?refreshToken={refreshToken}");
        }

        private void OnCode(object sender, CodeArgs e)
        {
            RequestTokenFromCode(e.Code);
        }

        public void GetToken()
        {
            provider.GetCode();
        }

        public Token RefreshToken(Client client)
        {
            return RequestRefreshToken(client.credentials.refreshToken);
        }
    }
}
