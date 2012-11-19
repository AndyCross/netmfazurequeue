using System;
using Elastacloud.AzureTableDemo;
using Microsoft.SPOT;

namespace Elastacloud.AzureQueueDemo.SharksLasers
{
    public class SharkWithAFreakinLaser
    {
        private readonly QueueClient _queueClient;

        public SharkWithAFreakinLaser(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public void ZAP()
        {
            var score = new Random().Next(32);

            Debug.Print("Zapping for " + score);
            _queueClient.CreateQueueMessage(score.ToString());
        }

        public void CHOMP()
        {
            var score = new Random().Next(16);

            Debug.Print("Chomping for " + score);
            _queueClient.CreateQueueMessage(score.ToString());
        }
    }
}
