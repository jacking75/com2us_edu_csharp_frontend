using System;
using System.Text.Json;
using CloudStructures.Structures;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using WebAPIServer.DataClass;
using WebAPIServer.Util;
using ZLogger;

namespace WebAPIServer.DbOperations;

public partial class RedisDb : IRedisDb
{
    const int CHAT_MAX_LENGTH = 100;

    // 로그인시 채팅로비 접속
    // 로비에 접속중인 인원수 변경 및 유저가 접속한 로비정보 저장
    public async Task<Tuple<ErrorCode, Int64>> EnterChatLobbyFromLoginAsync(Int64 userId)
    {
        try
        {
            // 로직에 따라 접속할 로비 선택
            (var errorCode, var lobbyNum) = await SelectChatLobbyAsync();
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, Int64>(errorCode, 0);
            }

            // 유저가 접속한 로비정보 저장
            errorCode = await UpdateUserLobbyData(userId, lobbyNum);
            if (errorCode != ErrorCode.None)
            {
                // 롤백
                await EnterChatLobbyFromLoginRollBack(lobbyNum);

                return new Tuple<ErrorCode, Int64>(errorCode, 0);
            }

            return new Tuple<ErrorCode, Int64>(ErrorCode.None, lobbyNum);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.EnterChatLobbyFromLoginFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "EnterChatLobbyFromLogin Exception");

            return new Tuple<ErrorCode, Int64>(errorCode, 0);
        }
    }

    private async Task<Tuple<ErrorCode,Int64>> SelectChatLobbyAsync()
    {
        var key = GenerateKey.LobbyUserCountKey();

        var script = @"local lobbyNum = redis.call('ZRANGEBYSCORE', KEYS[1], 0, 70, 'LIMIT', 0, 1)
                       if #lobbyNum == 0 then
                            lobbyNum = redis.call('ZREVRANGEBYSCORE', KEYS[1], 71, 99, 'LIMIT', 0, 1)
                            if #lobbyNum == 0 then
                                return {0}
                            end
                       else
                            redis.call('ZINCRBY', KEYS[1], 1, lobbyNum[1])
                            return {lobbyNum[1]}
                       end";

        var luaScript = new RedisLua(_redisConn, key);
        var luaScriptResult = await luaScript.ScriptEvaluateAsync<Int64>(script, new RedisKey[] { "LobbyUserCount" });

        var lobbyNum = luaScriptResult.Value;
        if (lobbyNum == 0)
        {
            return new Tuple<ErrorCode, Int64>(ErrorCode.EnterChatLobbyFromLoginFailLobbyFull, 0);
        }

        return new Tuple<ErrorCode, Int64>(ErrorCode.None, lobbyNum);
    }

    private async Task<ErrorCode> UpdateUserLobbyData(Int64 userId, Int64 lobbyNum)
    {
        var key = GenerateKey.LobbyUserListKey();

        var lobbyUserListRedis = new RedisDictionary<Int64, Int64>(_redisConn, key, null);

        await lobbyUserListRedis.SetAsync(userId, lobbyNum, TimeSpan.FromDays(1));

            /*
        if (await lobbyUserListRedis.SetAsync(userId, lobbyNum, TimeSpan.FromDays(1)) == false)
        {
            return ErrorCode.SelectChatLobbyFailRedis;
        }
            */

        return ErrorCode.None;
    }

    private async Task EnterChatLobbyFromLoginRollBack(Int64 lobbyNum)
    {
        var lobbyUserCountRedis = new RedisSortedSet<Int64>(_redisConn, "LobbyUserCount", null);

        await lobbyUserCountRedis.DecrementAsync(lobbyNum, 1);
    }

    // 채팅로비 지정 접속
    // 기존 데이터 업데이트
    public async Task<Tuple<ErrorCode, List<ChatMessage>>> SelectChatLobbyAsync(Int64 userId, Int64 newLobbyNum)
    {
        try
        {
            // 유저가 현재 접속중인 로비 정보 가져오기
            (var errorCode, var userLobbyNum) = await LoadUserCurrentLobby(userId);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, null);
            }

            if (userLobbyNum == newLobbyNum)
            {
                return new Tuple<ErrorCode, List<ChatMessage>>(ErrorCode.SelectChatLobbyFailAlreadyIn, null);
            }

            // 지정한 로비로 접속
            errorCode = await EnterChatLobbyFromSelect(userLobbyNum, newLobbyNum);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, null);
            }

            // 유저가 새로 접속한 로비정보 저장 
            errorCode = await UpdateUserLobbyData(userId, newLobbyNum);
            if (errorCode != ErrorCode.None)
            {
                // 롤백
                await SelectChatLobbyRollBack(userLobbyNum, newLobbyNum);

                return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, null);
            }

            // 접속한 로비의 채팅기록 가져오기
            (errorCode, var chatHistory) = await ReceiveChatAsync(userId);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, null);
            }

            return new Tuple<ErrorCode, List<ChatMessage>>(ErrorCode.None, chatHistory);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.SelectChatLobbyFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "SelectChatLobby Exception");

            return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, null);
        }
    }

    private async Task<Tuple<ErrorCode, Int64>> LoadUserCurrentLobby(Int64 userId)
    {
        var key = GenerateKey.LobbyUserListKey();

        var lobbyUserListRedis = new RedisDictionary<Int64, Int64>(_redisConn, key, null);
        var userLobbyNumRedisResult = await lobbyUserListRedis.GetAsync(userId);

        if (userLobbyNumRedisResult.HasValue == false)
        {
            return new Tuple<ErrorCode, Int64>(ErrorCode.LoadUserCurrentLobbyFailWrongUser, 0);
        }

        var userLobbyNum = userLobbyNumRedisResult.Value; 

        return new Tuple<ErrorCode, Int64>(ErrorCode.None, userLobbyNum);
    }

    private async Task<ErrorCode> EnterChatLobbyFromSelect(Int64 userLobbyNum, Int64 newLobbyNum)
    {
        var key = GenerateKey.LobbyUserCountKey();

        var script = @"local lobbyUserCount = redis.call('ZSCORE', KEYS[1], ARGV[2])
                       if lobbyUserCount and tonumber(lobbyUserCount) > 99 then
                            return { false }
                       else
                            redis.call('ZINCRBY', KEYS[1], -1, ARGV[1])
                            redis.call('ZINCRBY', KEYS[1], 1, ARGV[2])
                            return { true }
                       end";

        var luaScript = new RedisLua(_redisConn, key);
        var luaScriptResult = await luaScript.ScriptEvaluateAsync<bool>(script, new RedisKey[] { "LobbyUserCount" }, new RedisValue[] { userLobbyNum, newLobbyNum });

        if (luaScriptResult.Value == false)
        {
            return ErrorCode.SelectChatLobbyFailLobbyFull;
        }

        return ErrorCode.None;
    }

    private async Task SelectChatLobbyRollBack(Int64 userLobbyNum, Int64 newLobbyNum)
    {
        var key = GenerateKey.LobbyUserCountKey();
        var lobbyUserCountRedis = new RedisSortedSet<Int64>(_redisConn, key, null);

        await lobbyUserCountRedis.IncrementAsync(userLobbyNum, 1);
        await lobbyUserCountRedis.DecrementAsync(newLobbyNum, 1);
    }

    // 채팅 메시지 전송
    // RedisStream 형식 활용
    public async Task<ErrorCode> SendChatAsync(Int64 userId, string message)
    {
        try
        {
            // 유저가 현재 접속중인 로비 정보 가져오기
            (var errorCode, var userLobbyNum) = await LoadUserCurrentLobby(userId);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            // 송신한 메시지 레디스에 저장
            (errorCode, var id) = await AddLobbyChat(userLobbyNum, userId, message);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }         

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.SendChatFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "SendChat Exception");

            return errorCode;
        }
    }

    private async Task<Tuple<ErrorCode, string>> AddLobbyChat(Int64 lobbyNum, Int64 userId, string message)
    {
        var key = GenerateKey.LobbyChatKey(lobbyNum);
        var newChat = new ChatMessage { UserId = userId, TimeStamp = DateTime.Now, Message = message };

        var id = await _redisConnBySE.StreamAddAsync(
            key: key,
            streamField: "UserMsg",
            streamValue: JsonSerializer.Serialize(newChat),
            maxLength: CHAT_MAX_LENGTH);

        if (id.HasValue == false)
        {
            return new Tuple<ErrorCode, string>(ErrorCode.SendChatFailRedis, null);
        }

        return new Tuple<ErrorCode, string>(ErrorCode.None, id.ToString());
    }

    // 채팅 메시지 가져오기
    // 채팅은 RedisStream에서 부여되는 id순으로 정렬되어있음
    public async Task<Tuple<ErrorCode, List<ChatMessage>>> ReceiveChatAsync(Int64 userId)
    {
        var chatHistory = new List<ChatMessage>();

        try
        {
            // 유저가 현재 접속중인 로비 정보 가져오기
            (var errorCode, var userLobbyNum) = await LoadUserCurrentLobby(userId);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, chatHistory);
            }

            // 레디스에서 채팅 기록 가져오기
            chatHistory = await LoadLobbyChat(userLobbyNum);
           
            return new Tuple<ErrorCode, List<ChatMessage>>(ErrorCode.None, chatHistory);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.ReceiveChatFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ReceiveChat Exception");

            return new Tuple<ErrorCode, List<ChatMessage>>(errorCode, null);
        }
    }

    private async Task<List<ChatMessage>> LoadLobbyChat(Int64 lobbyNum)
    {
        var key = GenerateKey.LobbyChatKey(lobbyNum);
        var streamResult = await _redisConnBySE.StreamRangeAsync(key, "-", "+", 50, Order.Descending);

        var chatHistory = new List<ChatMessage>();

        foreach (var entry in streamResult)
        {
            var entryData = entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value);

            chatHistory.Add(JsonSerializer.Deserialize<ChatMessage>(entryData["UserMsg"]));
        }

        return chatHistory;
    }
}