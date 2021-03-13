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
    public class MaxReconnectErrorArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public delegate void RewardRedeemedHandler(object sender, RewardRedeemedArgs e);
    public delegate void MaxReconnectErrorHandler(object sender, MaxReconnectErrorArgs e);

    /// <summary>
    /// Twitch PubSub client.
    /// </summary>
    public class Client
    {
        public API.Client client;

        public event RewardRedeemedHandler OnRewardRedeemed;
        public event MaxReconnectErrorHandler OnMaxReconnect;

        private WebSocket WS;

        private readonly System.Timers.Timer PingTimer;
        private readonly System.Timers.Timer PongTimer;

        private const string PubSubURL = "wss://pubsub-edge.twitch.tv";
        public const int MaxReconnection = 9; // ~2 min.

        private int ReconnectionCount = 0;
        private int ReconnectionInterval = 1000;

        public Client(API.Client client)
        {
            this.client = client;

            PingTimer = new System.Timers.Timer();
            PingTimer.Elapsed += OnPingEvent;

            PongTimer = new System.Timers.Timer();
            PongTimer.Elapsed += OnPongTimeoutEvent;
            PongTimer.AutoReset = false;
            PongTimer.Interval = 10000;

            SetRandomPingInterval();
        }

        public Client(Credentials credentials) : this(new API.Client(credentials)) {}

        public Client(string clientId, string accessToken) : this(new API.Client(clientId, accessToken)) { }

        protected void SetRandomPingInterval()
        {
            PingTimer.Interval = 60000 * GetRandomNumber(3, 4);
        }

        // * Clients must LISTEN on at least one topic within 15 seconds of establishing the connection, or they will be disconnected by the server.
        // * Clients must send a PING command at least once every 5 minutes
        // * If a client does not receive a PONG message within 10 seconds of issuing a PING command, it should reconnect to the server. 
        // * Clients may receive a RECONNECT message at any time.
        //   This indicates that the server is about to restart (typically for maintenance) and will disconnect the client within 30 seconds.
        //   During this time, we recommend that clients reconnect to the server; otherwise, the client will be forcibly disconnected.
        public void Connect()
        {
            Log.Debug("Connect");

            if (WS != null)
            {
                return;
            }

            try
            {
                WS = new WebSocket(PubSubURL);

                WS.OnOpen += OnOpen;
                WS.OnClose += OnClose;
                WS.OnMessage += OnMessage;
                WS.OnError += OnError;

                WS.Connect();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

                Reconnect();
            }
        }

        public void Disconnect()
        {
            Log.Debug("Disconnect");

            if (WS == null)
            {
                return;
            }

            PingTimer.Stop();
            PongTimer.Stop();

            WS.Close();

            WS = null;
        }

        public void Reconnect()
        {
            ReconnectionCount++;

            if (ReconnectionCount > MaxReconnection)
            {
                var message = $"Maximum number of reconnections reached {ReconnectionCount}/{MaxReconnection}";

                OnMaxReconnect?.Invoke(this, new MaxReconnectErrorArgs { Message = message });

                return;
            }

            Log.Info($"Reconnect {ReconnectionCount}/{MaxReconnection} (backoff: {ReconnectionInterval / 1000} sec.)");

            Disconnect();

            var timer = new System.Timers.Timer();

            timer.Elapsed += (object source, ElapsedEventArgs e) => Connect();
            timer.Interval = ReconnectionInterval;
            timer.AutoReset = false;

            timer.Start();

            ReconnectionInterval *= 2;
        }

        protected double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        protected void OnPingEvent(object source, ElapsedEventArgs e)
        {
            if (WS == null || !WS.IsAlive)
            {
                return;
            }

            Log.Debug("PING");
            WS.Send(@"{""type"": ""PING""}");

            PongTimer.Start();

            SetRandomPingInterval();
        }

        private void OnPongTimeoutEvent(object sender, ElapsedEventArgs e)
        {
            Log.Error("Pong timeout!");

            Reconnect();
        }

        protected void OnOpen(object sender, EventArgs e)
        {
            Log.Debug("OnOpen");

            ReconnectionCount = 0;
            ReconnectionInterval = 1000;

            if (OnRewardRedeemed != null)
            {
                try
                {
                    var user = client.GetUser();
                    var topic = $"channel-points-channel-v1.{user.Id}";
                    var topics = new List<string>(new string[] { topic });
                    var message = new Messages.Listen(client.GetUserAcessToken(), topics, topic);
                    var json = JsonConvert.SerializeObject(message);

                    WS.Send(json);
                    PingTimer.Start();
                } 
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());

                    Reconnect();
                }
            }
        }

        protected void OnClose(object sender, CloseEventArgs e)
        {
            Log.Debug("OnClose");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Log.Error(e.Message);

            Reconnect();
        }

        protected void OnMessage(object sender, MessageEventArgs e)
        {
            Log.Debug($"OnMessage: {e.Data}");

            var iMessage = JsonConvert.DeserializeObject<Messages.IncomingMessage>(e.Data);

            if (iMessage.Type == "PONG")
            {
                PongTimer.Stop();

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

            if (iMessage.Type == "RECONNECT")
            {
                Reconnect();

                return;
            }
        }
    }
}
