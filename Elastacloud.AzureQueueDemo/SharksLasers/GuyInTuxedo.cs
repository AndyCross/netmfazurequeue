using System;
using System.Threading;
using Elastacloud.AzureTableDemo;
using Microsoft.SPOT;

namespace Elastacloud.AzureQueueDemo.SharksLasers
{
    public class GuyInTuxedo
    {
        private readonly QueueClient _queueClient;
        private int _hitPoints;
        public int HitPoints { get { return _hitPoints; } }

        public const int MaxHitPoints = 32;

        public GuyInTuxedo(QueueClient queueClient)
        {
            _queueClient = queueClient;
            _hitPoints = MaxHitPoints;
        }

        public void ApplyDamage(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (_hitPoints > 0)
                {
                    //Debug.Print("Took Damage");
                    Interlocked.Decrement(ref _hitPoints);
                }
                else
                {
                    Debug.Print("Lost his mojo.");
                }
            }
        }

        public void Heal(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (_hitPoints < MaxHitPoints)
                {
                    //Debug.Print("Healed");
                    Interlocked.Increment(ref _hitPoints);
                }
                else
                {
                    Debug.Print("Very very groovy baby.");
                    break;
                }
            }
        }

        public void Heal(object state)
        {
            Heal((int)state);
        }

        public void GetDamage()
        {
            var messageWrapper = _queueClient.RetrieveQueueMessage();

            if (messageWrapper == null || messageWrapper.Message == null || messageWrapper.Message == "") return;

            var damage = int.Parse(messageWrapper.Message);

            ApplyDamage(damage);

            _queueClient.DeleteMessage(messageWrapper.MessageId, messageWrapper.PopReceipt);
        }

        public void GetDamage(object state)
        {
            GetDamage();
        }
    }
}
