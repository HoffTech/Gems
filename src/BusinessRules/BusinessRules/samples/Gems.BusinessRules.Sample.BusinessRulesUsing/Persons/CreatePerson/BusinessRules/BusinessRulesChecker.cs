// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson.BusinessRules;

public class BusinessRulesChecker
{
    private readonly PersonAgeBusinessRule personAgeBusinessRule;

    public BusinessRulesChecker(PersonAgeBusinessRule personAgeBusinessRule)
    {
        this.personAgeBusinessRule = personAgeBusinessRule;
    }

    public void CheckPersonAge(int age)
    {
        if (this.personAgeBusinessRule.IsBroken(age, out var errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }
    }
}
