﻿using System;
using System.Threading.Tasks;
using eAuction.Buyer.Contract.Commands;
using eAuction.Buyer.Domain.BuyerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using static eAuction.Buyer.Contract.Commands.CreateBuyer;

namespace eAuction.Buyer.EndPoint.Handlers
{
    public class CreateBuyerCommandHandler : IConsumer<CreateBuyer.Command>
    {

        readonly ILogger<CreateBuyerCommandHandler> _logger;
        private readonly IBuyerRepository _buyerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="BuyerRepository"></param>
        public CreateBuyerCommandHandler(ILogger<CreateBuyerCommandHandler> logger, IBuyerRepository buyerRepository,
            IPublishEndpoint endpoint)
        {
            _logger = logger;
            _buyerRepository = buyerRepository;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Consumer/Handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<CreateBuyer.Command> context)
        {
            try
            {
                _buyerRepository.Add(context.Message.Buyer);
                await _buyerRepository.UnitOfWork.SaveChangesAsync();
                _logger.LogInformation("Value: {Value}", context.Message);
                await _endpoint.Publish(new CreateBuyer.SuccessEvent(context.Message.CorrelationId, context.Message.Buyer.Id));
            }
            catch (Exception e)
            {
                await _endpoint.Publish(new CreateBuyer.FailedEvent(context.Message.CorrelationId, e.Message));
            }
           
        }
    }
}

