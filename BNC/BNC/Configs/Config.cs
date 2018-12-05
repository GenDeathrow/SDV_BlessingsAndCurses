
namespace BNC.Configs
{
    public class Config
    {
        public bool Enable_Twitch_Integration { get; set; } = true;

        public bool Random_Day_Buffs { get; set; } = true;

        public int[] Random_Day_Buff_Min_Max { get; set; } = new int[] { 2, 4 };

        public bool Use_Bits_To_Spawn_Mobs { get; set; } = true;

        public int[] Bits_To_Spawn_Slimes_Range { get; set; } = new int[] {50,99};

        public int[] Bits_To_Spawn_Bugs_Range { get; set; } = new int[] { 100, 199 };

        public int[] Bits_To_Spawn_Crabs_Range { get; set; } = new int[] { 200, 299 };

        public int[] Bits_To_Spawn_Fly_Range { get; set; } = new int[] { 300, 399 };

        public int[] Bits_To_Spawn_Bat_Range { get; set; } = new int[] { 400, 499 };

        public int[] Bits_To_Spawn_Big_Slimes_Range { get; set; } = new int[] { 500 };

        public bool Spawn_Subscriber_Junimo { get; set; } = true;

        public bool Spawn_GiftSub_Subscriber_Mobs { get; set; } = true;

        public int Mine_Augment_Every_x_Levels { get; set; } = 5;

        public int Twitch_Vote_Time_In_Secs { get; set; } = 45;

        public bool Show_Debug_Text { get; set; } = false;


        public static int[] getBuffMinMax()
        {
            int[] value = BNC_Core.config.Random_Day_Buff_Min_Max;
            if (value.Length == 2)
                return value;
            else if (value.Length == 1)
                return new int[] { value[0], value[0] };
            else
                return new int[] { 2, 4 };
        }

        public static int getVotingTime()
        {
            return BNC_Core.config.Twitch_Vote_Time_In_Secs;
        }

        public static bool ShowDebug()
        {
            return BNC_Core.config.Show_Debug_Text;
        }



    }
}
