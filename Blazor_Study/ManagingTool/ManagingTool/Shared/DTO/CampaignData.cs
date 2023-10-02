namespace ManagingTool.Shared.DTO;

public class Campaign
{
    public string Title { get; set; }
    public string Content { get; set; }
    public Int64 CampaignType { get; set; }
    public double? BoostValue { get; set; }
    public Int64? CampaignItemCode { get; set; }
    public Int64? CampaignItemCount { get; set; }
}
