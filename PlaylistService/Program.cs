using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace PlaylistService
{
  class Program
  {
    public static void Main(string[] args)
    {
      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "topic_tracks_playlist_sub",
                 durable: false,
                 exclusive: false,
                 autoDelete: false);

        channel.ExchangeDeclare(exchange: "topic_tracks",
                        durable: true,
                        type: "topic");

        channel.QueueBind(queue: "topic_tracks_playlist_sub",
                          exchange: "topic_tracks",
                          routingKey: "tracks.removed");

        Console.WriteLine($"Playlist {Environment.MachineName} waiting for messages. To exit press CTRL+C");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
          var body = ea.Body;
          var json = Encoding.UTF8.GetString(body);
          var removedEvent = JsonConvert.DeserializeObject<TrackRemovedIntegrationEvent>(json);
          Console.WriteLine($"Received removed {removedEvent.TrackId}");

          channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: "topic_tracks_playlist_sub",
                             autoAck: false,
                             consumer: consumer);

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
      }
    }
  }
}
