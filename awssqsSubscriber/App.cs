using System;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

namespace awssqsSubscriber
{
    public class App
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonSQS _sqsClient;

        public App(IConfiguration configuration, IAmazonSQS sqsClient)
        {
            _configuration = configuration;
            _sqsClient = sqsClient;
        }

        public void Run()
        {
            Console.WriteLine($"Reading messages from queue\n");
            Console.WriteLine("Press any key to stop. (Response might be slightly delayed.)");
            do
            {
                var msg = GetMessage();
                if (msg.Messages.Count != 0)
                {
                    if (ProcessMessage(msg.Messages[0]))
                        DeleteMessage(msg.Messages[0]);
                }
            } while (!Console.KeyAvailable);
        }

        private ReceiveMessageResponse GetMessage(int waitTime = 0)
        {
            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                QueueUrl = _configuration["AWS:SQS:QueueUrl"],
                WaitTimeSeconds = waitTime
            };
            ReceiveMessageResponse response = Task.Run(async () => await _sqsClient.ReceiveMessageAsync(request)).Result;
            return response;
        }

        private bool ProcessMessage(Message message)
        {
            Console.WriteLine($"\nMessage body of {message.MessageId}:");
            Console.WriteLine($"{message.Body}");
            return true;
        }

        private void DeleteMessage(Message message)
        {
            Console.WriteLine($"\nDeleting message {message.MessageId} from queue...");
            DeleteMessageRequest request = new DeleteMessageRequest
            {
                QueueUrl = _configuration["AWS:SQS:QueueUrl"],
                ReceiptHandle = message.ReceiptHandle
            };
            Task.Run( async() => await _sqsClient.DeleteMessageAsync(request));
        }
    }
}