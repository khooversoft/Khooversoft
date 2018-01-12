using Khooversoft.Security;
using System.Security.Cryptography.X509Certificates;

namespace Vault.Contract
{
    public class ObjectData : SecretData
    {
        public ObjectData(byte[] encryptedData)
            : base(encryptedData)
        {
        }

        public ObjectData(X509Certificate2 certificate, byte[] encryptedData)
            : base(certificate, encryptedData)
        {
        }
    }

    public static class ObjectDataExtensions
    {
        public static ObjectData ToObjectData(this SecretData self)
        {
            return new ObjectData(self.Certificate, self.Value);
        }
    }
}
