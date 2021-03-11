using SimpleJSON;

namespace ValheimTwitch.Twitch.Helix
{
    public class User
    {
        /// <summary>User’s broadcaster type: "partner", "affiliate", or "".</summary>
        public string broadcasterType;

        /// <summary>User’s channel description.</summary>
        public string description;

        /// <summary>User’s display name.</summary>
        public string displayName;

        /// <summary>User’s ID.</summary>
        public string id;

        /// <summary>User’s login name.</summary>
        public string login;

        /// <summary>URL of the user’s offline image.</summary>
        public string offlineImageURL;

        /// <summary>URL of the user’s profile image.</summary>
        public string profileImageURL;

        /// <summary>User’s type: "staff", "admin", "global_mod", or "".</summary>
        public string type;

        /// <summary>Total number of views of the user’s channel.</summary>
        public int viewCount;

        /// <summary>User’s verified email address. Returned if the request includes the user:read:email scope.</summary>
        public string email;

        /// <summary>Date when the user was created.</summary>
        public string createdAt;

        public static User Factory(JSONNode node)
        {
            return new User
            {
                broadcasterType = node["broadcaster_type"].Value,
                description = node["description"].Value,
                displayName = node["display_name"].Value,
                id = node["id"].Value,
                login = node["login"].Value,
                offlineImageURL = node["offline_image_url"].Value,
                profileImageURL = node["profile_image_url"].Value,
                type = node["type"].Value,
                viewCount = node["view_count"].AsInt,
                email = node["email"].Value,
                createdAt = node["created_at"].Value
            };
        }
    }
}
