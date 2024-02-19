کتابخانه Taxcollector در راستا جامعه کد باز (Open source) و جهت سهولت و سرعت پیاده سازی ارتباط با Api سازمان امور مالیاتی(مودیان) ایجاد شده
این کتابخانه بر پایه SDK موجود در سایت https://www.intamedia.ir و با هدف رفع مشکلات فنی آن توسعه پیدا خواهد کرد

در مرحله اول متدهای زیر رفع مشکل شده و بهبود پیدا کرده

### InquiryByTime
این متد برای دریافت وضعیت فاکتورهای ارسال شده در یک بازه زمانی (حداکثر ۵ روزه ) استفاده می شود
مدل درخواست ارسالی به شکل زیر است :
##### InquiryByTimeRangeDto
```
    public DateTime Start { get; } // ابتدای بازه زمانی از ساعت 00:00:00 تاریخ ورودی
    public DateTime? End { get; } // انتهای بازه زمانی تا ساعت 12:59:59 تاریخ ورودی
    public Pageable? Pageable { get; } // پارمترهای صفحه بندی
    public RequestStatus? Status { get; } // پارمتر وضعیت فاکتور
```
##### RequestStatus (enum)
```
    SUCCESS
    FAILED
    PENDING
    TIMEOUT
```
##### Pageable
با توجه به اینکه حالت پیشفرض تعداد ۱۰ رکورد اول را برمیگرداند، در صورت مقدار دهی این متغیر میتوانید تعداد نتایج را مدیریت کنید 
```
    public int PageNumber { get; } // شماره صفحه
    public int PageSize { get; } // تعداد نتیجه در هر صفحه
```
برای مثال اگر بخواهیم تعداد ۱۰۰۰ فاکتور ثبت شده را بازه زمانی 5 روزه دریافت کنیم میتوانیم از این طریق اقدام کنیم:
```
        for (var i = 0; i < 10; i++)
        {
            var requestModel = new InquiryByTimeRangeDto(
                DateTime.Now.AddDays(-10),
                DateTime.Now.AddDays(-5),
                new Pageable(i, 100)
            );
            var result = await taxApi.InquiryByTimeAsync(requestModel);
        }
```
**حداکثر تعداد رکوردها در هر صفحه نباید بیشتر از ۱۰۰ باشد**

### InquiryByUid
از این متد برای دریافت وضعیت یک یا چند فاکتور با استفاده از شناسه یکتای ثبت فاکتور در تاریخ مشخص استفاده می شود

##### InquiryByUidDto
```
    public List<string> UidList { get; } //لیست شناسه های مالیاتی
    public string FiscalId { get; } // شناسه منحصر بفرد مالیاتی
    public DateTime? Start { get; } // ابتدای بازه زمانی از ساعت 00:00:00 تاریخ ورودی
    public DateTime? End { get; } //انتهای بازه زمانی تا ساعت 12:59:59 تاریخ ورودی
```
- در صورت عدم مقداردهی زمان بصورت پیشفرض جستجو در روز جاری انجام میشود

### InquiryByReferenceId
از این متد برای دریافت وضعیت یک یا چند فاکتور با استفاده از کد مرجع در تاریخ مشخص استفاده می شود

##### InquiryByReferenceNumberDto
```
    public List<string> ReferenceNumbers { get; } //  لیست کدهای مرجع
    public DateTime? Start { get; } // ابتدای بازه زمانی از ساعت 00:00:00 تاریخ ورود
    public DateTime? End { get; } // انتهای بازه زمانی تا ساعت 12:59:59 تاریخ ورودی
```
