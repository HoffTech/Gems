using System.Net;

using Bogus;
using Bogus.DataSets;

namespace Gems.TestInfrastructure.RestTest.Library;

public class FakerLibrary
{
    private readonly Faker faker;

    public FakerLibrary(string locale)
    {
        this.faker = new Faker(locale);
    }

    // Internet.
    public string Email() => this.faker.Internet.Email();

    public string Ip() => this.faker.Internet.Ip();

    public string Ipv6() => this.faker.Internet.Ipv6();

    public string Avatar() => this.faker.Internet.Avatar();

    public string Password() => this.faker.Internet.Password();

    public string Password(int length) => this.faker.Internet.Password(length);

    public string UserAgent() => this.faker.Internet.UserAgent();

    public string UserName() => this.faker.Internet.UserName();

    public string InternetColor() => this.faker.Internet.Color();

    public string Mac() => this.faker.Internet.Mac();

    public string Url() => this.faker.Internet.Url();

    public string DomainName() => this.faker.Internet.DomainName();

    public string DomainSuffix() => this.faker.Internet.DomainSuffix();

    public IPAddress IpAddress() => this.faker.Internet.IpAddress();

    public IPAddress Ipv6Address() => this.faker.Internet.Ipv6Address();

    public string UrlRootedPath() => this.faker.Internet.UrlRootedPath();

    public string UrlWithPath() => this.faker.Internet.UrlWithPath();

    // Random.
    public string String() => this.faker.Random.String();

    public string String(int length) => this.faker.Random.String(length);

    public int Int() => this.faker.Random.Int();

    public int Int(int min, int max) => this.faker.Random.Int(min, max);

    public double Double() => this.faker.Random.Double();

    public double Double(double min, double max) => this.faker.Random.Double(min, max);

    public object OneOf(params object[] elements) => this.faker.Random.CollectionItem(elements);

    public string OneOf(params string[] elements) => this.faker.Random.CollectionItem(elements);

    public double OneOf(params double[] elements) => this.faker.Random.CollectionItem(elements);

    public int OneOf(params int[] elements) => this.faker.Random.CollectionItem(elements);

    public bool OneOf(params bool[] elements) => this.faker.Random.CollectionItem(elements);

    public bool Bool() => this.faker.Random.Bool();

    public Guid Guid() => this.faker.Random.Guid();

    public string Hash() => this.faker.Random.Hash();

    public string Hash(int length) => this.faker.Random.Hash(length);

    public string Hexadecimal() => this.faker.Random.Hexadecimal();

    public string Hexadecimal(int length) => this.faker.Random.Hexadecimal(length);

    public string Replace(string format) => this.faker.Random.Replace(format);

    public string AlphaNumeric(int length) => this.faker.Random.AlphaNumeric(length);

    public string Word() => this.faker.Random.Word();

    public string Words() => this.faker.Random.Words();

    public string Words(int count) => this.faker.Random.Words(count);

    public long Long() => this.faker.Random.Long();

    public long Long(int min, int max) => this.faker.Random.Long(min, max);

    // Date.
    public DateTime Future() => this.faker.Date.Future();

    public DateTime Soon() => this.faker.Date.Soon();

    public DateTime Recent() => this.faker.Date.Recent();

    public DateTime Past() => this.faker.Date.Past();

    public string Weekday() => this.faker.Date.Weekday();

    public string Weekday(bool abbreviation) => this.faker.Date.Weekday(abbreviation);

    public TimeSpan Timespan() => this.faker.Date.Timespan();

    public string TimeZoneString() => this.faker.Date.TimeZoneString();

    public DateOnly RecentDateOnly() => this.faker.Date.RecentDateOnly();

    public DateOnly PastDateOnly() => this.faker.Date.PastDateOnly();

    public DateOnly FutureDateOnly() => this.faker.Date.FutureDateOnly();

    public DateOnly SoonDateOnly() => this.faker.Date.SoonDateOnly();

    public TimeOnly RecentTimeOnly() => this.faker.Date.RecentTimeOnly();

    public TimeOnly SoonTimeOnly() => this.faker.Date.SoonTimeOnly();

    // System.
    public string FileName() => this.faker.System.FileName();

    public string FileName(string ext) => this.faker.System.FileName(ext);

    public string FileExt() => this.faker.System.FileExt();

    public string DirectoryPath() => this.faker.System.DirectoryPath();

    public string FileType() => this.faker.System.FileType();

    public string FilePath() => this.faker.System.FilePath();

    public string MimeType() => this.faker.System.MimeType();

    public string CommonFileExt() => this.faker.System.CommonFileExt();

    public string CommonFileName() => this.faker.System.CommonFileName();

    public string CommonFileType() => this.faker.System.CommonFileType();

    public Exception Exception() => this.faker.System.Exception();

    // Name.
    public string FirstName() => this.faker.Name.FirstName();

    public string LastName() => this.faker.Name.LastName();

    public string FullName() => this.faker.Name.FullName();

    public string JobTitle() => this.faker.Name.JobTitle();

    public string JobArea() => this.faker.Name.JobArea();

    public string JobDescription() => this.faker.Name.JobDescriptor();

    // Phone.
    public string PhoneNumber() => this.faker.Phone.PhoneNumber();

    public string PhoneNumber(string format) => this.faker.Phone.PhoneNumber(format);

    // Address.
    public string BuildingNumber() => this.faker.Address.BuildingNumber();

    public string City() => this.faker.Address.City();

    public string Country() => this.faker.Address.Country();

    public string Direction() => this.faker.Address.Direction();

    public string Direction(bool abbreviation) => this.faker.Address.Direction(abbreviation);

    public string State() => this.faker.Address.State();

    public string CityPrefix() => this.faker.Address.CityPrefix();

    public string CitySuffix() => this.faker.Address.CitySuffix();

    public string FullAddress() => this.faker.Address.FullAddress();

    public string SecondaryAddress() => this.faker.Address.SecondaryAddress();

    public string StreetAddress() => this.faker.Address.StreetAddress();

    public string StateAbbr() => this.faker.Address.StateAbbr();

    public string StreetSuffix() => this.faker.Address.StreetSuffix();

    public string ZipCode() => this.faker.Address.ZipCode();

    public string StreetName() => this.faker.Address.StreetName();

    public string CountryCode() => this.faker.Address.CountryCode();

    public string OrdinalDirection() => this.faker.Address.OrdinalDirection();

    public string OrdinalDirection(bool abbreviation) => this.faker.Address.OrdinalDirection(abbreviation);

    // Commerce.
    public string Color() => this.faker.Commerce.Color();

    public string Price() => this.faker.Commerce.Price();

    public string Price(double min, double max) => this.faker.Commerce.Price(Convert.ToDecimal(min), Convert.ToDecimal(max));

    public string Ean8() => this.faker.Commerce.Ean8();

    public string Ean13() => this.faker.Commerce.Ean13();

    public string Department() => this.faker.Commerce.Department();

    public string Product() => this.faker.Commerce.Product();

    public string ProductAdjective() => this.faker.Commerce.ProductAdjective();

    public string ProductDescription() => this.faker.Commerce.ProductDescription();

    public string ProductMaterial() => this.faker.Commerce.ProductMaterial();

    public string ProductName() => this.faker.Commerce.ProductName();

    public string[] Categories(int num) => this.faker.Commerce.Categories(num);

    public string Category(int num) => this.faker.Random.ArrayElement(this.faker.Commerce.Categories(num));

    // Company.
    public string CompanyName() => this.faker.Company.CompanyName();

    public string CompanySuffix() => this.faker.Company.CompanySuffix();

    public string CatchPhrase() => this.faker.Company.CatchPhrase();

    public string Bs() => this.faker.Company.Bs();

    // Finance.
    public string Account() => this.faker.Finance.Account();

    public string Account(int length) => this.faker.Finance.Account(length);

    public string Bic() => this.faker.Finance.Bic();

    public string Iban() => this.faker.Finance.Iban();

    public string Iban(bool formatted) => this.faker.Finance.Iban(formatted);

    public string AccountName() => this.faker.Finance.AccountName();

    public string RoutingNumber() => this.faker.Finance.RoutingNumber();

    public string CreditCardCvv() => this.faker.Finance.CreditCardCvv();

    public string CreditCardNumber() => this.faker.Finance.CreditCardNumber();

    public string MaestroCreditCardNumber() => this.faker.Finance.CreditCardNumber(CardType.Maestro);

    public string VisaCreditCardNumber() => this.faker.Finance.CreditCardNumber(CardType.Visa);

    public string TransactionType() => this.faker.Finance.TransactionType();

    public double Amount() => Convert.ToDouble(this.faker.Finance.Amount());

    public double Amount(double min, double max) => Convert.ToDouble(this.faker.Finance.Amount(Convert.ToDecimal(min), Convert.ToDecimal(max)));

    public Currency Currency(bool includeFundCodes) => this.faker.Finance.Currency(includeFundCodes);
}
