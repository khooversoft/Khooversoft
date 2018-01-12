using Khooversoft.Toolbox;

namespace Vault.Contract
{
    public class CertificateId : StringType
    {
        public CertificateId(string value)
            : base(value, Constants.Sizes.Id)
        {
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source">source reference</param>
        public static implicit operator string(CertificateId source)
        {
            return source?.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
