using System;

namespace Elastacloud.AzureQueueDemo.LEDStrip //LEDSTRIP.WS2801
{
    public class ColorHelper
    {
        public static int Black = 0x00000000;
        public static int White = 0xFFFFFF;
        public static int Red = 0xFF0000;
        public static int Green = 0x00FF00;
        public static int Blue = 0x0000FF;
        public static int Yellow = 0x00FFFF00;
        public static int Purple = 0x00FF00FF;
        public static int Cyan = 0x0000FFFF;

        private static int[] _colors = new int[] { Black, White, Red, Green, Blue, Yellow, Purple, Cyan };

        // Indexer implementation.
        public int this[int index]
        {
            get
            {
                return _colors[index];
            }
        }

        public static int Length
        {
            get
            {
                return _colors.Length;
            }
        }

        public static int RandomColor()
        {
            Random rand = new Random();
            return (int)rand.Next((int)White);
        }

        public static int GetRandomColor()
        {
            Random rand = new Random();
            return _colors[rand.Next(_colors.Length-1)+1];
        }

        public static int RandomBlue()
        {
            Random rand = new Random();
            return (int)rand.Next(0x000000FF) & White;
        }

        public static int RandomGreen()
        {
            Random rand = new Random();
            return (int)(rand.Next(0x000000FF) << 2) & White;
        }

        public static int RandomRed()
        {
            Random rand = new Random();
            return (int)(rand.Next(0x000000FF) << 4) & White;
        }

        public static int Color(byte red, byte green, byte blue)
        {
            return ((int)red << 4) & ((int)green << 2) & ((int)blue) & (White);
        }
    }
}
