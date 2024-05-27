// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Http.Samples.Authentication.Jwt.Payments.CreatePayment.BankApi.Options
{
    public class BankApiOptions : HttpClientServiceOptions
    {
        /// <summary>
        /// Section in appsettings.json.
        /// </summary>
        public const string Name = "Payments:CreatePayment:BankApi";

        /// <summary>
        /// Логин Jwt аутентификации.
        /// </summary>
        public string UserName { get; init; }

        /// <summary>
        /// Пароль Jwt аутентификации.
        /// </summary>
        public string Password { get; init; }
    }
}
