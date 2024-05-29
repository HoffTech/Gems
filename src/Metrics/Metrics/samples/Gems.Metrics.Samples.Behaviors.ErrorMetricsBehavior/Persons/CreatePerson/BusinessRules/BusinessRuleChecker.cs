// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.Filters.Exceptions;

namespace Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson.BusinessRules
{
    public class BusinessRuleChecker(OnlyAdultAgeBusinessRule onlyAdultAgeBusinessRule)
    {
        public void EnsurePersonIsAdult(int age)
        {
            if (onlyAdultAgeBusinessRule.IsBroken(age, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
