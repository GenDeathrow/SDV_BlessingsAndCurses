using BNC.Configs;
using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using static BNC.Spawner;
using BNC.TwitchAppIntergration;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;
using BNC.Actions;

namespace BNC
{
    public class BNC_Core : Mod
    {
        public static SaveFile BNCSave = new SaveFile();
        public static readonly string saveFileName = "bncsave";
        public static IModHelper helper;
        public static IMonitor Logger;
        public static Config config = new Config();
        private static AppIntergration connection;
        public static ActionManager manager;

        public override void Entry(IModHelper helperIn)
        {
            helper = helperIn;
            Logger = this.Monitor;
            config = helper.ReadConfig<Config>();

            if (config.Enable_Twitch_Integration)
                TwitchIntergration.LoadConfig(helperIn);

            helper.Events.Player.Warped += MineBuffManager.mineLevelChanged;

            helper.Events.GameLoop.UpdateTicked += this.updateTick;

            helper.Events.GameLoop.DayStarted += NewDayEvent;

            helper.Events.GameLoop.Saving += BeforeSaveEvent;
            helper.Events.GameLoop.Saved += SaveEvent;

            helper.Events.GameLoop.SaveLoaded += LoadEvent;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;

            manager = new ActionManager();
            connection = new TwitchAppIntergration.AppIntergration("GenDeathrow_Stardew");
            connection.Start();
            //old
            // MineEvents.MineLevelChanged += MineBuffManager.mineLevelChanged;
            /*            BookcaseEvents.GameQuaterSecondTick.Add(QuaterSecondUpdate);
                        BookcaseEvents.GameFullSecondTick.Add(FullSecondTick);
                         TimeEvents.AfterDayStarted += NewDayEvent;
                        SaveEvents.AfterSave += SaveEvent;
                        SaveEvents.AfterLoad += LoadEvent;
                        SaveEvents.BeforeSave += BeforeSaveEvent;
                        SaveEvents.AfterReturnToTitle += OnReturnToTitle; */




            BuffManager.Init();
            MineBuffManager.Init();
            Spawner.Init();

            //Debug button
            //helper.Events.Input.ButtonPressed += this.InputEvents_ButtonPressed;
        }
                       
        Spawner spawner = new Spawner();
        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            this.Monitor.Log(e.Button.ToString());
            if (e.Button.Equals(SButton.B)) {
                this.Monitor.Log("B was pressed", LogLevel.Debug);



                Farmer who = Game1.player;
                for (int i = 0; i < 10; i++)
                {
                    Item bomb = Utility.getItemFromStandardTextDescription($"O 286 1", who);
                    BombEvent.updateQueue.Enqueue(bomb);
                }


                this.Monitor.Log($"list: {BombEvent.updateQueue.Count}", LogLevel.Debug);
            }
        }


        private static Vector2 getRangeFromPlayer(int range, int minRange = 3)
        {
            int xStart = Game1.player.getTileX() - range;
            int yStart = Game1.player.getTileY() - range;

            int randX = Game1.random.Next(range * 2 + 2);
            int randY = Game1.random.Next(range * 2 + 2);


            Vector2 vector = new Vector2(xStart + randX, yStart + randY);
            while (Vector2.Distance(vector, Game1.player.getTileLocation()) < minRange)
            {
                vector.X = xStart + Game1.random.Next(range * 2 + 2);
                vector.Y = yStart + Game1.random.Next(range * 2 + 2);
            }
            return vector;
        }

        private void OnReturnToTitle(object sender, EventArgs e)
        {
            BNCSave.clearData();
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            BNCSave.LoadModData(helper);
        }

        private void BeforeSaveEvent(object sender, EventArgs e)
        {
            Spawner.ClearMobs();
        }

        private void SaveEvent(object sender, EventArgs e) {
            BNCSave.SaveModData(helper);
        }

        private void NewDayEvent(object sender, EventArgs e) {
            if (!Context.IsWorldReady)
                return;
            if (BNC_Core.config.Random_Day_Buffs)
            {
                BuffManager.UpdateDay();
            }

            //Allow Weather to change again. 
            Weather.clearForNewDay();
        }



        private void updateTick(object sender, UpdateTickedEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            if (e.IsMultipleOf(15))
            {
                BuffManager.UpdateTick();
                Spawner.UpdateTick();
                manager.Update();
                BombEvent.UpdateTick();
            }
            else if (e.IsMultipleOf(60))
            {
                MineBuffManager.UpdateTick();
            }
        }
    }
}
