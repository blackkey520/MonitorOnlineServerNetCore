using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.RunTime
{
    public class Module_RabbitMQ: IMsgQueue
    {
        ConnectionFactory connectionFactory;
        IConnection connection;
        IModel channel;
        private readonly ILogger<Module_RabbitMQ> m_logger;
        string exchangeName = "MonitorServer";
        public string ModuleName => "RabbitMQ通信模块";

        public bool IsRun => true;
        public Module_RabbitMQ()
        {
            try
            {
                //创建连接工厂
                connectionFactory = new ConnectionFactory
                {
                    HostName = "127.0.0.1",//IP地址
                    Port = 5672,//端口号
                    UserName = "guest",//用户账号
                    Password = "guest"//用户密码
                };
                //创建连接
                connection = connectionFactory.CreateConnection();
                //创建通道
                channel = connection.CreateModel();
                //声明交换机
                channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            }
            catch (Exception e)
            {
                m_logger.LogError("消息队列异常-消息队列初始化异常");
            }
        }
        public bool Start()
        {
            return true;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queName"></param>
        /// <param name="msg"></param>
        public void SendMsg<T>(string queName, T msg) where T : class
        {
            //声明一个队列
            channel.QueueDeclare(queName, true, false, false, null);
            //绑定队列，交换机，路由键
            channel.QueueBind(queName, exchangeName, queName);

            var basicProperties = channel.CreateBasicProperties();
            //1：非持久化 2：可持久化
            basicProperties.DeliveryMode = 2;
            var json = JsonConvert.SerializeObject(msg, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var eventBody = Encoding.UTF8.GetBytes(json);
            var address = new PublicationAddress(ExchangeType.Direct, exchangeName, queName);

            channel.BasicPublish(address, basicProperties, eventBody);
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="queName"></param>
        /// <param name="received"></param>
        public void Receive(string queName, Action<object> received)
        {
            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var messageBody = ea.Body;
                var json = Encoding.UTF8.GetString(messageBody.ToArray());
                var message = JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                received(message);
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queName, false, consumer);

        }
        public bool Stop()
        {
            return true;
        }
    }
}
