// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Mvc.Behaviors
{
    public interface IRequestException
    {
        bool NeedThrowException(Exception exception)
        {
            return true;
        }
    }
}
