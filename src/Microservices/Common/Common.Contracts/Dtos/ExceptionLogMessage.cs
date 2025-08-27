using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts.Dtos
{
    public class ExceptionLogMessage : RabbitMQMesage
    {
        public string? Level { get; set; }

        public string? Message { get; set; }

        public string? Exception { get; set; }
    }
}
