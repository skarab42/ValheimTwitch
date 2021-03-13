using BepInEx.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Timers;
using WebSocketSharp;

namespace ValheimTwitch.Twitch.PubSub
{
    public class RewardRedeemedArgs : EventArgs
    {
        public Messages.Redemption Redemption { get; set; }
    }

    public delegate void RewardRedeemedHandler(object sender, RewardRedeemedArgs e);

    /// <summary>
    /// Twitch PubSub client.
    /// </summary>
    public class Client
    {
        public Twitch.Client client;

        public event RewardRedeemedHandler OnRewardRedeemed;
        
        private WebSocket ws;
        private System.Timers.Timer pingTimer;
        private System.Timers.Timer pongTimer;

        private const string pubSubURL = "wss://pubsub-edge.twitch.tv";

        public Client(Twitch.Client client)
        {
            this.client = client;

            pingTimer = new System.Timers.Timer();
            pingTimer.Elapsed += OnPingEvent;

            SetRandomPingInterval();
        }

        public Client(Credentials credentials) : this(new Twitch.Client(credentials)) {}

        public Client(string clientId, string accessToken) : this(new Twitch.Client(clientId, accessToken)) { }

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
                    var topic = $"channel-points-channel-v1.{user.Id}";
                    var topics = new List<string>(new string[] { topic });
                    var message = new Messages.Listen(client.GetUserAcessToken(), topics, topic);
                    var json = JsonConvert.SerializeObject(message);

                    ws.Send(json);
                    pingTimer.Start();
                } 
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    // TODO try to reconnect ?
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

            var iMessage = JsonConvert.DeserializeObject<Messages.IncomingMessage>(e.Data);

            if (iMessage.Type == "PONG")
            {
                pongTimer.Stop();
                return;
            }

            if (iMessage.Type == "MESSAGE")
            {
                var message = iMessage as Messages.Message;

                if (message.Data.Topic.StartsWith("channel-points-channel"))
                {
                    var rewardRedeem = JsonConvert.DeserializeObject<Messages.RewardRedeem>(message.Data.Message);

                    OnRewardRedeemed?.Invoke(this, new RewardRedeemedArgs { Redemption = rewardRedeem.Data.Redemption });
                }

                return;
            }
        }
    }
}
