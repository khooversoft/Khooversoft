﻿using Khooversoft.Toolbox;

namespace Vault.Contract
{
    public class SecretVersion : StringType
    {
        public SecretVersion(string value)
            : base(value, Constants.Sizes.Name)
        {
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source">source reference</param>
        public static implicit operator string(SecretVersion source)
        {
            return source?.Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool IsValueValid()
        {
            if (!base.IsValueValid())
            {
                return false;
            }

            return Value.IndexOf('/') == -1 &&
                Value.IndexOf('\\') == -1;
        }
    }
}
