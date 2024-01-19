using System.Text;
using System.Web;
using TaxCollectData.Library.Abstraction.Properties;
using TaxCollectData.Library.Abstraction.Providers;
using TaxCollectData.Library.Constants;
using TaxCollectData.Library.Dto;

namespace TaxCollectData.Library.Providers;

public class RequestProvider : IRequestProvider
{
    private const string MemoryId = "memoryId";
    private const string EconomicCode = "economicCode";
    private const string ReferenceIds = "referenceIds";
    private const string UidList = "uidList";
    private const string FiscalId = "fiscalId";
    private const string PageSize = "pageSize";
    private const string PageNumber = "pageNumber";
    private const string End = "end";
    private const string Start = "start";
    private const string Status = "status";
    private const string MediaType = "application/json";
    private const string StartDateTimeFormat = "yyyy-MM-ddT00:00:00.000000000+03:30";
    private const string EndDateTimeFormat = "yyyy-MM-ddT23:59:59.123456789+03:30";
    private readonly ISerializer _serializer;
    private readonly IUrlProperties _urlProperties;

    public RequestProvider(IUrlProperties urlProperties, ISerializer serializer)
    {
        _urlProperties = urlProperties;
        _serializer = serializer;
    }

    public HttpRequestMessage GetNonceRequest()
    {
        return new HttpRequestMessage(HttpMethod.Get, _urlProperties.GetUrl(PacketTypeConstants.NONCE));
    }

    public HttpRequestMessage GetServerInformation()
    {
        return new HttpRequestMessage(HttpMethod.Get,
            _urlProperties.GetUrl(PacketTypeConstants.GET_SERVER_INFORMATION));
    }

    public HttpRequestMessage GetInquiryByTimeRequest(InquiryByTimeRangeDto dto)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query[Start] = dto.Start.ToString(StartDateTimeFormat);
        if (dto.End != null) query[End] = dto.End?.ToString(EndDateTimeFormat);

        if (dto.Status != null) query[Status] = dto.Status.ToString();

        if (dto.Pageable != null)
        {
            query[PageNumber] = dto.Pageable.PageNumber.ToString();
            query[PageSize] = dto.Pageable.PageSize.ToString();
        }

        return GetRequestFromQuery(PacketTypeConstants.INQUIRY_BY_TIME_RANGE, query.ToString());
    }


    public HttpRequestMessage GetInquiryByUidRequest(InquiryByUidDto dto)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query[FiscalId] = dto.FiscalId;
        dto.UidList.ForEach(u => query.Add(UidList, u));
        if (dto.Start != null) query[Start] = dto.Start?.ToString(StartDateTimeFormat);
        if (dto.End != null) query[End] = dto.End?.ToString(EndDateTimeFormat);
        return GetRequestFromQuery(PacketTypeConstants.INQUIRY_BY_UID, query.ToString());
    }


    public HttpRequestMessage GetInquiryByReferenceIdRequest(InquiryByReferenceNumberDto dto)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        dto.ReferenceNumbers.ForEach(u => query.Add(ReferenceIds, u));
        if (dto.Start != null) query[Start] = dto.Start?.ToString(StartDateTimeFormat);
        if (dto.End != null) query[End] = dto.End?.ToString(EndDateTimeFormat);
        return GetRequestFromQuery(PacketTypeConstants.INQUIRY_BY_REFERENCE_ID, query.ToString());
    }

    public HttpRequestMessage GetInvoicesRequest(List<PacketDto> data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _urlProperties.GetUrl(PacketTypeConstants.INVOICE));
        request.Content = new StringContent(_serializer.Serialize(data), Encoding.UTF8, MediaType);
        return request;
    }


    public HttpRequestMessage GetTaxpayerRequest(string economicCode)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query[EconomicCode] = economicCode;
        return GetRequestFromQuery(PacketTypeConstants.GET_TAXPAYER, query.ToString());
    }


    public HttpRequestMessage GetFiscalInformationRequest(string memoryId)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query[MemoryId] = memoryId;
        return GetRequestFromQuery(PacketTypeConstants.GET_FISCAL_INFORMATION, query.ToString());
    }

    private HttpRequestMessage GetRequestFromQuery(string requestUrl, string query)
    {
        var uriBuilder = new UriBuilder(_urlProperties.GetUrl(requestUrl))
        {
            Query = query
        };
        return new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());
    }
}