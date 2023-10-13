// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;

using FluentValidation;

using Gems.Mvc.Filters.Errors;
using Gems.Mvc.Filters.Exceptions;

using Npgsql;

namespace Gems.Mvc.Filters
{
    public class ExceptionToBusinessErrorViewModelConverter : IConverter<Exception, BusinessErrorViewModel>
    {
        public BusinessErrorViewModel Convert(Exception exception)
        {
            switch (exception)
            {
                case UnauthorizedAccessException e:
                    return new BusinessErrorViewModel(e)
                    {
                        StatusCode = 401
                    };
                case ModelStateValidationException e:
                    return new BusinessErrorViewModel(e.ModelState)
                    {
                        StatusCode = 400
                    };
                case ValidationException e:
                    return new BusinessErrorViewModel(e)
                    {
                        StatusCode = 400
                    };
                case InvalidDataException e:
                    return new BusinessErrorViewModel(e)
                    {
                        StatusCode = 400
                    };
                case System.InvalidOperationException { Source: not null } e when e.Source.StartsWith("System."):
                    return new BusinessErrorViewModel(e, 499);
                case System.InvalidOperationException e:
                    return new BusinessErrorViewModel(e, 422);
                case BusinessException e:
                    return new BusinessErrorViewModel(e);
                case RequestException { StatusCode: { } } e when (int)e.StatusCode < 499:
                    return new BusinessErrorViewModel(e)
                    {
                        StatusCode = 499
                    };
                case RequestException { StatusCode: { } } e when (int)e.StatusCode == 499 ||
                                                                  e.StatusCode == HttpStatusCode.BadGateway ||
                                                                  e.StatusCode == HttpStatusCode.ServiceUnavailable
                                                                  || e.StatusCode == HttpStatusCode.GatewayTimeout:
                    return new BusinessErrorViewModel(e)
                    {
                        StatusCode = 499
                    };
                case { } when (exception.InnerException is SocketException { SocketErrorCode: SocketError.TimedOut }) ||
                              (exception is SocketException { SocketErrorCode: SocketError.TimedOut }) ||
                              (exception is SqlException { Number: -2 }):
                    return new BusinessErrorViewModel(new RequestException(exception.Message, exception, HttpStatusCode.GatewayTimeout))
                    {
                        StatusCode = 499
                    };
                case { } when (exception.InnerException is SocketException) ||
                              (exception is SocketException) ||
                              (exception is PostgresException x && (x.SqlState.StartsWith("08") || x.SqlState.StartsWith("28") || x.SqlState.StartsWith("3D"))) ||
                              (exception is NpgsqlException y && (y.Message.StartsWith("Failed to connect") || y.Message.StartsWith("Exception while reading from stream"))) ||
                              (exception is SqlException z && (z.Number == 53 || z.Number == 87 || z.Number == 11001)):
                    return new BusinessErrorViewModel(new RequestException(exception.Message, exception, HttpStatusCode.BadGateway))
                    {
                        StatusCode = 499
                    };
                default:
                    return new BusinessErrorViewModel(exception)
                    {
                        StatusCode = 499
                    };
            }
        }
    }
}
