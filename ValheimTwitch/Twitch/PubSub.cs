using SimpleJSON;
using System;
using System.Timers;
using WebSocketSharp;

namespace ValheimTwitch.Twitch
{
    public class RewardRedeemedArgs : EventArgs
    {
        public string Message { get; set; }
        public JSONNode Data { get; set; }
    }

    public delegate void RewardRedeemedHandler(object sender, RewardRedeemedArgs e);

    /// <summary>
    /// Twitch PubSub client.
    /// </summary>
    public class PubSub
    {
        public Client client;

        public event RewardRedeemedHandler OnRewardRedeemed;
        
        private WebSocket ws;
        private System.Timers.Timer pingTimer;
        private System.Timers.Timer pongTimer;

        private const string pubSubURL = "wss://pubsub-edge.twitch.tv";

        public PubSub(Client client)
        {
            this.client = client;

            pingTimer = new System.Timers.Timer();
            pingTimer.Elapsed += OnPingEvent;

            SetRandomPingInterval();
        }

        public PubSub(Credentials credentials) : this(new Client(credentials)) {}

        public PubSub(string clientId, string accessToken) : this(new Client(clientId, accessToken)) { }

        protected void SetRandomPingInterval()
        {
            pingTimer.Interval = 60000 * GetRandomNumber(3, 4);
        }

        // * Clients must LISTEN on at least one topic within 15 seconds of establishing the connection, or they will be disconnected by the server.
        // * Clients must send a PING command at least once every 5 minutes
        // - If a client does not receive a PONG message within 10 seconds of issuing a PING command,
        // - ...it should reconnect to the server. 
        // - Clients may receive a RECONNECT message at any time.
        //   This indicates that the server is about to restart (typically for maintenance) and will disconnect the client within 30 seconds.
        //   During this time, we recommend that clients reconnect to the server; otherwise, the client will be forcibly disconnected.
        public void Connect()
        {
            if (ws != null)
            {
                throw new Exception("Websocket already connected");
            }

            Log.Info("Connected");

            ws = new WebSocket(pubSubURL);
            
            ws.OnOpen += OnOpen;
            ws.OnClose += OnClose;
            ws.OnMessage += OnMessage;
            ws.OnError += OnError;

            ws.Connect();
        }

        public void Disconnect()
        {
            if (ws == null)
            {
                return;
            }

            Log.Info("Disconnected");

            ws.Close();
            pingTimer.Stop();
            pongTimer.Stop();
        }

        protected double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        protected void OnPingEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (ws == null || !ws.IsAlive)
            {
                return;
            }

            Log.Info("PING");
            ws.Send(@"{""type"": ""PING""}");

            pongTimer = new System.Timers.Timer();
            pongTimer.Elapsed += OnPongTimeoutEvent;
            pongTimer.AutoReset = false;
            pongTimer.Interval = 10000;
            pongTimer.Start();

            SetRandomPingInterval();
        }

        private void OnPongTimeoutEvent(object sender, ElapsedEventArgs e)
        {
            Log.Error("Pong timeout!");
            Disconnect();
            // TODO reconnect
        }

        protected void OnOpen(object sender, EventArgs e)
        {
            Log.Info("OnOpen");

            if (OnRewardRedeemed != null)
            {
                try
                {
                    var user = client.GetUser();
                    var json = JSON.Parse("{}");

                    json["type"] = "LISTEN";
                    json["data"] = JSON.Parse("{}");
                    json["nonce"] = "OnRewardRedeemed";
                    json["data"]["auth_token"] = client.GetUserAcessToken();
                    json["data"]["topics"][0] = $"channel-points-channel-v1.{user.id}";

                    Log.Info(json.ToString());
                    ws.Send(json.ToString());
                    pingTimer.Start();
                } catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        protected void OnClose(object sender, CloseEventArgs e)
        {
            Log.Info("OnClose");
            pingTimer.Stop();
            pongTimer.Stop();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Log.Error(e.Message);
            pingTimer.Stop();
            pongTimer.Stop();
            // TODO reconnect
        }

        protected void OnMessage(object sender, MessageEventArgs e)
        {
            Log.Debug($"OnMessage: {e.Data}");

            var data = JSON.Parse(e.Data);

            if (data["type"] == "PONG")
            {
                pongTimer.Stop();
                return;
            }

            if (data["type"] == "MESSAGE")
            {
                var topic = data["data"]["topic"].Value;
                var message = data["data"]["message"].Value;

                if (topic.StartsWith("channel-points-channel"))
                {
                    var jsonData = JSON.Parse(message);

                    if (jsonData["type"] == "reward-redeemed") 
                    {
                        OnRewardRedeemed?.Invoke(this, new RewardRedeemedArgs { Message = message, Data = jsonData["data"] });
                    }

                }

                return;
            }

            
        }
    }
}
