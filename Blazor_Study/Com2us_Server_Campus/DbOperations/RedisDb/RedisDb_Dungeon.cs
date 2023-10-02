using System;
using CloudStructures.Structures;
using Org.BouncyCastle.Asn1.Pkcs;
using StackExchange.Redis;
using WebAPIServer.DataClass;
using WebAPIServer.Util;
using ZLogger;

namespace WebAPIServer.DbOperations;

public partial class RedisDb : IRedisDb
{
    // 스테이지 진행 정보 생성
    // UserId로 키 생성
    public async Task<ErrorCode> CreateStageProgressDataAsync(Int64 userId, Int64 stageCode)
    {
        try
        {
            // 이미 스테이지 진행중인지 여부 검증
            var errorCode = await CheckStageInProgress(userId, stageCode);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            // 드롭 아이템 리스트 생성
            await CreateObtainedItemList(userId, stageCode);

            // 처치한 적 리스트 생성
            await CreateKilledEnemyList(userId, stageCode);

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.CreateStageProgressDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "CreateStageProgressData Exception");

            return errorCode;
        }
    }

    private async Task<ErrorCode> CheckStageInProgress(Int64 userId, Int64 stageCode)
    {
        var stageUserKey = GenerateKey.StageUserKey(userId);

        var stageRedis = new RedisString<Int64>(_redisConn, stageUserKey, DungeonKeyTimeSpan());
        var stageRedisResult = await stageRedis.GetAsync();

        if (stageRedisResult.HasValue == true)
        {
            if (stageRedisResult.Value == stageCode)
            {
                return ErrorCode.CreateStageProgressDataFailInProgress;
            }
            else
            {
                await DeleteStageProgressDataAsync(userId);

                return ErrorCode.CreateStageProgressDataFailAlreadyInDifferentStage;
            }
        }
        else
        {
            if (await stageRedis.SetAsync(stageCode) == false)
            {
                return ErrorCode.CreateStageProgressDataFailRedis;
            }
        }

        return ErrorCode.None;
    }

    private async Task CreateObtainedItemList(Int64 userId, Int64 stageCode)
    {
        var stageItemKey = GenerateKey.StageItemKey(userId);

        var itemRedis = new RedisDictionary<Int64, ObtainedStageItem>(_redisConn, stageItemKey, DungeonKeyTimeSpan());

        var itemList = new List<KeyValuePair<Int64, ObtainedStageItem>>();
        foreach (StageItem itemData in _masterDb.StageItemInfo.FindAll(i => i.Code == stageCode))
        {
            itemList.Add(new KeyValuePair<Int64, ObtainedStageItem>(itemData.ItemCode, new ObtainedStageItem { MaxCount = itemData.Count }));
        }

        await itemRedis.SetAsync(itemList);
    }

    private async Task CreateKilledEnemyList(Int64 userId, Int64 stageCode)
    {
        var stageEnemyKey = GenerateKey.StageEnemyKey(userId);

        var enemyRedis = new RedisDictionary<Int64, KilledStageEnemy>(_redisConn, stageEnemyKey, DungeonKeyTimeSpan());

        var enemyList = new List<KeyValuePair<Int64, KilledStageEnemy>>();
        foreach (StageEnemy enemyData in _masterDb.StageEnemyInfo.FindAll(i => i.Code == stageCode))
        {
            enemyList.Add(new KeyValuePair<Int64, KilledStageEnemy>(enemyData.NpcCode, new KilledStageEnemy { GoalCount = enemyData.Count }));
        }

        await enemyRedis.SetAsync(enemyList);
    }

    // 스테이지 진행 정보 삭제
    // UserId에 해당하는 키 제거
    public async Task<ErrorCode> DeleteStageProgressDataAsync(Int64 userId)
    {
        var stageUserKey = GenerateKey.StageUserKey(userId);
        var stageItemKey = GenerateKey.StageItemKey(userId);
        var stageEnemyKey = GenerateKey.StageEnemyKey(userId);

        try
        {
            var stageRedis = new RedisString<Int64>(_redisConn, stageUserKey, null);
            var itemRedis = new RedisDictionary<Int64, ObtainedStageItem>(_redisConn, stageItemKey, null);
            var enemyRedis = new RedisDictionary<Int64, KilledStageEnemy>(_redisConn, stageEnemyKey, null);

            if (await stageRedis.DeleteAsync() == false || await itemRedis.DeleteAsync() == false | await enemyRedis.DeleteAsync() == false)
            {
                return ErrorCode.DeleteStageProgressDataFailRedis;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.DeleteStageProgressDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "DeleteStageProgressData Exception");

            return errorCode;
        }
    }

    // 스테이지 아이템 획득
    // 유저의 stageItemKey에 아이템 추가
    public async Task<ErrorCode> ObtainItemAsync(Int64 userId, Int64 itemCode, Int64 itemCount)
    {
        try
        {
            // 유저가 스테이지 진행중인지 확인
            (var errorCode, var stageCode) = await CheckUserInProgress(userId);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            // 아이템 획득 처리
            errorCode = await UpdateObtainedItem(userId, itemCode, itemCount);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.ObtainItemFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ObtainItem Exception");

            return errorCode;
        }
    }

    private async Task<Tuple<ErrorCode, Int64>> CheckUserInProgress(Int64 userId)
    {
        var stageUserKey = GenerateKey.StageUserKey(userId);

        var stageRedis = new RedisString<Int64>(_redisConn, stageUserKey, null);
        var stageRedisResult = await stageRedis.GetAsync();

        if (stageRedisResult.HasValue == false)
        {
            return new Tuple<ErrorCode, Int64>(ErrorCode.CheckUserInProgressFailWrongKey, 0);
        }

        return new Tuple<ErrorCode, Int64>(ErrorCode.None, stageRedisResult.Value);
    }

    private async Task<ErrorCode> UpdateObtainedItem(Int64 userId, Int64 itemCode, Int64 itemCount)
    {
        var stageItemKey = GenerateKey.StageItemKey(userId);

        var obtainedItemRedis = new RedisDictionary<Int64, ObtainedStageItem>(_redisConn, stageItemKey, null);
        var obtainedItemRedisResult = await obtainedItemRedis.GetAsync(itemCode);

        if (obtainedItemRedisResult.HasValue == false)
        {
            return ErrorCode.ObtainItemFailWrongItem;
        }

        var obtainedItem = obtainedItemRedisResult.Value;
        obtainedItem.ObtainedCount += itemCount;

        if (obtainedItem.ObtainedCount > obtainedItem.MaxCount)
        {
            return ErrorCode.ObtainItemFailToManyItem;
        }

        await obtainedItemRedis.SetAsync(itemCode, obtainedItem);

        return ErrorCode.None;
    }


    // 스테이지 적 제거 
    // 유저의 stageEnemyKey에 적 추가
    public async Task<ErrorCode> KillEnemyAsync(Int64 userId, Int64 enemyCode)
    {
        try
        {
            // 유저가 스테이지 진행중인지 확인
            (var errorCode, var stageCode) = await CheckUserInProgress(userId);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            // 적 제거 정보 처리
            errorCode = await UpdateKilledEnemy(userId, enemyCode);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.KillEnemyFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "KillEnemy Exception");

            return errorCode;
        }
    }

    private async Task<ErrorCode> UpdateKilledEnemy(Int64 userId, Int64 enemyCode)
    {
        var stageEnemyKey = GenerateKey.StageEnemyKey(userId);

        var killedEnemyRedis = new RedisDictionary<Int64, KilledStageEnemy>(_redisConn, stageEnemyKey, null);
        var killedEnemyRedisResult = await killedEnemyRedis.GetAsync(enemyCode);

        if (killedEnemyRedisResult.HasValue == false)
        {
            return ErrorCode.KillEnemyFailWrongEnemy;
        }

        var killedEnemy = killedEnemyRedisResult.Value;
        killedEnemy.KilledCount++;

        if (killedEnemy.KilledCount > killedEnemy.GoalCount)
        {
            return ErrorCode.killEnemyFailToManyEnemy;
        }

        await killedEnemyRedis.SetAsync(enemyCode, killedEnemy, null);

        return ErrorCode.None;
    }

    // 스테이지 클리어 확인
    // MasterData의 StageEnemy 데이터와 redis에 저장해놓은 데이터 비교 
    public async Task<Tuple<ErrorCode, List<ItemInfo>, Int64>> CheckStageClearDataAsync(Int64 userId)
    {
        try
        {
            // 유저가 스테이지 진행중인지 확인
            (var errorCode, var stageCode) = await CheckUserInProgress(userId);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ItemInfo>, Int64>(errorCode, null, 0);
            }

            // 모든 적 처치 여부 확인
            errorCode = await CheckAllEnemiesEliminated(userId, stageCode);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ItemInfo>, Int64>(errorCode, null, 0);
            }

            // 획득 아이템 정보 가져오기
            var itemList = await LoadObtainedItemList(userId);

            return new Tuple<ErrorCode, List<ItemInfo>, Int64>(ErrorCode.None, itemList, stageCode);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.CheckStageClearDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "CheckStageClearData Exception");

            return new Tuple<ErrorCode, List<ItemInfo>, Int64>(errorCode, null, 0);
        }
    }

    private async Task<ErrorCode> CheckAllEnemiesEliminated(Int64 userId, Int64 stageCode)
    {
        var stageEnemyKey = GenerateKey.StageEnemyKey(userId);

        var enemyRedis = new RedisDictionary<Int64, KilledStageEnemy>(_redisConn, stageEnemyKey, null);
        var enemyList = await enemyRedis.GetAllAsync();

        foreach (StageEnemy stageEnemy in _masterDb.StageEnemyInfo.FindAll(i => i.Code == stageCode))
        {
            var isRightEnemy = enemyList.FirstOrDefault(i => i.Key == stageEnemy.NpcCode && i.Value.GoalCount == stageEnemy.Count);

            if (isRightEnemy.Equals(default(KeyValuePair<Int64, KilledStageEnemy>)))
            {
                return ErrorCode.CheckStageClearDataFailNotComplete;
            }
        }

        return ErrorCode.None;
    }

    private async Task<List<ItemInfo>> LoadObtainedItemList(Int64 userId)
    {
        var stageItemKey = GenerateKey.StageItemKey(userId);

        var itemRedis = new RedisDictionary<Int64, ObtainedStageItem>(_redisConn, stageItemKey, null);

        var itemDictionary = await itemRedis.GetAllAsync();
        var itemList = new List<ItemInfo>();

        foreach (KeyValuePair<Int64, ObtainedStageItem> item in itemDictionary)
        {
            if (item.Value.ObtainedCount > 0)
            {
                itemList.Add(new ItemInfo { ItemCode = item.Key, ItemCount = item.Value.ObtainedCount });
            }
        }

        return itemList;
    }
}

