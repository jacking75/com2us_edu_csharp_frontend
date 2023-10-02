namespace ManagingTool.Shared.DTO;

public class GetItemTableRequest
{

}
public class GetItemTableResponse
{
    public List<ItemAttribute> Item_Weapon { get; set; }
    public List<ItemAttribute> Item_Armor { get; set; }
    public List<ItemAttribute> Item_Clothes { get; set; }
    public List<ItemAttribute> Item_MagicTool { get; set; }
}

public class ItemAttribute
{
    public string Name { get; set; }
    public Int64 Code { get; set; }
}

public class GetUserItemListRequest
{
    public string SearchType { get; set; }  
    public Int64 SearchValue { get; set; }
}
public class GetUserItemListResponse
{
    public ErrorCode errorCode { get; set; }
    public List<UserItem> UserItem { get; set; }
}

public class UserItem
{
    public Int64 ItemId { get; set; }
    public Int64 UserId { get; set; }
    public Int64 ItemCode { get; set; }
    public Int64 ItemCount { get; set; }
    public Int64 Attack { get; set; }
    public Int64 Defence { get; set; }
    public Int64 Magic { get; set; }
    public Int64 EnhanceCount { get; set; }
    public bool IsDestroyed { get; set; }
    public DateTime ObtainedAt { get; set; }
}

public class RetrieveUserItemRequest
{
    public List<Tuple<Int64, Int64>> SelectedItemList { get; set; }
    public MailForm? MailForm { get; set; }
}
public class RetrieveUserItemResponse
{
    public ErrorCode errorCode { get; set; }
}

