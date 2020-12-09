using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SendCloudToDevice
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=KeyHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=alcUDny8jMKl/+te/up3PjX0LDwKZK2ULxT69ksXNWc=";
        static string targetDevice = "MyDotnetDevice";

        private async static Task SendDeviceToCloudMessagesAsync(string input)
        {
            var commandMessage = new
            Message(Encoding.ASCII.GetBytes(input));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(targetDevice, commandMessage);
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}",
                  string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
        static void Main(string[] args)
        {
            //int counter = 0;
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            while(true)
            {
                ReceiveFeedbackAsync();
                Console.WriteLine("Press any key to send a message.");
                string input = Console.ReadLine();
                SendDeviceToCloudMessagesAsync(input).Wait();
                //Console.ReadLine();
                //counter++;
            }
           
        }
    }
}
