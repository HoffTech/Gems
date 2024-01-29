// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gems.Mvc;

public class DelegateConverter<TFrom, TTo, TArg1> : BaseDelegateConverter<TFrom, TTo>
{
    private readonly IConverter<TFrom, TTo, TArg1> converter;
    private readonly TArg1 arg1;

    public DelegateConverter(IConverter<TFrom, TTo, TArg1> converter, TArg1 arg1)
    {
        this.converter = converter;
        this.arg1 = arg1;
    }

    public override TTo Convert(TFrom exception)
    {
        return this.converter.Convert(exception, this.arg1);
    }
}

public class DelegateConverterFactory<TFrom, TTo, TArg1> : BaseDelegateConverterFactory<TFrom, TTo>
{
    private readonly IConverter<TFrom, TTo, TArg1> converter;

    public DelegateConverterFactory(IConverter<TFrom, TTo, TArg1> converter)
    {
        this.converter = converter;
    }

    internal override Type ArgType => typeof(TArg1);

    public override BaseDelegateConverter<TFrom, TTo> CreateDelegateConverter(object arg1)
    {
        return new DelegateConverter<TFrom, TTo, TArg1>(this.converter, (TArg1)arg1);
    }
}

public abstract class BaseDelegateConverterFactory<TFrom, TTo>
{
    internal abstract Type ArgType { get; }

    public abstract BaseDelegateConverter<TFrom, TTo> CreateDelegateConverter(object arg1);
}

public abstract class BaseDelegateConverter<TFrom, TTo>
{
    public abstract TTo Convert(TFrom exception);
}

public class DelegateConverterProvider<TFrom, TTo>
{
    private readonly IEnumerable<BaseDelegateConverterFactory<TFrom, TTo>> converterFactories;

    public DelegateConverterProvider(IEnumerable<BaseDelegateConverterFactory<TFrom, TTo>> converterFactories)
    {
        this.converterFactories = converterFactories;
    }

    public BaseDelegateConverter<TFrom, TTo> GetConverter(object arg)
    {
        var argType = arg.GetType();
        var delegateConverterFactory = this.converterFactories.FirstOrDefault(x => x.ArgType == argType);
        if (delegateConverterFactory == null)
        {
            return null;
        }

        var delegateConverter = delegateConverterFactory.CreateDelegateConverter(arg);
        return delegateConverter;
    }
}
