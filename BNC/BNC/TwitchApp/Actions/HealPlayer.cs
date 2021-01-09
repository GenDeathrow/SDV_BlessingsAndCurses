using System;
using BNC.TwitchApp;
using Newtonsoft.Json;
using StardewValley;


namespace BNC.Actions
{
    class HealPlayer : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "heal")]
        public int HealthIn;


        public override ActionResponse Handle()
        {
            BNC_Core.Logger.Log($"Healing Player {HealthIn}", StardewModdingAPI.LogLevel.Debug);
            
            if (Game1.player.health + HealthIn > Game1.player.maxHealth)
                Game1.player.health = Game1.player.maxHealth;
            else
                Game1.player.health += HealthIn;

            return ActionResponse.Done;
        }
    }
}
