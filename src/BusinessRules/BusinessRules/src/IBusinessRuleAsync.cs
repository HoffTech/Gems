// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

namespace Gems.BusinessRules
{
    public interface IBusinessRuleAsync
    {
        Task<(bool IsBroken, string ErrorMessage)> BreakAsync(CancellationToken cancellationToken);
    }

    public interface IBusinessRuleAsync<in TArg0>
    {
        Task<(bool IsBroken, string ErrorMessage)> BreakAsync(TArg0 arg0, CancellationToken cancellationToken);
    }

    public interface IBusinessRuleAsync<in TArg0, in TArg1>
    {
        Task<(bool IsBroken, string ErrorMessage)> BreakAsync(TArg0 arg0, TArg1 arg1, CancellationToken cancellationToken);
    }

    public interface IBusinessRuleAsync<in TArg0, in TArg1, in TArg2>
    {
        Task<(bool IsBroken, string ErrorMessage)> BreakAsync(TArg0 arg0, TArg1 arg1, TArg2 arg2, CancellationToken cancellationToken);
    }

    public interface IBusinessRuleAsync<in TArg0, in TArg1, in TArg2, in TArg3>
    {
        Task<(bool IsBroken, string ErrorMessage)> BreakAsync(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken cancellationToken);
    }

    public interface IBusinessRuleAsync<in TArg0, in TArg1, in TArg2, in TArg3, in TArg4>
    {
        Task<(bool IsBroken, string ErrorMessage)> BreakAsync(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, CancellationToken cancellationToken);
    }
}
