using System;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

namespace awssqsPublisher
{
    public class App
    {
        private readonly IConfiguration _configuration;
        private static IAmazonSQS _sqsClient;
        private const string JsonMessage = "{\"product\":[{\"name\":\"Product A\",\"price\": \"32\"},{\"name\": \"Product B\",\"price\": \"27\"}]}";

        public App(IConfiguration configuration, IAmazonSQS sqsClient)
        {
            _configuration = configuration;
            _sqsClient = sqsClient;
        }

        public void Run()
        {
            var sqsConfig = _configuration.GetSection("SQS");

            SendMessageRequest request = new SendMessageRequest
            {
                QueueUrl = _configuration["AWS:SQS:QueueUrl"],
                MessageBody = JsonMessage
            };
            SendMessageResponse response = Task.Run(async () => await _sqsClient.SendMessageAsync(request)).Result;

            Console.WriteLine($"Message added to queue\n");
            Console.WriteLine($"HttpStatusCode: {response.HttpStatusCode}");
        }
    }
}