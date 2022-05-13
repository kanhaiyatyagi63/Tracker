using Tracker.Core.Services.Abstractions;

namespace Tracker.Core.Services
{
    public class PasswordGeneratorService : IPasswordGeneratorService
    {
        private int length = 6;
        private bool useLowerCase;
        private bool useUpperCase;
        private bool useNumbers;
        private bool useSpecial;

        public string Generate()
        {
            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@#$%^&*-+?."                        // non-alphanumeric
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (useUpperCase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
            }

            if (useLowerCase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
            }

            if (useNumbers)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
            }

            if (useSpecial)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < length || chars.Distinct().Count() < length; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        public IPasswordGeneratorService WithLength(int length)
        {
            this.length = length;
            return this;
        }
        public IPasswordGeneratorService WithLowerCase(bool useLowerCase)
        {
            this.useLowerCase = useLowerCase;
            return this;
        }
        public IPasswordGeneratorService WithNumbers(bool useNumbers)
        {
            this.useNumbers = useNumbers;
            return this;
        }
        public IPasswordGeneratorService WithSpecials(bool useSpecial)
        {
            this.useSpecial = useSpecial;
            return this;
        }
        public IPasswordGeneratorService WithUpperCase(bool useUpperCase)
        {
            this.useUpperCase = useUpperCase;
            return this;
        }
    }
}
