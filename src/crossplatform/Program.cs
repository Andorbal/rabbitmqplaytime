using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace RabbitMQTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var cf = new ConnectionFactory();
            cf.Endpoint.HostName = "localhost";
            cf.Password = "guest";
            cf.UserName = "guest";
            using (IConnection conn = cf.CreateConnection())
            {
                using (IModel ch = conn.CreateModel())
                {
                    string queueName = ensureQueue(ch);

                    /* We'll consume msgCount message twice: once
                       using Subscription.Next() and once using the
                       IEnumerator interface.  So, we'll send out
                       2*msgCount messages. */
                    using (var sub = new Subscription(ch, queueName))
                    {
                        blockingReceiveMessages(sub, 100);
                    }
                }
            }

            return;
        }


        private static void blockingReceiveMessages(Subscription sub, long msgCount)
        {
            Console.WriteLine("Receiving {0} messages (using a Subscriber)", msgCount);

            for (int i = 0; i < msgCount; ++i)
            {
                Console.WriteLine("Message {0}: {1} (via Subscription.Next())",
                                  i, messageText(sub.Next()));
                Console.WriteLine("Message {0} again: {1} (via Subscription.LatestEvent)",
                                  i, messageText(sub.LatestEvent));
                sub.Ack();
            }

            Console.WriteLine("Done.\n");
        }

        private static string messageText(BasicDeliverEventArgs ev)
        {
            return Encoding.UTF8.GetString(ev.Body);
        }

        private static string ensureQueue(IModel ch)
        {
            Console.WriteLine("Creating a queue and binding it to amq.direct");
            string queueName = ch.QueueDeclare("tasks4", true, false, false, null);
            ch.QueueBind(queueName, "amq.direct", queueName);
            //ch.QueueBind(queueName, "amq.direct", queueName, null);
            Console.WriteLine("Done.  Created queue {0} and bound it to amq.direct.\n", queueName);
            return queueName;
        }
    }
}
