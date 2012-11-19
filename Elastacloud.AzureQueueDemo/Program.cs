using System;
using System.Threading;
using Elastacloud.AzureBlobDemo.NTP;
using Elastacloud.AzureQueueDemo.LEDStrip;
using Elastacloud.AzureQueueDemo.SharksLasers;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Elastacloud.AzureTableDemo
{
    public class Program
    {
        private static LEDStripSpi _ledStrip;
        private static int healLevel = 1;

        public static void Main()
        {
            var mode = SharksLaserPlayerMode.AustinWithSharkWithAFreakinLaserOnSameFreakinDevice;

            //retrive and set device time via NTP if current value is shipdate (in 2011)
            if (DateTime.Now < new DateTime(2012,01,01))
            {
                var networkTime = NtpClient.GetNetworkTime();
                Utility.SetLocalTime(networkTime);
            }

            var queueClient = new QueueClient("netmf",
                                              "UstPuYqYwj1EEIc815wcVxV6oItRmrvRVByl7A152XoVeDJMr7vn1cahO5xXg0q8z5rSjd6SmQRWJliGQH9j0Q==");

            _ledStrip = new LEDStripSpi();
            _ledStrip.Initialize();

            if (mode == SharksLaserPlayerMode.Austin)
            {   
                GuyInTuxedo austin = new GuyInTuxedo(queueClient);
                Timer healingMojo = new Timer(austin.Heal, 4, 0, 1000);//austin's mojo heals him 4 points a second. groovy baby.
                while (true)
                {
                    austin.GetDamage();
                    Debug.Print("How's austin feeling? : " + austin.HitPoints);
                    UpdateLEDStrip(austin.HitPoints);
                }
            }
            else if (mode == SharksLaserPlayerMode.SharkWithAFreakinLaser)
            {
                SharkWithAFreakinLaser drEvilsBirthdayPresent = new SharkWithAFreakinLaser(queueClient);
                while (true)
                {
                    drEvilsBirthdayPresent.ZAP();
                    drEvilsBirthdayPresent.CHOMP();
                }
            }
            else if (mode == SharksLaserPlayerMode.AustinWithSharkWithAFreakinLaserOnSameFreakinDevice)
            {
                GuyInTuxedo austin = new GuyInTuxedo(queueClient);
                SharkWithAFreakinLaser drEvilsBirthdayPresent = new SharkWithAFreakinLaser(queueClient);

                Timer healingMojo = new Timer(austin.Heal, healLevel, 0, 1000);//austin's mojo heals him 1 point a second. groovy baby.
                Timer damageRetriever = new Timer(austin.GetDamage, null, 0, 1000);//austin pats himself down every second.

                while (true)
                {
                    Debug.Print("How's austin feeling? : " + austin.HitPoints);
                    UpdateLEDStrip(austin.HitPoints);
                    drEvilsBirthdayPresent.ZAP();
                    Debug.Print("How's austin feeling? : " + austin.HitPoints);
                    UpdateLEDStrip(austin.HitPoints);
                    drEvilsBirthdayPresent.CHOMP();

                    healingMojo = AdjustHealingEquilibrium(austin, healingMojo);
                }
            }

        }

        private static Timer AdjustHealingEquilibrium(GuyInTuxedo austin, Timer healingMojo)
        {
            if (austin.HitPoints < 5)
            {
                healLevel += 5;
                healingMojo.Dispose();
                healingMojo = new Timer(austin.Heal, healLevel, 0, 1000);
            }
            else if (austin.HitPoints == GuyInTuxedo.MaxHitPoints)
            {
                healLevel--;
                healingMojo.Dispose();
                healingMojo = new Timer(austin.Heal, healLevel, 0, 1000);
            }
            return healingMojo;
        }

        private static void UpdateLEDStrip(object state)
        {
            var hitPoints = (int) state;
            for (int i = 0; i < _ledStrip.NumOfLEDs; i++)
            {
                if (hitPoints < i+1)
                {
                    _ledStrip.SetLed(i, ColorHelper.Red);
                }
                else
                {
                    _ledStrip.SetLed(i, ColorHelper.Green);
                }
            }
            _ledStrip.Update();
        }
    }
}
