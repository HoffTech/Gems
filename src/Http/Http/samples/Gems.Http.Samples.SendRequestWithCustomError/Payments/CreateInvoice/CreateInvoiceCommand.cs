﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

namespace Gems.Http.Samples.SendRequestWithCustomError.Payments.CreateInvoice
{
    public record CreateInvoiceCommand : IRequest
    {
    }
}