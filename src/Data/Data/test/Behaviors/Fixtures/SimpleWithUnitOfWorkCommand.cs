// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;

using MediatR;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class SimpleWithUnitOfWorkCommand : IRequest, IRequestUnitOfWork
{
}
