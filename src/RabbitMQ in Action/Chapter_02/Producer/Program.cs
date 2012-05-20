using System;
using System.Text;
using RabbitMQ.Client;

namespace Producer {
  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Starting the producer...");

      var connectionFactory = new ConnectionFactory {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
      };

      using (var connection = connectionFactory.CreateConnection()) {
        using (var channel = connection.CreateModel()) {
          channel.ExchangeDeclare("hello-exchange", "direct", true, false, null);

          Console.WriteLine("Enter messages now:");
          string message;
          do {
            message = Console.ReadLine();
            var properties = channel.CreateBasicProperties();
            properties.ContentType = "text/plain";
            channel.BasicPublish("hello-exchange", "hola",
                               properties, Encoding.UTF8.GetBytes(message));
          } while (!string.Equals(message, "quit", StringComparison.OrdinalIgnoreCase));
        }
      }
    }
  }
}
