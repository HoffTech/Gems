// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Behaviors
{
    public interface IRequestNotFound
    {
        string GetNotFoundErrorMessage()
        {
            return "Ресурс не найден.";
        }
    }
}
