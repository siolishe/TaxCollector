using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace TaxCollectData.Library.Factories;

public class PrivateKeyFactory
{
    public RSA ReadPrivateKeyFromFile(string privateKeyPath)
    {
        var privateKeyPemReader = new PemReader(File.OpenText(privateKeyPath));
        var parms = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)privateKeyPemReader.ReadObject());
        var rsa = RSA.Create();
        rsa.ImportParameters(parms);
        return rsa;
    }
}