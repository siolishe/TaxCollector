using System.Security.Cryptography.X509Certificates;

namespace TaxCollectData.Library.Factories;

public class X509CertificateFactory
{
    public X509Certificate ReadCertificateFromFile(string certificatePath)
    {
        return new X509Certificate(File.ReadAllBytes(certificatePath));
    }
}