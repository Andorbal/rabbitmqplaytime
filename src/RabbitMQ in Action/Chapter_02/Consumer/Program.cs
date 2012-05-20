using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Consumer {
  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Starting the consumer...");

      var connectionFactory = new ConnectionFactory {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
      };

      using (var connection = connectionFactory.CreateConnection()) {
        using (var channel = connection.CreateModel()) {
          channel.ExchangeDeclare("hello-exchange", "direct", true, false, null);
          channel.QueueDeclare("hello-queue", true, false, false, null);
          channel.QueueBind("hello-queue", "hello-exchange", "hola");

          var consumer = new QueueingBasicConsumer(channel);
          var consumerTag = channel.BasicConsume("hello-queue", false, consumer);
          
          while (true) {
            try {
              var e = (RabbitMQ.Client.Events.BasicDeliverEventArgs) consumer.Queue.Dequeue();
              var bodyString = Encoding.UTF8.GetString(e.Body);
              channel.BasicAck(e.DeliveryTag, false);
              if (string.Equals(bodyString, "quit", StringComparison.OrdinalIgnoreCase)) {
                channel.BasicCancel(consumerTag);
                break;
              }

              Console.WriteLine(bodyString);
            }
            catch (OperationInterruptedException ex) {
              break;
            }
          }
        }
      }
    }
  }
}
