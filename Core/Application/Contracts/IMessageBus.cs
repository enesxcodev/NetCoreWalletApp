using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T @event, string queueName, CancellationToken cancellationToken = default) where T : class;
    }
}
