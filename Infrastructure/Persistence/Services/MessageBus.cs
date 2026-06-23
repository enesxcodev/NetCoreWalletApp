using Application.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Services
{
    public class MessageBus(IPublishEndpoint publishEndpoint) : IMessageBus
    {
        public async Task PublishAsync<T>(T @event, string queueName, CancellationToken cancellationToken = default) where T : class
        {            
            await publishEndpoint.Publish(@event, cancellationToken);
        }
    }
}
