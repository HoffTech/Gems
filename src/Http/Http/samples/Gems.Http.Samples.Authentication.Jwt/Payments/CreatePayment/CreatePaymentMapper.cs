// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Dto;
using Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.Dto;

namespace Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment
{
    public class CreatePaymentMapper : Profile
    {
        public CreatePaymentMapper()
        {
            this.CreateMap<CreatePaymentResponseDto, PaymentDto>();
        }
    }
}
