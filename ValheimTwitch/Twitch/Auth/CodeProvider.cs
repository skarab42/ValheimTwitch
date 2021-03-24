using System;
using System.Threading.Tasks;
using UnityEngine;
using ValheimTwitch.Twitch.Utils;
using WatsonWebserver;

namespace ValheimTwitch.Twitch.Auth
{ 
    public class CodeArgs : EventArgs
    {
        public string Code { get; set; }
    }

    public class CodeErrorArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public delegate void CodeHandler(object sender, CodeArgs e);
    public delegate void CodeErrorHandler(object sender, CodeErrorArgs e);

    public class CodeProvider
    {
        public event CodeHandler OnCode;
        public event CodeErrorHandler OnError;

        private const string AUTH_URL = "https://id.twitch.tv/oauth2/authorize";

        private const string HTML_TEMPLATE = "<html><head><title>{0}</title></head><body>{1}</body></html>";
        private const string SUCCESS_MESSAGE = "<p><strong>✔ Authentication successful!</strong></p><p>You can close this page.</p>";
        private const string DENIED_MESSAGE = "<p><strong>❌ Authentication denied!</strong></p><p>{0}</p><p>You can close this page.</p>";

        private readonly string[] scopes;
        private readonly string clientId;
        private readonly string redirectHost;
        private readonly int redirectPort;

        private string state;
        private Server server;

        public CodeProvider(string clientId, string redirectHost, int redirectPort, string[] scopes)
        {
            this.scopes = scopes;
            this.clientId = clientId;
            this.redirectHost = redirectHost;
            this.redirectPort = redirectPort;
        }

        public void GetCode()
        {
            state = Generate.RandomString();

            var scope = string.Join(" ", scopes);
            var redirectURL = $"http://{redirectHost}:{redirectPort}";
            var url = $"{AUTH_URL}?client_id={clientId}&redirect_uri={redirectURL}&scope={scope}&state={state}&response_type=code";

            Application.OpenURL(url);

            if (server == null)
            {
                server = new Server(redirectHost, redirectPort, false, GetCodeRoute);
                server.Start();
            }
        }

        private static Task<bool> SendHTML(HttpContext context, string title, string body, int code)
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = "text/html; charset=utf-8";
            return context.Response.Send(String.Format(HTML_TEMPLATE, title, body));
        }

        private async Task GetCodeRoute(HttpContext context)
        {
            try
            {
                if (context.Request.Query.Elements.ContainsKey("error"))
                {
                    var message = context.Request.Query.Elements["error_description"].Replace("+", " ");
                    OnError?.Invoke(this, new CodeErrorArgs { Message = message });
                    await SendHTML(context, "Error", String.Format(DENIED_MESSAGE, $"<i>{message}.</i>"), 401);
                }
                else if (context.Request.Query.Elements.ContainsKey("code"))
                {
                    var code = context.Request.Query.Elements["code"];
                    var responseState = context.Request.Query.Elements["state"];

                    if (responseState != state)
                    {
                        var message = "Invalid auth state 🤡";
                        OnError?.Invoke(this, new CodeErrorArgs { Message = message });
                        await SendHTML(context, "Error", String.Format(DENIED_MESSAGE, message), 401);
                    }
                    else
                    {
                        OnCode?.Invoke(this, new CodeArgs { Code = code });
                        await SendHTML(context, "Success", SUCCESS_MESSAGE, 200);
                    }
                }

                server.Stop();
                server.Dispose();
                server = null;
            }
            catch (Exception ex)
            {
                Log.Error("HTTP ERROR -> " + ex.ToString());
            }
        }
    }
}
