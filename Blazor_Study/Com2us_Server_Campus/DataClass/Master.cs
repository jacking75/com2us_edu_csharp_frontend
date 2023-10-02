namespace WebAPIServer.DataClass;

public class VersionData
{
    public double AppVersion { get; set; }
    public double MasterVersion { get; set; }
}

public class Item
{
    public Int64 Code { get; }
    public string Name { get; }
    public Int64 Attribute { get; }
    public Int64 SellPrice { get; }
    public Int64 BuyPrice { get; }
    public Int64 UseLv { get; }
    public Int64 Attack { get; }
    public Int64 Defence { get; }
    public Int64 Magic { get; }
    public Int64 EnhanceMaxCount { get; }
}

public class ItemAttribute
{
    public string Name { get; set; }
    public Int64 Code { get; set; }
}

public class AttendanceReward
{
    public Int64 Code { get; }
    public Int64 ItemCode { get; }
    public Int64 Count { get; }
}

public class InAppProduct
{
    public Int64 Code { get; }
    public Int64 ItemCode { get; }
    public string ItemName { get; }
    public Int64 ItemCount { get; }
}

public class StageItem
{
    public Int64 Code { get; }
    public Int64 ItemCode { get; }
    public Int64 Count { get; }
}

public class StageEnemy
{
    public Int64 Code { get; }
    public Int64 NpcCode { get; }
    public Int64 Count { get; }
    public Int64 Exp { get; }
}

public class ExpTable
{
    public Int64 Level { get; }
    public Int64 RequireExp { get; }
}