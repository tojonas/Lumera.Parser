using System.Text.RegularExpressions;

namespace Lumera.Parser.Dto
{
    public record AccountNumber
    {
        public string Clearing { get; init; }
        public string Account { get; init; }
        public AccountNumber(string clearing, string account)
        {
            if (clearing.Trim().Length != 4)
            {
                throw new ArgumentException($"Clearing number [{clearing}] is not in a valid format");
            }
            if (account.Trim().Length != 10)
            {
                throw new ArgumentException($"Account number [{account}] is not in a valid format");
            }
            long.Parse(clearing);
            long.Parse(account);
            Clearing = clearing;
            Account = account;
        }
        public static AccountNumber Parse(string value)
        {
            string pattern = @"^(\d{4}) (\d{10})$";
            var match = Regex.Match(value, pattern);
            if (!match.Success)
            {
                throw new ArgumentException($"Account number [{value}] is not in a valid format");
            }
            return new AccountNumber(match.Groups[1].Value, match.Groups[2].Value);
        }

        internal string ToCanonicalString()
        {
            return $"{this.Clearing} {this.Account}";
        }
    }
}
