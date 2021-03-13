using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ValheimTwitch.Twitch.PubSub.Messages
{
    public enum Types
    {
        Unknown,
        Pong,
        Response,
        Message
    }

    public class MessageConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jObject = JToken.ReadFrom(reader);
            Types type = jObject["type"].ToObject<Types>();

            IncomingMessage result;

            switch (type)
            {
                case Types.Pong:
                    result = new Pong();
                    break;
                case Types.Response:
                    result = new Response();
                    break;
                case Types.Message:
                    result = new Message();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializer.Populate(jObject.CreateReader(), result);

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
    public abstract class AbstractMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    [JsonConverter(typeof(MessageConverter))]
    public abstract class IncomingMessage : AbstractMessage {}

    public class Pong : IncomingMessage {}

    public class Response : IncomingMessage
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class Message : IncomingMessage
    {
        [JsonProperty("data")]
        public MessageData Data { get; set; }
    }

    public class MessageData
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class ListenData
    {
        [JsonProperty("topics")]
        public List<string> Topics { get; set; }

        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }

        public ListenData(string authToken, List<string> topics)
        {
            this.Topics = topics;
            this.AuthToken = authToken;
        }
    }

    public class Listen : AbstractMessage
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("data")]
        public ListenData Data { get; set; }

        public Listen(string authToken, List<string> topics, string nonce = "")
        {
            this.Type = "LISTEN";
            this.Nonce = nonce;
            this.Data = new ListenData(authToken, topics);
        }
    }

}