using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger.Application.Dtos
{
    public class ExceptionLogDto
    {
        public DateTime? Timestamp { get; set; }

        public string? Service { get; set; }

        public string? Level { get; set; }

        public string? Message { get; set; }

        public string? Exception { get; set; }

        public string? TraceId { get; set; }
    }
}
