using Microsoft.EntityFrameworkCore;
using SimpleRabbitMQ.Repository.Context;

namespace SimpleRabbitMQ.Repository
{
    internal class OutboxMessageRepository : BaseRepository<OutboxMessage>, IOutboxMessageRepository
    {
        public OutboxMessageRepository(OutboxMessageDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OutboxMessage>> GetAllPendingAsync()
        {
            return await this._entitySet.Where(x => !x.IsProcessed).ToListAsync();
        }
    }
}
