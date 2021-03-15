using System.Threading.Tasks;
using UnityEngine;
using ValheimTwitch.Twitch.Utils;
using WatsonWebserver;

namespace ValheimTwitch.Twitch.Auth
{
    class Provider
    {
        private static Server server;
        private static string state;

        private static readonly string authorizeURL = "https://id.twitch.tv/oauth2/authorize";
        
        public static void GetCode(string clientId, string redirectURL, string scope)
        {
            state = Generate.RandomString();

            var url = $"{authorizeURL}?client_id={clientId}&redirect_uri={redirectURL}&scope={scope}&state={state}&response_type=code";

            Log.Debug($"Open --> {url}");
            Application.OpenURL(url);

            if (server == null)
            {
                server = new Server(Plugin.TWITCH_REDIRECT_HOST, Plugin.TWITCH_REDIRECT_PORT, false, GetCodeRoute);
                server.Start();
            }
        }

        private static async Task GetCodeRoute(HttpContext context)
        {
            if (context.Request.Query.Elements.ContainsKey("error"))
            {
                var error = context.Request.Query.Elements["error"];
                var description = context.Request.Query.Elements["error_description"];

                context.Response.StatusCode = 401;
                await context.Response.Send($"Error: {error}: {description}");
            }
            else if (context.Request.Query.Elements.ContainsKey("code"))
            {
                var code = context.Request.Query.Elements["code"];
                var responseState = context.Request.Query.Elements["state"];

                if (responseState != state)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.Send($"42");
                }
                else
                {
                    Log.Info($"Code: {code}");

                    context.Response.StatusCode = 200;
                    await context.Response.Send($"Done!");

                    // https://us-central1-valheim-twitch-mod.cloudfunctions.net/getTwitchTokenFromCode

                }
            }

            server.Stop();
            server = null;
        }
    }
}
