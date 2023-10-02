public enum ErrorCode : UInt16
{
    None = 0,

    // Managing Error
    GetUserDataByUserIdFailException = 11001,
    GetUserDataByRangeFailException = 11002,
    GetItemTableFailException = 11003,
    SendMailFailInsertItemIntoMail = 11004,
    SendMailFailException = 11005,
    GetUserItemListFailException = 11006,
    GetUserMailListFailException = 11007,
    RetrieveUserItemFailException = 11008,
    UpdateUserBasicInfoFailException = 11009
}