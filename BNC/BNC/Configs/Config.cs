
namespace BNC.Configs
{
    public class Config
    {
        public bool Random_Day_Buffs { get; set; } = true;

        public int[] Random_Day_Buff_Min_Max { get; set; } = new int[] { 2, 4 };

        public bool Use_Bits_To_Spawn_Mobs { get; set; } = true;

        public int Bits_To_Mobs_Bit_Amount { get; set; } = 100;

        public bool Spawn_Subscriber_Junimo { get; set; } = true;

        public int Mine_Augment_Every_x_Levels { get; set; } = 5;

        public int Twitch_Vote_Time_In_Secs { get; set; } = 30;



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

    }
}
