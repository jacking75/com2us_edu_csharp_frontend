using WebAPIServer.ReqRes;
using WebAPIServer.DataClass;
using WebAPIServer.Util;
using ZLogger;
using SqlKata.Execution;

namespace WebAPIServer.DbOperations;

public partial class MasterDb : IMasterDb
{
    public async Task<Tuple<ErrorCode, GetItemTableResponse>> GetItemTableAsync()
    {
        try
        {
            var response = new GetItemTableResponse();

            response.Item_Weapon = ItemInfo.Where(item => item.Attribute == 1)
                                   .Select(item => new ItemAttribute
                                   {
                                       Name = item.Name,
                                       Code = item.Code
                                   }).ToList();

			response.Item_Armor = ItemInfo.Where(item => item.Attribute == 2)
								   .Select(item => new ItemAttribute
								   {
									   Name = item.Name,
									   Code = item.Code
								   }).ToList();

			response.Item_Clothes = ItemInfo.Where(item => item.Attribute == 3)
								   .Select(item => new ItemAttribute
								   {
									   Name = item.Name,
									   Code = item.Code
								   }).ToList();

			response.Item_MagicTool = ItemInfo.Where(item => item.Attribute == 4)
								   .Select(item => new ItemAttribute
								   {
									   Name = item.Name,
									   Code = item.Code
								   }).ToList();

			return new Tuple<ErrorCode, GetItemTableResponse>(ErrorCode.None, response);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.GetItemTableFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "GetUserDataByRange Exception");

            return new Tuple<ErrorCode, GetItemTableResponse>(errorCode, null);
        }   
    }
}