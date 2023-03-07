using ParanaBanco.Domain.Common;
using ParanaBanco.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ParanaBanco.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        private readonly string _emailAddress;

        private Email() { }

        public Email(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length < 5)
                throw new InvalidEmailException();

            _emailAddress = address.ToLower().Trim();
            const string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            if (!Regex.IsMatch(address, pattern))
                throw new InvalidEmailException();
        }

        public string Address
        {
            get { return _emailAddress; }
            private init { _emailAddress = value?.Trim().ToLowerInvariant(); }
        }

        public override string ToString() => Address;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address;
        }
    }
}
