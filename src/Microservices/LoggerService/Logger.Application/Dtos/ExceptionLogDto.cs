﻿namespace Logger.Application.Dtos
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
