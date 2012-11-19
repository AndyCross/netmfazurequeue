namespace Elastacloud.AzureTableDemo
{
    public class QueueMessageWrapper
    {
        public string Message { get; set; }
        public string PopReceipt { get; set; }
        public string MessageId { get; set; }
    }
}