using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Elastacloud.AzureQueueDemo.LEDStrip
{
    /// <summary>
    /// WS2801 led strip driver
    /// </summary>
    public class LEDStripSpi
    {
        private int[] _LEDS;
        private bool _needsUpdate = false;
        private SPI _data;

        public LEDStripSpi()
        {
            NumOfLEDs = 32;

            _data = new SPI(new SPI.Configuration(Pins.GPIO_NONE, false, 0, 0, false, true, 100, SPI.SPI_module.SPI1));

            post_frame();
        }

        /// <summary>
        /// Number of LEDs in strip
        /// </summary>
        public int NumOfLEDs
        {
            get
            {
                return _LEDS.Length;
            }
            set
            {
                _LEDS = new int[value];
                for (byte i = 0; i < _LEDS.Length; i++)
                {
                    _LEDS[i] = 0;
                }
            }
        }

        /// <summary>
        /// Update the led strip
        /// </summary>
        public void Update()
        {
            //only push new LEDs out if they changed - saves processing time
            if (_needsUpdate)
            {
                /*for (int i = 0; i < _LEDS.Length; i++)
                {
                    Debug.Print("u " + i + " " + Program.ConvertIntToHex(_LEDS[i]));
                    ShiftOut(_LEDS[i]);
                }
                _clock.Write(false);
                DelayMicroSec(500);*/
                post_frame();
                _needsUpdate = false;
            }
        }
    
        /// <summary>
        /// Force an update to the led strip
        /// </summary>
        public void Initialize()
        {
            _needsUpdate = true;
            Update();
        }

        /// <summary>
        /// Set a led to a specific color
        /// </summary>
        /// <param name="index">led to set</param>
        /// <param name="color">color to set the led</param>
        public void SetLed(int index, int color)
        {
            if (_LEDS[index] != color)
            {
                //Debug.Print(index + " " + Program.ConvertIntToHex(color));
                _LEDS[index] = color;
                _needsUpdate = true;
            }
        }

        /// <summary>
        /// Blocks thread for given number of microseconds
        /// </summary>
        /// <param name="microSeconds">Delay in microseconds</param>
        private void DelayMicroSec(int microSeconds)
        {
            DateTime startTime = DateTime.Now;

            int stopTicks = microSeconds * 10;

            TimeSpan divTime = DateTime.Now - startTime;
            while (divTime.Ticks < stopTicks)
            {
                divTime = DateTime.Now - startTime;
            }
        }

        private void post_frame()
        {
            //Each LED requires 24 bits of data 
            //MSB: R7, R6, R5..., G7, G6..., B7, B6... B0  
            //Once the 24 bits have been delivered, the IC immediately relays these bits to its neighbor 
            //Pulling the clock low for 500us or more causes the IC to post the data.

            //3 because three bytes per color.
            int numColorBytes = 3;
            byte[] writeBuf = new byte[numColorBytes * sizeof(byte) * _LEDS.Length];

            int j = 0;


            for (int LED_number = 0; LED_number < _LEDS.Length; LED_number++)
            {
                int this_led_color = _LEDS[LED_number]; //24 bits of color data 

                byte[] data = IntToBytes(this_led_color);

                if (data.Length > 3) Debug.Assert(data.Length == 3);//ofcourse this is nonsensical, but code that inspired this thought data was byte[4]

                writeBuf[j] = data[0];
                j++;
                writeBuf[j] = data[1];
                j++;
                writeBuf[j] = data[2];
                j++;
            }

            _data.Write(writeBuf);
        }


        private byte[] IntToBytes(int val)
        {
            //-1 because 24 bit color
            byte[] rslt = new byte[sizeof(int) - 1];

            //-2 instead of -1 because 24 bit color
            int numBytes = sizeof(int) - 1;

            for (int i = numBytes - 1; i > 0; i--)
            {
                //Using numBytes-i because data is supposed to be in MSB format.
                rslt[i] = (byte)((val >> ((i) * 8)) & 0x000000FF);
            }
            return rslt;
        }
    }
}