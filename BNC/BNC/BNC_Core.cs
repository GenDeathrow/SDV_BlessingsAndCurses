using Bookcase.Events;
using BNC.Configs;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace BNC
{
    public class BNC_Core : Mod
    {
        public static SaveFile BNCSave = new SaveFile();
        public static readonly string saveFileName = "bncsave";
        public static IModHelper helper;
        public static IMonitor Logger;
        public static Config config = new Config();

        public static bool DebugMode = false;

        public override void Entry(IModHelper helperIn)
        {
            helper = helperIn;
            Logger = this.Monitor;
            config = helper.ReadConfig<Config>();
            TwitchIntergration.LoadConfig(helperIn);

            MineEvents.MineLevelChanged += MineBuffManager.mineLevelChanged;

            BookcaseEvents.GameQuaterSecondTick.Add(QuaterSecondUpdate);
            BookcaseEvents.GameFullSecondTick.Add(FullSecondTick);
            TimeEvents.AfterDayStarted += NewDayEvent;
            SaveEvents.AfterSave += SaveEvent;
            SaveEvents.AfterLoad += LoadEvent;
            SaveEvents.BeforeSave += BeforeSaveEvent;
            SaveEvents.AfterReturnToTitle += OnReturnToTitle;
            SaveEvents.AfterCreate += OnCreate;

            BuffManager.Init();
            MineBuffManager.Init();
            Spawner.Init();

            //if(DebugMode)
                //InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
        }
        
        Spawner spawner = new Spawner();
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (e.Button.Equals(SButton.K)) {
                for (int i = 0; i < 3; i++)
                {
                    TwitchSlime m = new TwitchSlime(Vector2.Zero);
                    m.Name = "test name"+ (i > 0 ? "'s minion" : "");
                    Spawner.addMonsterToSpawn(m);



                    //Junimo j = new TwitchJunimo(Vector2.Zero);
                    //j.Name = "test name" + (i > 0 ? "'s npc" : "");
                    //j.collidesWithOtherCharacters.Value = false;
                    //Spawner.addSubToSpawn(j);
                }
            }
        }

        private void OnCreate(object sender, EventArgs e)
        {
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
                BuffManager.UpdateDay();
        }

        private void QuaterSecondUpdate(Bookcase.Events.Event args)
        {
            if (!Context.IsWorldReady)
                return;
            BuffManager.UpdateTick();
            Spawner.UpdateTick();
        }

        private void FullSecondTick(Bookcase.Events.Event args)
        {
            if (!Context.IsWorldReady)
                return;
            MineBuffManager.UpdateTick();
        }
    }
}
