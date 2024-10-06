﻿using Authentication.MessageBroker.PublisherModels;
using Event.Entities;
using Event.Persistences;
using MassTransit;

namespace Event.MessageBroker
{
    public class NewUserConsumer : IConsumer<UserPublishEventModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public NewUserConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<UserPublishEventModel> context)
        {
            var newUser = new User
            {
                Id = context.Message.Id,
                Name = context.Message.Name,
                Email = context.Message.Email,
                PhoneNumber = context.Message.PhoneNumber,
                CreatedBy = context.Message.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}