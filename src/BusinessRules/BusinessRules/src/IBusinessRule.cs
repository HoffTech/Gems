// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.BusinessRules
{
    public interface IBusinessRule
    {
        bool IsBroken(out string errorMessage);
    }

    public interface IBusinessRule<in TArg0>
    {
        bool IsBroken(TArg0 arg0, out string errorMessage);
    }

    public interface IBusinessRule<in TArg0, in TArg1>
    {
        bool IsBroken(TArg0 arg0, TArg1 arg1, out string errorMessage);
    }

    public interface IBusinessRule<in TArg0, in TArg1, in TArg2>
    {
        bool IsBroken(TArg0 arg0, TArg1 arg1, TArg2 arg2, out string errorMessage);
    }

    public interface IBusinessRule<in TArg0, in TArg1, in TArg2, in TArg3>
    {
        bool IsBroken(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, out string errorMessage);
    }

    public interface IBusinessRule<in TArg0, in TArg1, in TArg2, in TArg3, in TArg4>
    {
        bool IsBroken(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, out string errorMessage);
    }
}
