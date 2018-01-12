using Khooversoft.Toolbox;

namespace Vault.Contract
{
    public class LessorId : StringType
    {
        public LessorId(string value)
            : base(value, Constants.Sizes.Id)
        {
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator string(LessorId source)
        {
            return source?.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
