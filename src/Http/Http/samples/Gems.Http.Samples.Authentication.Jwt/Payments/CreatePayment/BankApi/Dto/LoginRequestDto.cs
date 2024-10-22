// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Dto
{
    public class LoginRequestDto
    {
        public string UserName { get; init; }

        public string Password { get; init; }
    }
}
