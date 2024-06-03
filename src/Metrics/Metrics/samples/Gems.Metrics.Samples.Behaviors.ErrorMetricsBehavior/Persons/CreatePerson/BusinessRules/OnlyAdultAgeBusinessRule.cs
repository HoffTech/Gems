// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.BusinessRules;

namespace Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson.BusinessRules
{
    public class OnlyAdultAgeBusinessRule : IBusinessRule<int>
    {
        public bool IsBroken(int age, out string errorMessage)
        {
            if (age < 18)
            {
                errorMessage = "Возраст должен быть больше 18 лет";
                return true;
            }

            errorMessage = string.Empty;
            return false;
        }
    }
}
