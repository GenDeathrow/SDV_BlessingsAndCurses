﻿using System;
using Newtonsoft.Json;
using static BNC.Spawner;
using StardewModdingAPI;
using BNC.TwitchApp;

namespace BNC.Actions
{
    class Spawn : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "mob")]
        public string Monster;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amt")]
        public int amt = 1;

        public override ActionResponse Handle()
        { 
            try
            {
                TwitchMobType mob;

                BNC_Core.Logger.Log($"Spawning {amt} {Monster} from Actions", LogLevel.Error);

                if (Monster == "junimo")
                {
                    for (int i = 0; i < this.amt; i++)
                    {
                        Spawner.SpawnTwitchJunimo(from);
                    }
                }
                else
                {
                    Spawner.MobTypeToEnum.TryGetValue(Monster, out mob);

                    for (int i = 0; i < this.amt; i++)
                    {
                        Spawner.AddMonsterToSpawnFromType(mob, from);
                    }
                }
                
                return ActionResponse.Done;
            }
            catch (ArgumentNullException)
            {
                BNC_Core.Logger.Log($"Error trying to Spawn {Monster} from Actions", LogLevel.Error);
                return ActionResponse.Done;
            }
            
        }
    }
}
