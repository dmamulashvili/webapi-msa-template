using System;
using System.Collections.Generic;
using MSA.Template.SharedKernel;

namespace MSA.Template.Core.OrderAggregate;

public class Address : ValueObject
{
    public String City { get; private set; }    
    public String Street { get; private set; }

    public Address(string city, string street)
    {
        City = city;
        Street = street;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return City;
        yield return Street;
    }
}