using System;
using System.ComponentModel.DataAnnotations;
using WebAPIServer.DataClass;

namespace WebAPIServer.ReqRes;

public class PurchaseInAppProductRequest : UserAuthRequest
{
    public Int64 UserId { get; set; }
    public Int64 PurchaseId { get; set; }

    [Range(1, int.MaxValue)]
    public Int64 ProductCode { get; set; }
}

public class PurchaseInAppProductResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}