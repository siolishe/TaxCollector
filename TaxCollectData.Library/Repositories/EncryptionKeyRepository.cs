using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using TaxCollectData.Library.Abstraction.Repositories;
using TaxCollectData.Library.Models;

namespace TaxCollectData.Library.Repositories;

public class EncryptionKeyRepository : IEncryptionKeyRepository
{
    private readonly Func<List<KeyModel>> _builder;
    private readonly Random _random = new();
    private DateTime _expiredTime;
    private RSA _key;
    private string _keyId;

    public EncryptionKeyRepository(Func<List<KeyModel>> builder)
    {
        _builder = builder;
    }

    public RSA GetKey()
    {
        if (NeedRefresh()) Refresh();

        return _key;
    }

    public string GetKeyId()
    {
        if (NeedRefresh()) Refresh();

        return _keyId;
    }

    private void Refresh()
    {
        lock (this)
        {
            if (!NeedRefresh()) return;

            var keyModels = _builder.Invoke();
            var keyModel = keyModels[_random.Next(keyModels.Count)];
            _key = GetPublicKeyFromBase64(keyModel.Key);
            _keyId = keyModel.Id;
            _expiredTime = DateTime.Now.AddHours(1).ToLocalTime();
        }
    }

    private bool NeedRefresh()
    {
        return _expiredTime == null || DateTime.Now.ToLocalTime() > _expiredTime;
    }

    private RSA GetPublicKeyFromBase64(string key)
    {
        var decoded = Base64.Decode(key);
        var asymmetricKeyParameter = PublicKeyFactory.CreateKey(decoded);
        var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)asymmetricKeyParameter);
        var rsa = RSA.Create();
        rsa.ImportParameters(rsaParams);
        return rsa;
    }
}