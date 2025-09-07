using Common.Contracts.Dtos;
using Common.Contracts.Encryption;
using Logger.Domain.Data;
using Logger.Domain.Entities;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;


namespace Logger.Application.Services
{
    public class ExceptionLoggerService 
    {
        private readonly MongoDbService _mongoDbService;
        private readonly EncryptionHelper _encryptionHelper;


        public ExceptionLoggerService(
            MongoDbService mongoDbService, EncryptionHelper helper)
        {
            _mongoDbService = mongoDbService;
            _encryptionHelper = helper;
        }

        public async Task<List<ExceptionLogEntity>> FindAsync(FilterDefinition<ExceptionLogEntity> filter)
        {
            try
            {
                var logs = await _mongoDbService.ExceptionLogs
                    .Find(filter)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving logs.", ex);
            }
        }

        public async Task<ExceptionLogEntity?> FindByTraceId(string traceId)
        {
            try
            {

                var log = await _mongoDbService.ExceptionLogs
                    .Find(log => log.TraceId == traceId)
                    .FirstOrDefaultAsync();

                return log;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving log by ID.", ex);
            }
        }

        public async Task LogExceptionAsync(ExceptionLogMessage message)
        {
            try
            {
                var logEntry = new ExceptionLogEntity
                {
                    Timestamp = message.Timestamp ?? DateTime.UtcNow,
                    Service = message.Service,
                    Level = message.Level,
                    Message = message.Message,
                    Exception = message.Exception,
                    TraceId = message.TraceId
                };

                await _mongoDbService.ExceptionLogs.InsertOneAsync(logEntry);
            }
            catch (Exception ex)
            {
                throw new Exception("Error logging exception.", ex);
            }
        }

        public async Task LogExceptionSecureAsync(string encryptedText)
        {
            try
            {
                Console.WriteLine($"\n\n\n{encryptedText}\n\n\n");

                var decryptedText = _encryptionHelper.Decrypt(encryptedText);

                Console.WriteLine($"\n\n\n{decryptedText}\n\n\n");

                var message = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<ExceptionLogMessage>(decryptedText);
                var logEntry = new ExceptionLogEntity
                {
                    Timestamp = message.Timestamp ?? DateTime.UtcNow,
                    Service = message.Service,
                    Level = message.Level,
                    Message = message.Message,
                    Exception = message.Exception,
                    TraceId = message.TraceId
                };

                await _mongoDbService.ExceptionLogs.InsertOneAsync(logEntry);
            }
            catch (Exception ex)
            {
                throw new Exception("Error logging exception.", ex);
            }
        }
    }
}
