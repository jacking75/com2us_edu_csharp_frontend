using WebAPIServer.DataClass;

namespace WebAPIServer.DbOperations;

public interface IRedisDb
{
    public Task<ErrorCode> Init();

    public Task<Tuple<ErrorCode, string>> CreateUserAuthAsync(string email, Int64 accountid);
    public Task<UserAuth> GetUserAuthAsync(Int64 accountid);
    public Task<bool> SetUserReqLockAsync(string userLockKey);
    public Task<bool> DelUserReqLockAsync(string userLockKey);
    public Task<Tuple<ErrorCode, string>> LoadNotification();

    public Task<ErrorCode> CreateStageProgressDataAsync(Int64 userId, Int64 stageCode);
    public Task<ErrorCode> DeleteStageProgressDataAsync(Int64 userId);
    public Task<ErrorCode> ObtainItemAsync(Int64 userId, Int64 itemCode, Int64 itemCount);
    public Task<ErrorCode> KillEnemyAsync(Int64 userId, Int64 enemyCode);
    public Task<Tuple<ErrorCode, List<ItemInfo>, Int64>> CheckStageClearDataAsync(Int64 userId);

    public Task<Tuple<ErrorCode, Int64>> EnterChatLobbyFromLoginAsync(Int64 userId);
    public Task<Tuple<ErrorCode, List<ChatMessage>>> SelectChatLobbyAsync(Int64 userId, Int64 lobbyNum);
    public Task<ErrorCode> SendChatAsync(Int64 userId, string message);
    public Task<Tuple<ErrorCode, List<ChatMessage>>> ReceiveChatAsync(Int64 userId);
}
