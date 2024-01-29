// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gems.Mvc;

public class DelegateConverter<TFrom, TTo, TArg1> : BaseDelegateConverter<TFrom, TTo>, IConverter<TFrom, TTo, TArg1>
{
    private readonly IConverter<TFrom, TTo, TArg1> converter;

    public DelegateConverter(IConverter<TFrom, TTo, TArg1> converter)
    {
        this.converter = converter;
    }

    internal override Type ArgType => typeof(TArg1);

    public TTo Convert(TFrom exception, TArg1 arg1)
    {
        return this.converter.Convert(exception, arg1);
    }

    internal override TTo Convert(TFrom exception)
    {
        return this.Convert(exception, (TArg1)this.Arg);
    }
}

public abstract class BaseDelegateConverter<TFrom, TTo> : ICloneable
{
    public object Arg { get; internal set; }

    internal virtual Type ArgType { get; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    internal abstract TTo Convert(TFrom exception);
}

public class DelegateConverterProvider<TFrom, TTo>
{
    private readonly IEnumerable<BaseDelegateConverter<TFrom, TTo>> converters;

    public DelegateConverterProvider(IEnumerable<BaseDelegateConverter<TFrom, TTo>> converters)
    {
        this.converters = converters;
    }

    public BaseDelegateConverter<TFrom, TTo> GetConverter(object arg)
    {
        var delegateConverter = this.converters.FirstOrDefault(x => x.ArgType == arg.GetType());
        if (delegateConverter != null)
        {
            delegateConverter = delegateConverter.Clone() as BaseDelegateConverter<TFrom, TTo>;
        }

        if (delegateConverter != null)
        {
            delegateConverter.Arg = arg;
        }

        return delegateConverter;
    }
}
