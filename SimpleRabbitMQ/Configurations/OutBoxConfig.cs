using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Configurations
{
    public class OutBoxConfig
    {
        public string ConnectionStringDataBase { get; set; } = string.Empty;
        public string CronJobConfig { get; set; } = string.Empty;
    }
}
