// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Data.Behaviors
{
    public interface IRequestUnitOfWork
    {
        bool IsSkip()
        {
            return false;
        }
    }
}
