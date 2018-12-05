using System;
using System.Collections;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using System.Linq;
using BNC.Twitch;
using System.Collections.Generic;
using StardewValley.TerrainFeatures;
using BNC.Configs;

namespace BNC
{
    public class Spawner
    {
        private static Dictionary<NPC, string> SpawnList_AroundPlayer = new Dictionary<NPC, string>();
        private static Dictionary<string,NPC> MobsSpawned = new Dictionary<string, NPC>();

        private static object minelvl;

        public static void Init()
        {
            if(BNC_Core.config.Use_Bits_To_Spawn_Mobs)
                GraphicsEvents.OnPostRenderHudEvent += new EventHandler(OnPostRender);
        }


        public static void UpdateTick()
        {

            foreach (string name in MobsSpawned.Keys.ToArray())
            {
                bool flagRmv = false;
                NPC npc = MobsSpawned[name];
                if (npc == null)
                    flagRmv = true;
                else if (!Game1.player.currentLocation.characters.Contains(npc))
                    flagRmv = true;
                else if (npc is Monster && ((Monster)npc).Health <= 0)
                    flagRmv = true;

                if (flagRmv)
                    MobsSpawned.Remove(name);
            }

            if (!Context.CanPlayerMove || Game1.CurrentEvent != null || Game1.isFestival() || Game1.weddingToday)
                return;
            if (SpawnList_AroundPlayer.Count > 0)
            {

               bool flag = false;
                NPC npc = SpawnList_AroundPlayer.Keys.ElementAt(0);
                if (npc is Monster)
                    flag = tryToSpawnNPC(npc, SpawnList_AroundPlayer.Values.ElementAt(0), getRangeFromPlayer(10,4));
                else if (npc is Junimo)
                {
                    Junimo j = (Junimo)npc;
                    j.stayPut.Value = false;
                    flag = tryToSpawnNPC(j, SpawnList_AroundPlayer.Values.ElementAt(0), getRangeFromPlayer(8));
                }

                if (flag) SpawnList_AroundPlayer.Remove(SpawnList_AroundPlayer.Keys.ElementAt(0));
            }
        }

        private static Vector2 getRangeFromPlayer(int range, int minRange = 3)
        {
            int xStart = Game1.player.getTileX() - range;
            int yStart = Game1.player.getTileY() - range;

            int randX = Game1.random.Next(range * 2 + 2);
            int randY = Game1.random.Next(range * 2 + 2);


            Vector2 vector = new Vector2(xStart + randX, yStart + randY);
            while (Vector2.Distance(vector, Game1.player.getTileLocation()) < minRange) {
                vector.X = xStart + Game1.random.Next(range * 2 + 2);
                vector.Y = yStart + Game1.random.Next(range * 2 + 2);
            }



            return vector;
        }
        public static bool canSpawn()
        {
            if (Game1.player.currentLocation.Name.Equals("Hospital"))
                return false;
            else
                return true;
        }

        public static void UpdateDifficulty()
        {
            minelvl = Game1.player.deepestMineLevel;
        }

        public static void addMonsterToSpawn(Monster m, string username)
        { 
            SpawnList_AroundPlayer.Add(m, username);
        }

        public static void addSubToSpawn(NPC sub, string username)
        {
            SpawnList_AroundPlayer.Add(sub, username);
        }

        public static void SpawnTwitchJunimo(string name)
        {
            Junimo j = new TwitchJunimo(Vector2.Zero);
            Spawner.addSubToSpawn(j, name);
        }

        private static bool tryToSpawnNPC(NPC m, string username, Vector2 pos)
        {
            if (!canSpawn() || pos.Equals(Game1.player.getTileLocation()) || !Game1.currentLocation.isTileLocationTotallyClearAndPlaceable(pos) || Game1.player.currentLocation.isTileOccupied(pos, ""))
               return false;

            ((ITwitchMonster)m).setTwitchName(username);

            m.setTilePosition((int)pos.X, (int)pos.Y);
            Game1.player.currentLocation.characters.Add((NPC)m);
            MobsSpawned.Add(username, m);
            if (m is GreenSlime)
                Game1.player.currentLocation.playSoundAt("slime", pos);

            return true;
        }
 
        //## Client Rendering
        private static void OnPostRender(object sender, EventArgs e)
        {
            if (Game1.currentLocation != null && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {

                //                foreach (string name in MobsSpawned.Keys) Tried something different
                foreach (NPC npc in Game1.currentLocation.getCharacters())
                {
                    if (npc is ITwitchMonster)
                    {
                        int localX = npc.GetBoundingBox().Center.X - Game1.viewport.X - ((int)Game1.smallFont.MeasureString(((ITwitchMonster)npc).GetTwitchName()).Length() / 2);
                        int localY = npc.GetBoundingBox().Y - Game1.viewport.Y - (npc is Monster ? 60 : 30);
                        Utility.drawTextWithColoredShadow(Game1.spriteBatch, $"{((ITwitchMonster)npc).GetTwitchName()}", Game1.smallFont, new Vector2(localX, localY), Color.Wheat, Color.Black);
                    }
                }
            }
        }

        public static void ClearMobs()
        {

            foreach (StardewValley.GameLocation location in StardewValley.Game1.locations.ToArray())
            {
                foreach (NPC mob in location.characters.ToArray())
                {
                    if(mob is ITwitchMonster)
                    {
                        location.characters.Remove(mob);
                    }
                }
            }

            MobsSpawned.Clear();
        }

        public static void SpawnTwitchNPC(string username, Monster mob)
        {
           // mob.Name = username;
            addMonsterToSpawn(mob, username);
        }

        public static void AddMonsterToSpawnFromType(TwitchMobType type, string name)
        {
            switch (type)
            {

                case TwitchMobType.Slime:
                    SpawnTwitchNPC(name, new TwitchSlime(Vector2.Zero));
                    break;
                case TwitchMobType.Crab:
                    SpawnTwitchNPC(name, new TwitchCrab(Vector2.Zero));
                    break;
                case TwitchMobType.Bug:
                    SpawnTwitchNPC(name, new TwitchBug(Vector2.Zero));
                    break;
                case TwitchMobType.Fly:
                    SpawnTwitchNPC(name, new TwitchFly(Vector2.Zero));
                    break;
                case TwitchMobType.Bat:
                    SpawnTwitchNPC(name, new TwitchBat(Vector2.Zero));
                    break;
                case TwitchMobType.BigSlime:
                    SpawnTwitchNPC(name, new TwitchBigSlime(Vector2.Zero));
                    break;


                    /* Tried vanilla mobs didn't work
                    case TwitchMobType.Slime:
                        SpawnTwitchNPC(name, new GreenSlime(Vector2.Zero));
                        break;
                    case TwitchMobType.Crab:
                        SpawnTwitchNPC(name, new RockCrab(Vector2.Zero));
                        break;
                    case TwitchMobType.Bug:
                        SpawnTwitchNPC(name, new Bug(Vector2.Zero, 0));
                        break;
                    case TwitchMobType.Fly:
                        SpawnTwitchNPC(name, new Fly(Vector2.Zero));
                        break;
                    case TwitchMobType.Bat:
                        SpawnTwitchNPC(name, new Bat(Vector2.Zero));
                        break;
                    case TwitchMobType.BigSlime:
                        SpawnTwitchNPC(name, new BigSlime(Vector2.Zero, 0));
                        break;
                        */
            }
        }

        public static bool IsMonsterEnabled(TwitchMobType type)
        {
            switch (type)
            {
                case TwitchMobType.Slime:
                    return BNC_Core.config.Bits_To_Spawn_Slimes_Range != null;
                case TwitchMobType.Crab:
                    return BNC_Core.config.Bits_To_Spawn_Crabs_Range != null;
                case TwitchMobType.Bug:
                    return BNC_Core.config.Bits_To_Spawn_Bugs_Range != null;
                case TwitchMobType.Fly:
                    return BNC_Core.config.Bits_To_Spawn_Fly_Range != null;
                case TwitchMobType.Bat:
                    return BNC_Core.config.Bits_To_Spawn_Bat_Range != null;
                case TwitchMobType.BigSlime:
                    return BNC_Core.config.Bits_To_Spawn_Big_Slimes_Range != null;
            }
            return false;
        }

        public static TwitchMobType? GetMonsterFromBits(int bit)
        {
            foreach (TwitchMobType type in Enum.GetValues(typeof(TwitchMobType)))
            {
                if (!IsMonsterEnabled(type))
                    continue;
                BNC_Core.Logger.Log($"bits {bit}");
                int[] range;
                switch (type)
                {
                    case TwitchMobType.Slime:
                        range = GetBitRangeFromMonster(TwitchMobType.Slime);
                        if (Enumerable.Range(range[0], range[1]).Contains(bit))
                            return TwitchMobType.Slime;
                        break;
                    case TwitchMobType.Crab:
                        range = GetBitRangeFromMonster(TwitchMobType.Crab);
                        if (Enumerable.Range(range[0], range[1]).Contains(bit))
                            return TwitchMobType.Crab;
                        break;
                    case TwitchMobType.Bug:
                        range = GetBitRangeFromMonster(TwitchMobType.Bug);
                        if (Enumerable.Range(range[0], range[1]).Contains(bit))
                            return TwitchMobType.Bug;
                        break;
                    case TwitchMobType.Fly:
                        range = GetBitRangeFromMonster(TwitchMobType.Fly);
                        if (Enumerable.Range(range[0], range[1]).Contains(bit))
                            return TwitchMobType.Fly;
                        break;
                    case TwitchMobType.Bat:
                        range = GetBitRangeFromMonster(TwitchMobType.Bat);
                        if (Enumerable.Range(range[0], range[1]).Contains(bit))
                            return TwitchMobType.Bat;
                        break;
                    case TwitchMobType.BigSlime:
                        range = GetBitRangeFromMonster(TwitchMobType.BigSlime);
                        if (Enumerable.Range(range[0], range[1]).Contains(bit))
                            return TwitchMobType.BigSlime;
                        break;
                    default:
                        break;
                }
            }
            return null;
        }


        private static int[] GetBitRangeFromMonster(TwitchMobType type)
        {
            switch (type)
            {
                case TwitchMobType.Slime:
                    return CalculateRanges(BNC_Core.config.Bits_To_Spawn_Slimes_Range);
                case TwitchMobType.Crab:
                    return CalculateRanges(BNC_Core.config.Bits_To_Spawn_Crabs_Range);
                case TwitchMobType.Bug:
                    return CalculateRanges(BNC_Core.config.Bits_To_Spawn_Bugs_Range);
                case TwitchMobType.Fly:
                    return CalculateRanges(BNC_Core.config.Bits_To_Spawn_Fly_Range);
                case TwitchMobType.Bat:
                    return CalculateRanges(BNC_Core.config.Bits_To_Spawn_Bat_Range);
                case TwitchMobType.BigSlime:
                    return CalculateRanges(BNC_Core.config.Bits_To_Spawn_Big_Slimes_Range);
            }
            return new int[] { -1, -2 };
        }

        private static int[] CalculateRanges(int[] ranges)
        {
            if (ranges.Length >= 2)
            {
                int min = Math.Min(ranges[0], ranges[1]);
                int max = Math.Max(ranges[0], ranges[1]);
                int count = max - min;

                if (Config.ShowDebug())
                    BNC_Core.Logger.Log($"min{min} / max{max} - cnt{count}");
                return new int[] { min, count };
            }
            else if (ranges.Length == 1)
            {
                int count = int.MaxValue - ranges[0];
                return new int[] { ranges[0], count };
            }
            return new int[] { -100, -200 };
        }


        public enum TwitchMobType   { Slime, Crab, Bug, Fly, Bat, BigSlime }
    }
}
