using System;
using System.Collections;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;


namespace BNC
{
    class Spawner
    {
        private static ArrayList SpawnList_AroundPlayer = new ArrayList();
        private static ArrayList MobsSpawned = new ArrayList();

        public static void Init()
        {
            if(BNC_Core.config.Use_Bits_To_Spawn_Mobs)
                GraphicsEvents.OnPostRenderHudEvent += new EventHandler(OnPostRender);
        }


        public static void UpdateTick()
        {
            if (!Context.CanPlayerMove || Game1.CurrentEvent != null || Game1.isFestival() || Game1.weddingToday)
                return;
            if (SpawnList_AroundPlayer.Count > 0)
            {

               bool flag = false;
                if (SpawnList_AroundPlayer[0] is Monster)
                    flag = tryToAddMonster((NPC)SpawnList_AroundPlayer[0], (int)Game1.player.getTileX() + Game1.random.Next(8), (int)Game1.player.getTileY() + Game1.random.Next(8));
                else if (SpawnList_AroundPlayer[0] is Junimo)
                {
                    Junimo j = (Junimo)SpawnList_AroundPlayer[0];
                    j.stayPut.Value = false;
                    flag = tryToAddMonster((NPC)SpawnList_AroundPlayer[0], (int)Game1.player.getTileX() + Game1.random.Next(3), (int)Game1.player.getTileY() + Game1.random.Next(3));
                }

               if (flag) SpawnList_AroundPlayer.RemoveAt(0);
            }
        }

        public static void addMonsterToSpawn(Monster m)
        {
            SpawnList_AroundPlayer.Add(m);
        }

        private static bool tryToAddMonster(NPC m, int tileX, int tileY)
        {
            Vector2 v = new Vector2((float)tileX, (float)tileY);
            if (v.Equals(Game1.player.getTileLocation()) || !Game1.currentLocation.isTileLocationTotallyClearAndPlaceable(v) || Game1.player.currentLocation.isTileOccupied(v, ""))
               return false;
            m.setTilePosition(tileX, tileY);
            Game1.player.currentLocation.characters.Add((NPC)m);
            MobsSpawned.Add(m);

            if (m is Monster)
                Game1.player.currentLocation.playSoundAt("slime", v);

            return true;
        }
 
        //## Client Rendering
        private static void OnPostRender(object sender, EventArgs e)
        {
            if (Game1.currentLocation != null && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                foreach (NPC npc in Game1.currentLocation.getCharacters())
                {
                    if (npc is TwitchSlime || npc is TwitchJunimo)
                    {
                        int localX = npc.GetBoundingBox().Center.X - Game1.viewport.X - ((int)Game1.smallFont.MeasureString(npc.Name).Length() / 2);
                        int localY = npc.GetBoundingBox().Y - Game1.viewport.Y - (npc is TwitchSlime ? 60 : 30);
                        Utility.drawTextWithShadow(Game1.spriteBatch, $"{npc.Name}", Game1.smallFont, new Vector2(localX, localY), Color.OrangeRed, 1f, -1f, -1, -1, 1f, 3);
                    }
                }
            }
        }

        public static void ClearMobs()
        {
            foreach(NPC m in MobsSpawned)
            {
                GameLocation location = m.currentLocation;
                location.characters.Remove(m);
            }

            MobsSpawned.Clear();
        }

        public static void addSubToSpawn(NPC sub)
        {
            SpawnList_AroundPlayer.Add(sub);
        }
    }
}
