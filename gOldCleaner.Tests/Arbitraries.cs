using System.IO;
using FsCheck;

namespace gOldCleaner.Tests
{
    public static class Arbitraries
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        public static Arbitrary<string> InvalidFileNameGenerator()
        {
            var input = from invalidChar in Gen.Elements(InvalidChars)
                from fname in Arb.Generate<NonEmptyString>()
                select $"{fname}{invalidChar}";

            return input.ToArbitrary();
        }
    }
    
    public static class NonNullOrEmptyStringArbitraries
    {
        public static Arbitrary<string> InvalidFileNameGenerator()
        {
            var input = from fname in Arb.Generate<NonEmptyString>()
                select $"z{fname}";

            return input.ToArbitrary();
        }
    }
}