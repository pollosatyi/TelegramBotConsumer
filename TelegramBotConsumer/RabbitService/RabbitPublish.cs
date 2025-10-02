using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherTelegramBot.Data;
using WeatherTelegramBot.Models;

namespace TelegramBotPublish.Services.RabbitService
{
    internal class RabbitPublish
    {
        private readonly string _hostName = "localhost";
        private readonly string _userName = "admin";
        private readonly string _password = "password123";
        private readonly IWeatherRepo _weatherRepo;
        
        private ConnectionFactory? _connectionFactory;


        public RabbitPublish(IWeatherRepo weatherRepo)
        {
           _weatherRepo = weatherRepo;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password

            };

        }

        public async Task PutMessageAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            string nameQueue = "weatherQueue";
            await CreateQueue(channel, nameQueue);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var weatherData = JsonSerializer.Deserialize<WeatherModel>(message);
                    if (weatherData != null)
                    {
                        await _weatherRepo.CreateModel(weatherData);
                    }
                    await channel.BasicAckAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false
                        );
                    

                }
                catch (Exception ex)
                {
                    await channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true

                        );

                }

            };
            await channel.BasicConsumeAsync(
                queue: nameQueue,
                autoAck: false,
                consumer: consumer
                );

            

        }

        

        private async Task CreateQueue(IChannel channel, string nameQueue)
        {
            await channel.QueueDeclareAsync(queue: nameQueue,
                exclusive: false,
                durable: true,
                autoDelete: false,
                arguments: null
                );
        }


    }
}
