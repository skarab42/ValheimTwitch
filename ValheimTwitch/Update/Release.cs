using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace ValheimTwitch.Update
{
    class Release
    {
        public static bool HasNewVersion()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"User-Agent: {Plugin.NAME}");

                    var json = client.DownloadString(Plugin.GITHUB_RELEASE_URL);
                    var releases = JsonConvert.DeserializeObject<List<ReleasePayload>>(json);
                    var lastRelease = releases[0].Name.TrimStart('v');

                    if (System.Version.TryParse(lastRelease, out System.Version newVersion))
                    {
                        if (System.Version.TryParse(Plugin.VERSION, out System.Version currentVersion))
                        {
                            if (currentVersion < newVersion)
                            {
                                Log.Info($"A new version is available -> {newVersion}");
                                return true;
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;

                    Log.Warning($"HasNewVersion Error: {response.StatusCode} - {ex}");
                }

                return false;
            }
        }
    }
}
