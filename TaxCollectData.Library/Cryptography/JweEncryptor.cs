using System.Diagnostics;
using Jose;
using Microsoft.Extensions.Logging;
using TaxCollectData.Library.Abstraction.Cryptography;
using TaxCollectData.Library.Abstraction.Repositories;

namespace TaxCollectData.Library.Cryptography;

public class JweEncryptor : IEncryptor
{
    private readonly ILogger? _logger;
    private readonly IEncryptionKeyRepository _repository;

    public JweEncryptor(IEncryptionKeyRepository repository, ILogger? logger = null)
    {
        _repository = repository;
        _logger = logger;
    }


    public string Encrypt(string text)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var header = new Dictionary<string, object>
            {
                {
                    "kid", _repository.GetKeyId()
                }
            };
            var recipient = new JweRecipient(JweAlgorithm.RSA_OAEP_256, _repository.GetKey(), header);
            return JWE.Encrypt(text, new[] { recipient }, JweEncryption.A256GCM, mode: SerializationMode.Compact);
        }
        finally
        {
            _logger?.LogDebug("encrypt in {} ms", stopwatch.ElapsedMilliseconds);
        }
    }
}