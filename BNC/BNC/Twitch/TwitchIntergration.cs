using System;
using System.Collections.Generic;
using System.Timers;
using StardewValley;
using StardewModdingAPI;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using BNC.Configs;
using static BNC.Spawner;

namespace BNC
{
    class TwitchIntergration
    {
        private static TwitchClient client;
        private static ConnectionCredentials credentials;
        private static String username;
        private static String token;
        private static String channel;

        public static bool hasPollStarted;
        public static Timer timer = new Timer();

        public static List<String> usersVoted = new List<string>();

        public static bool hasMinePollStarted;


        public static Dictionary<BuffManager.BuffOption, int> votes;
        public static Dictionary<MineBuffManager.AugmentOption, int> augvotes;

        
        private static int currentTick = 30;

        private static List<String> currentDisplayText = new List<string>();

        private static Spawner spawner = new Spawner();

        private static readonly string fileName = "twitch_secret.json";


        public static void LoadConfig(IModHelper helper)
        {
            TwitchSecret twitchSettings = new TwitchSecret();

            if (helper.Data.ReadJsonFile<TwitchSecret>(fileName) == null)
                helper.Data.WriteJsonFile<TwitchSecret>(fileName, twitchSettings);
            else
                twitchSettings = helper.Data.ReadJsonFile<TwitchSecret>(fileName);

            username = twitchSettings.Twitch_User_Name;
            token = twitchSettings.OAuth_Token;
            channel = twitchSettings.Twitch_Channel_Name;
            TwitchInit(helper);
        }


        public static void TwitchInit(IModHelper helper)
        {
            if (username == null || token == null || channel == null)
            {
                BNC_Core.Logger.Log($"Tried to initialize twitch but failed. username:{username == null} || token:{token == null} || channel:{channel == null}");
                return;
            }
            credentials = new ConnectionCredentials(username, token);
            client = new TwitchClient();
            client.Initialize(credentials, channel);
            client.OnConnected += OnConnected;
            client.OnDisconnected += OnDisconnect;
            client.OnMessageReceived += OnMessageReceived;
            client.OnNewSubscriber += OnNewSubscriber;
            client.OnConnectionError += OnConnectionError;
            client.OnError += OnError;
            client.OnGiftedSubscription += OnGifted;
            client.OnReSubscriber += OnReSubscriber;
            GraphicsEvents.OnPostRenderHudEvent += new EventHandler(GraphicsEvents_OnPostRenderHudEvent);
            timer.Interval = 1000;
            timer.Elapsed += onTimertick;
            Connet();
        }


        private static void OnError(object sender, OnErrorEventArgs e)
        {
            BNC_Core.Logger.Log($"Twitch Integration Error {e.Exception.Message}");
        }

        private static void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            BNC_Core.Logger.Log($"Twitch Integration Connection Error {e.Error.Message}");
        }

        private static void onTimertick(object sender, ElapsedEventArgs e)
        {
            if (currentTick-- <= 0)
                if (hasPollStarted)
                    EndBuffPoll();
                else if (hasMinePollStarted)
                    EndMinePoll();
        }

        private static void UpdateVotingText()
        {
            currentDisplayText.Clear();

            if (hasPollStarted)
            {
                int maximum = 0;
                foreach (int count in votes.Values)
                    maximum += count;

                foreach (KeyValuePair<BuffManager.BuffOption, int> vote in votes)
                {
                    int perc = (vote.Value == 0 || maximum == 0) ? 0 : Convert.ToInt32(((double)vote.Value / maximum) * 100);
                    currentDisplayText.Add($"{vote.Key.displayName} [{vote.Key.id}] ({perc}%) -- {vote.Key.shortdesc}");
                }
            }else if (hasMinePollStarted)
            {
                int maximum = 0;
                foreach (int count in augvotes.Values)
                    maximum += count;
                foreach (KeyValuePair<MineBuffManager.AugmentOption, int> vote in augvotes)
                {
                    int perc = (vote.Value == 0 || maximum == 0) ? 0 : Convert.ToInt32(((double)vote.Value / maximum) * 100);
                    currentDisplayText.Add($"{vote.Key.DisplayName} [{vote.Key.id}] ({perc}%) -- {vote.Key.desc}");
                }
            }
        }

        public static bool isConnected()
        {
            if (client == null)
                return false;

            return client.IsConnected;
        }

        static int reconnectTrys = 0;
        public static void attemptReconnect(string msg = null)
        {
            if (reconnectTrys >= 4)
                return;
            BNC_Core.Logger.Log($"Attempting to reconnect to the channel {channel}...");

            if (reconnectTrys++ < 4)
            {
                if (client != null)
                {
                    Connet();

                    if (msg != null) sendMessage(msg);
                }
                else
                    BNC_Core.Logger.Log($"Attempted to connect to {channel}, But failed");
            }
        }

        private static bool sendMessage(string msg)
        {
            try
            {
                //client.SendMessage(channel, msg);
                return true;
            }
            catch(Exception e)
            {
                attemptReconnect();
                BNC_Core.Logger.Log("Faild to send message to Twitch! >>> "+ e);
                return false;
            }
        }

        public static void StartBuffPoll(BuffManager.BuffOption[] buffs)
        {

            if(Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has started...");

           if (!sendMessage($"---> TIME TO CHOOSE A BLESSING/CURSE!"))
               return;

            votes = new Dictionary<BuffManager.BuffOption, int>();
            string buildMsg =  "[";
            foreach (BuffManager.BuffOption buff in buffs)
            {
                buildMsg += $"{buff.id} {(buffs.GetValue(buffs.Length-1) == buff ? "" : " / ")}";
                votes.Add(buff, 0);
            }
            buildMsg += "]";

            sendMessage(buildMsg);

            timer.Start();
            hasPollStarted = true;
            usersVoted.Clear();
            UpdateVotingText();
        }

        public static void EndBuffPoll()
        {
            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has ended!");
            currentTick = Config.getVotingTime();
            timer.Stop();
            hasPollStarted = false;

            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Time to tally the votes!");

            BuffManager.BuffOption selectedId = null;
            int votecount = -1;
            foreach(KeyValuePair<BuffManager.BuffOption, int> vote in votes)
            {
                if(vote.Value > votecount)
                {
                   votecount = vote.Value;
                   selectedId = vote.Key;
                }
            }

            if (selectedId != null)
            {
                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Chat has selected {selectedId.displayName} : vote# {votecount}");

                BuffManager.AddBuffToQueue(selectedId);

                sendMessage($"Chat has spoken! Selected {selectedId.displayName}!");

                if(selectedId.hudMessage != null)
                    Game1.addHUDMessage(new HUDMessage(selectedId.hudMessage, null));
            }
            else if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Error: {selectedId} Buff was null");

            votes.Clear();
        }

        public static void StartMinePoll(MineBuffManager.AugmentOption[] augments)
        {
            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has started...");

            sendMessage($"---> TIME TO CHOOSE A COMBAT AUGMENT!");

            augvotes = new Dictionary<MineBuffManager.AugmentOption, int>();
            string buildMsg = "[";
            foreach (MineBuffManager.AugmentOption aug in augments)
            {
                buildMsg += $"{aug.id} {(augments[augments.Length - 1] == aug ? "" : " / ")}";
                augvotes.Add(aug, 0);
            }
            buildMsg += "]";

            sendMessage(buildMsg);

            timer.Start();
            hasMinePollStarted = true;
            usersVoted.Clear();
            UpdateVotingText();
        }

        public static void EndMinePoll()
        {
            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Poll has ended!");
            currentTick = Config.getVotingTime();
            timer.Stop();
            hasMinePollStarted = false;

            if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Time to tally the votes!");

            MineBuffManager.AugmentOption selectedId = null;
            int votecount = -1;
            foreach (KeyValuePair<MineBuffManager.AugmentOption, int> vote in augvotes)
            {
                if (vote.Value > votecount)
                {
                    votecount = vote.Value;
                    selectedId = vote.Key;
                }
            }

            if (selectedId != null)
            {
                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"Chat has selected {selectedId.DisplayName} : vote# {votecount}");

                MineBuffManager.CurrentAugment = selectedId.id;

                sendMessage($"Chat has spoken! Selected {selectedId.DisplayName}!");

                if (selectedId.DisplayName != null)
                    Game1.addHUDMessage(new HUDMessage(selectedId.DisplayName, null));
            }
            else if (Config.ShowDebug())
                BNC_Core.Logger.Log($"Error: {selectedId} Buff was null");

            augvotes.Clear();
        }


        public static void Connet()
        {
            BNC_Core.Logger.Log($"{username} Connecting to {channel}");
            try
            {
                client.Connect();
            }
            catch (Exception e) { BNC_Core.Logger.Log($"Attempted to connect to {channel}, But failed {e}"); }
        }

        public static void Disconnect()
        {
            BNC_Core.Logger.Log($"{username} Disconnecting to {channel}");
            client.Disconnect();
        }

        private static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Spawner.SpawnTwitchJunimo(e.Subscriber.DisplayName);


            if (Config.ShowDebug())
                BNC_Core.Logger.Log("Attemting to spawn Junimo from resub");
        }


        private static void OnGifted(object sender, OnGiftedSubscriptionArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Spawner.AddMonsterToSpawnFromType(TwitchMobType.Slime, e.GiftedSubscription.MsgParamRecipientUserName);

            if (Config.ShowDebug())
                BNC_Core.Logger.Log("Attemting to spawn Slime from giftsub");
        }


        private static void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Spawner.SpawnTwitchJunimo(e.ReSubscriber.DisplayName);

            if (Config.ShowDebug())
                BNC_Core.Logger.Log("Attemting to spawn Junimo from resub");
        }

        private static int BitCount = 0;
        private static List<String> mobNames = new List<String>();

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (hasMinePollStarted && !usersVoted.Contains(e.ChatMessage.UserId))
            {
                List<MineBuffManager.AugmentOption> keysvalue = new List<MineBuffManager.AugmentOption>(augvotes.Keys);
                foreach (MineBuffManager.AugmentOption vote in keysvalue)
                {
                    if (vote.id.ToLower().Equals(e.ChatMessage.Message.Trim().ToLower()))
                    {
                        augvotes[vote]++; 
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"Recieved Vote for {vote.id} : #{augvotes[vote]}");
                        usersVoted.Add(e.ChatMessage.UserId);
                        UpdateVotingText();
                    }
                }
            }
            else if (hasPollStarted && !usersVoted.Contains(e.ChatMessage.UserId))
            {
                List<BuffManager.BuffOption> keysvalue = new List<BuffManager.BuffOption>(votes.Keys);
                foreach (BuffManager.BuffOption vote in keysvalue)
                {
                    if (vote.id.ToLower().Equals(e.ChatMessage.Message.Trim().ToLower()))
                    {
                        votes[vote]++;
                        if (Config.ShowDebug())
                            BNC_Core.Logger.Log($"Recieved Vote for {vote.id} : #{votes[vote]}");
                        usersVoted.Add(e.ChatMessage.UserId);
                        UpdateVotingText();
                    }
                }
            }
            else if (BNC_Core.config.Use_Bits_To_Spawn_Mobs && e.ChatMessage.Bits > 0)
            {
                BitCount += e.ChatMessage.Bits;


                TwitchMobType? type = (TwitchMobType)Spawner.GetMonsterFromBits(e.ChatMessage.Bits);

                if(type != null)
                {
                    if (Config.ShowDebug())
                        BNC_Core.Logger.Log($"Trying to spawn {type.ToString()} with {e.ChatMessage.Bits}bits");
                    Spawner.AddMonsterToSpawnFromType((TwitchMobType)type, e.ChatMessage.Username);
                }
                else if(Config.ShowDebug())
                    BNC_Core.Logger.Log($"Failed to spawn monster with {e.ChatMessage.Bits}bits");

                /*
                if (e.ChatMessage.Bits >= BNC_Core.config.Bits_To_Spawn_Big_Slimes)
                {
                    if (Config.ShowDebug())
                        BNC_Core.Logger.Log($"Trying to spawn large slime with {e.ChatMessage.Bits}");

                    Spawner.SpawnTwitchSlime(e.ChatMessage.Username, true);
                }
                else if (e.ChatMessage.Bits >= BNC_Core.config.Bits_To_Spawn_Slimes)
                {
                    int amount = Math.Max(1, ((int)e.ChatMessage.Bits / BNC_Core.config.Bits_To_Spawn_Slimes));
                    if (Config.ShowDebug())
                        BNC_Core.Logger.Log($"Trying to spawn slime with {e.ChatMessage.Bits}");
                    Spawner.SpawnTwitchSlime(e.ChatMessage.Username, true);
                }
                */
            }
        }

        private static void OnConnected(object sender, OnConnectedArgs e)
        {
            BNC_Core.Logger.Log($"{username} (BNC Bot) has connected to Twitch channel : {channel}");

            if(Config.ShowDebug())
                sendMessage("Blessings and Curses initialized a connection to Twitch!!!");
        }



        private static void OnDisconnect(object sender, OnDisconnectedEventArgs e)
        {
            BNC_Core.Logger.Log($"{username} has disconnected from Twitch channel : {channel}");
            attemptReconnect();
        }


        private static void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || (!hasPollStarted && !hasMinePollStarted) || currentDisplayText.Count < 1 &&  Game1.CurrentEvent == null)
                return;

            int num1 = 64;
            SpriteFont smallFont = Game1.smallFont;
            SpriteBatch spriteBatch = Game1.spriteBatch;

            Vector2 vector2 = smallFont.MeasureString("");
            foreach (String str in currentDisplayText)
            {
                Vector2 temp = smallFont.MeasureString(str);
                if (temp.X > vector2.X)
                    vector2 = temp;
            }

            int num2 = num1 / 2;
            int width = (int)((double)vector2.X + (double)num2) + 65;
            int height = Math.Max(60, 60 + 35 * (currentDisplayText.Count - 1));
            int x = 0;
            int y = 0;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += num2;
            }
            if (y + height > Game1.viewport.Height)
            {
                x += num2;
                y = Game1.viewport.Height - height;
            }
            int cnt = 0;
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height+ 35, Color.White, 1f, true);
            Utility.drawTextWithShadow(spriteBatch, "Vote: ", smallFont, new Vector2((float)(x + num1 / 4), (float)(y + num1 / 4) + (cnt++ * vector2.Y)), Color.Purple, 1f, -1f, -1, -1, 1f, 3);
            foreach (String str in currentDisplayText)
            {
                Utility.drawTextWithShadow(spriteBatch, str, smallFont, new Vector2((float)(x + num1 / 4) + 5, (float)(y + num1 / 4) + (cnt * vector2.Y)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                cnt++;
            }
            Utility.drawTextWithShadow(spriteBatch, $"{currentTick}s", smallFont, new Vector2((x + width) - smallFont.MeasureString(currentTick + "s").Length() - 4, height/2 - 4), Color.Blue, 1f, -1f, -1, -1, 1f, 3);
        }

    }
}
