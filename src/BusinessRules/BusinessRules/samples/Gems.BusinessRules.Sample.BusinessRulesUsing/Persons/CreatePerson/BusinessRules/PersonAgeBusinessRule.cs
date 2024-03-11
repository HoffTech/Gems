// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson.BusinessRules;

public class PersonAgeBusinessRule : IBusinessRule<int>
{
    public bool IsBroken(int age, out string errorMessage)
    {
        if (age < 18)
        {
            errorMessage = "Человек не достиг 18 лет.";

            return true;
        }

        errorMessage = string.Empty;
        return false;
    }
}
