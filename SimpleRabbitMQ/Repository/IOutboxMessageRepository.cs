using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Repository
{
    internal interface IOutboxMessageRepository
    {
        Task InsertAsync(OutboxMessage entity);
        void  Insert(OutboxMessage entity);
        Task UpdateAsync(OutboxMessage entity);
        Task DeleteAsync(OutboxMessage entity);
        Task<IEnumerable<OutboxMessage>> GetAllPendingAsync();
    }
}
