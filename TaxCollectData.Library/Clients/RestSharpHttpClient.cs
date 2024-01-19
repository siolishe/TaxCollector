using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TaxCollectData.Library.Abstraction.Clients;
using TaxCollectData.Library.Abstraction.Cryptography;
using TaxCollectData.Library.Abstraction.Properties;
using TaxCollectData.Library.Abstraction.Providers;
using TaxCollectData.Library.Models;
using TaxCollectData.Library.Properties;

namespace TaxCollectData.Library.Clients;

public class RestSharpHttpClient : IClient
{
    private const string Bearer = "Bearer ";
    private const string MediaType = "application/json";
    private const string Charset = "utf-8";
    private readonly HttpClient _httpClient;
    private readonly IHttpHeadersProperties _httpHeadersProperties;
    private readonly ILogger? _logger;
    private readonly ISerializer _serializer;
    private readonly ISignatory _signatory;
    private readonly TaxProperties _taxProperties;

    public RestSharpHttpClient(HttpClient httpClient,
        TaxProperties taxProperties,
        ISignatory signatory,
        IHttpHeadersProperties httpHeadersProperties,
        ISerializer serializer,
        ILogger? logger = null)
    {
        _httpClient = httpClient;
        _taxProperties = taxProperties;
        _signatory = signatory;
        _httpHeadersProperties = httpHeadersProperties;
        _serializer = serializer;
        _logger = logger;
    }

    public async Task<T> SendRequestAsync<T>(HttpRequestMessage request, HttpRequestMessage nonceRequest)
    {
        var authenticatedRequest = await GetAuthenticatedRequestAsync(request, nonceRequest).ConfigureAwait(false);
        return await SendRequestAsync<T>(authenticatedRequest).ConfigureAwait(false);
    }

    private async Task<T> SendRequestAsync<T>(HttpRequestMessage request)
    {
        foreach (var customHeader in _httpHeadersProperties.CustomHeaders)
            request.Headers.Add(customHeader.Key, customHeader.Value);

        try
        {
            _logger?.LogDebug("call {}", request.RequestUri.ToString());

            var response = await _httpClient.SendAsync(request);
            _logger?.LogDebug("call {}", response.Content.ReadAsStreamAsync());
            return await response.Content.ReadFromJsonAsync<T>()
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<HttpRequestMessage> GetAuthenticatedRequestAsync(HttpRequestMessage request,
        HttpRequestMessage nonceRequest)
    {
        var signedNonce = await GetSignedNonceAsync(nonceRequest).ConfigureAwait(false);
        request.Headers.Add(_httpHeadersProperties.AuthorizationHeaderName, signedNonce);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
        request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue(Charset));
        return request;
    }

    private async Task<string> GetSignedNonceAsync(HttpRequestMessage request)
    {
        var nonce = await SendRequestAsync<NonceEntity>(request).ConfigureAwait(false);
        var tokenModel = new TokenModel(nonce.Nonce, _taxProperties.MemoryId);
        return $"{Bearer} {_signatory.Sign(_serializer.Serialize(tokenModel))}";
    }
}