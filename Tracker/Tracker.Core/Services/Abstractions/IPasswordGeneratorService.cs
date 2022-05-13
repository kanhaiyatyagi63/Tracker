namespace Tracker.Core.Services.Abstractions
{
    public interface IPasswordGeneratorService
    {
        IPasswordGeneratorService WithLowerCase(bool useLowerCase);
        IPasswordGeneratorService WithUpperCase(bool useUppercase);
        IPasswordGeneratorService WithNumbers(bool useNumbers);
        IPasswordGeneratorService WithSpecials(bool useSpecial);
        IPasswordGeneratorService WithLength(int length);
        string Generate();
    }
}
