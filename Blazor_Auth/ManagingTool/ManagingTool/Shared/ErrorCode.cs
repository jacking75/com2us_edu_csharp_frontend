public enum ErrorCode : UInt16
{
    None = 0,
    Unauthorized = 1,
    EmptyResponse = 2,

    // Managing Error
    GetUserDataByUserIdFailException = 11001,
    GetUserDataByRangeFailException = 11002,
    GetItemTableFailException = 11003,
    SendMailFailInsertItemIntoMail = 11004,
    SendMailFailException = 11005,
    GetUserItemListFailException = 11006,
    GetUserMailListFailException = 11007,
    RetrieveUserItemFailException = 11008,
    UpdateUserBasicInfoFailException = 11009,

    ManagingGetLoginUserDataException = 11010,
    UpdateRefreshTokenFail = 11011,
    ManagingUpdateRefreshTokenException = 11012,
    InvalidJWTToken = 11013,
    LoginFailed = 11014,

    // Service
    GetUserBasicInfoFail = 12000,
    GetUserItemListFail = 12001,
    GetUserMailListFail = 12002,
    LoginRequestFail = 12003,

}