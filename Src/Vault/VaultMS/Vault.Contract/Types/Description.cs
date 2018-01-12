using Khooversoft.Toolbox;

namespace Vault.Contract
{
    public class Description : StringType
    {
        public Description(string value)
            : base(value, Constants.Sizes.Name)
        {
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator string(Description source)
        {
            return source?.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
