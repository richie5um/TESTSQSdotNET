using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;

namespace TESTAwsSqs
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Console.Write(GetServiceOutput());
            Console.Write(SQSMain());
            Console.Read();
        }

        public static string SQSMain()
        {
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                // Set up the config and create the SQS Client (note, the credentials are configured in app.config)
                AmazonSQSConfig awsSQSConfig = new AmazonSQSConfig();
                awsSQSConfig.ServiceURL = "https://sqs.us-west-2.amazonaws.com";
                AmazonSQSClient awsSQSClient = new AmazonSQSClient(awsSQSConfig);

                // Create a Send Message
                Amazon.SQS.Model.SendMessageRequest sendRequest = new Amazon.SQS.Model.SendMessageRequest();
                sendRequest.QueueUrl = "https://sqs.us-west-2.amazonaws.com/935977267825/RichS-SQS";
                sendRequest.MessageBody = "Hello from C#";

                // Send the Message
                Amazon.SQS.Model.SendMessageResponse sendResponse = awsSQSClient.SendMessage(sendRequest);
                sr.WriteLine(sendResponse.MessageId);

                // Receive a Message
                Amazon.SQS.Model.ReceiveMessageRequest recvRequest = new Amazon.SQS.Model.ReceiveMessageRequest();
                recvRequest.QueueUrl = "https://sqs.us-west-2.amazonaws.com/935977267825/RichS-SQS";
                Amazon.SQS.Model.ReceiveMessageResponse recvResponse = awsSQSClient.ReceiveMessage(recvRequest);
                if (0 < recvResponse.Messages.Count)
                {
                    // Output the received messages
                    foreach( var message in recvResponse.Messages)
                    {
                        sr.WriteLine(message.Body);

                        // Delete the Message (to indicate we've _handled_ it)
                        Amazon.SQS.Model.DeleteMessageRequest delRequest = new Amazon.SQS.Model.DeleteMessageRequest();
                        delRequest.QueueUrl = "https://sqs.us-west-2.amazonaws.com/935977267825/RichS-SQS";
                        delRequest.ReceiptHandle = message.ReceiptHandle;

                        Amazon.SQS.Model.DeleteMessageResponse delResponse = awsSQSClient.DeleteMessage(delRequest);
                        sr.WriteLine(delResponse.HttpStatusCode);
                    }
                }

                sr.WriteLine("Press return key to continue...");
            }

            return sb.ToString();
        }
    }
}