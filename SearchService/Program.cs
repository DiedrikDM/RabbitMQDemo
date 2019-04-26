using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace SearchService
{
  class Program
  {
    public static void Main(string[] args)
    {
      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.ExchangeDeclare(exchange: "topic_tracks",
                                durable: true,
                                type: "topic");
        var queueName = channel.QueueDeclare(exclusive: true, autoDelete: true).QueueName;

        channel.QueueBind(queue: queueName,
                          exchange: "topic_tracks",
                          routingKey: "tracks.#");


        Console.WriteLine($"SearchService {Environment.MachineName} waiting for messages. To exit press CTRL+C");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
          var body = ea.Body;
          var json = Encoding.UTF8.GetString(body);
          switch (ea.RoutingKey)
          {
            case "tracks.removed":
              var removedEvent = JsonConvert.DeserializeObject<TrackRemovedIntegrationEvent>(json);
              Console.WriteLine($"Received removed {removedEvent.TrackId}");
              break;
            case "tracks.added":
              var addedEvent = JsonConvert.DeserializeObject<TrackAddedIntegrationEvent>(json);
              Console.WriteLine($"Received added {addedEvent.TrackId}");
              break;
            default:
              break;
          }

          channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: queueName,
                             autoAck: false,
                             consumer: consumer);

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
      }
    }
  }
}
