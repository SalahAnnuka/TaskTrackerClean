using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts.Dtos
{
    public class RabbitMQMesage
    {
        public DateTime? Timestamp { get; set; }
        public string? Service { get; set; }
        public string? TraceId { get; set; }

    }
}
