// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.BankApi.Dto;

using PaymentDto = Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment.Dto.PaymentDto;

namespace Gems.Http.Samples.Logging.AddLogsFromPayload.Payments.CreatePayment
{
    public class CreatePaymentMapper : Profile
    {
        public CreatePaymentMapper()
        {
            this.CreateMap<CreatePaymentResponseDto, PaymentDto>();
        }
    }
}
