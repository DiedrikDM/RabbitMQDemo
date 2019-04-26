using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace CatalogService
{
  class Program
  {
    static void Main(string[] args)
    {
      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        //channel.QueueDeclare(queue: "queue_tracks",
        //                     durable: false,
        //                     exclusive: false,
        //                     autoDelete: false);

        channel.ExchangeDeclare(exchange: "topic_tracks",
                                  durable: true,
                                  type: "topic");

        Console.WriteLine("press any key to exit");
        while (!Console.KeyAvailable)
        {
          SendTrackRemovedMessage(channel);
          Thread.Sleep(TimeSpan.FromSeconds(1));
          SendTrackAddedMessage(channel);
          Thread.Sleep(TimeSpan.FromSeconds(1));
        }
      }
    }

    private static void SendTrackRemovedMessage(IModel channel)
    {
      var trackId = Guid.NewGuid();
      var integrationEvent = new TrackRemovedIntegrationEvent(trackId.ToString(), "song too bad");

      var json = JsonConvert.SerializeObject(integrationEvent);
      var body = Encoding.UTF8.GetBytes(json);

      channel.BasicPublish(exchange: "topic_tracks",
                           routingKey: "tracks.removed",
                           basicProperties: null,
                           body: body);

      Console.WriteLine($"Sent TrackRemoved {trackId}");
    }

    private static void SendTrackAddedMessage(IModel channel)
    {
      var trackId = Guid.NewGuid();
      var integrationEvent = new TrackRemovedIntegrationEvent(trackId.ToString(), "Love is all you need");

      var json = JsonConvert.SerializeObject(integrationEvent);
      var body = Encoding.UTF8.GetBytes(json);

      channel.BasicPublish(exchange: "topic_tracks",
                           routingKey: "tracks.added",
                           basicProperties: null,
                           body: body);

      Console.WriteLine($"Sent TrackAdded {trackId}");
    }

  }
}
