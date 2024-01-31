// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using FluentValidation;

using Gems.Mvc.Filters.Exceptions;

namespace Gems.Mvc
{
    public interface IConverter<in TFrom, out TTo>
    {
        TTo Convert(TFrom obj);
    }

    public interface IConverter<in TFrom, out TTo, in TArg1>
    {
        TTo Convert(TFrom obj, TArg1 arg1);
    }

    public interface IConverter<in TFrom, out TTo, in TArg1, in TArg2>
    {
        TTo Convert(TFrom obj, TArg1 arg1, TArg2 arg2);
    }

    public interface IConverter<in TFrom, out TTo, in TArg1, in TArg2, in TArg3>
    {
        TTo Convert(TFrom obj, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface IExceptionConverter<in TFromException, in TRequest> : IConverter<TFromException, Exception, TRequest>
    {
    }

    public interface IValidationExceptionConverter<in TRequest> : IExceptionConverter<ValidationException, TRequest>
    {
    }

    public interface IModelStateValidationExceptionConverter<in TRequest> : IExceptionConverter<ModelStateValidationException, TRequest>
    {
    }
}
