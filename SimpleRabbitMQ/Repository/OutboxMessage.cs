using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Repository
{
    public class OutboxMessage
    {
        public long Id { get; set; }

        public string MessageData { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; private set; }

        public bool IsProcessed { get; private set; } = false;

        public string ExchangeName { get; set; }

        public string ConnectionName { get; set; }

        public string RoutingKey { get; set; }

        public void UpdateProcess() { 
            IsProcessed= true;
            ModifiedAt= DateTime.UtcNow;
        }
    }
}
