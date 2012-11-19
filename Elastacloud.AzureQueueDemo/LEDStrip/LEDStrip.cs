using System;
using Microsoft.SPOT.Hardware;

namespace Elastacloud.AzureQueueDemo.LEDStrip //LEDSTRIP.WS2801
{
    /// <summary>
    /// WS2801 led strip driver
    /// </summary>
    public class LEDStrip
    {
        private int[] _LEDS;
        private bool _needsUpdate = false;
        private OutputPort _clock;
        private OutputPort _data;

        public LEDStrip(Cpu.Pin clockPin, Cpu.Pin dataPin)
        {
            NumOfLEDs = 32;
            _clock = new OutputPort(clockPin, false);
            _data = new OutputPort(dataPin, false);
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


        /// <summary>
        /// Shifts a color value out to led strip
        /// </summary>
        /// <param name="value">Color to output</param>
        private void ShiftOut(int value)
        {
            for (int color_bit = 23 ; color_bit >= 0 ; color_bit--)
            {
                // Lower Clock
                _clock.Write(false);
                //Write bit
                _data.Write(((value>>color_bit) & 0x00000001) != 0);
                // Raise Clock
                _clock.Write(true);
            }
        }

        //Takes the current strip color array and pushes it out
        private void post_frame()
        {
            //Each LED requires 24 bits of data
            //MSB: R7, R6, R5..., G7, G6..., B7, B6... B0 
            //Once the 24 bits have been delivered, the IC immediately relays these bits to its neighbor
            //Pulling the clock low for 500us or more causes the IC to post the data.

            for (int LED_number = 0; LED_number < _LEDS.Length; LED_number++)
            {
                int this_led_color = _LEDS[LED_number]; //24 bits of color data

                for (byte color_bit = 23; color_bit != 255; color_bit--)
                {
                    //Feed color bit 23 first (red data MSB)

                    //digitalWrite(CKI, LOW); //Only change data when clock is low
                    _clock.Write(false);

                    int mask = 1 << color_bit;
                    //The 1'L' forces the 1 to start as a 32 bit number, otherwise it defaults to 16-bit.

                    if ((this_led_color & mask) != 0)
                        //digitalWrite(SDI, HIGH);
                        _data.Write(true);
                    else
                        //digitalWrite(SDI, LOW);
                        _data.Write(false);

                    //digitalWrite(CKI, HIGH); //Data is latched when clock goes high
                    _clock.Write(true);
                }
            }

            //Pull clock low to put strip into reset/post mode
            //digitalWrite(CKI, LOW);
            //delayMicroseconds(500); //Wait for 500us to go into reset
            _clock.Write(false);
            DelayMicroSec(500);
        }
    }
}
