using System;
using SqlKata.Execution;
using MySqlConnector;
using WebAPIServer.DataClass;
using ZLogger;
using WebAPIServer.Util;

namespace WebAPIServer.DbOperations;

public partial class GameDb : IGameDb
{
    // 던전 스테이지 로딩
    // ClearStage 테이블에서 클리어한 스테이지 정보 가져오기 
    public async Task<Tuple<ErrorCode, List<ClearData>>> LoadStageListAsync(Int64 userId)
    {
        try
        {
            var clearStage = await _queryFactory.Query("User_ClearStage").Where("UserId", userId)
                                                .GetAsync<ClearData>() as List<ClearData>;

            return new Tuple<ErrorCode, List<ClearData>>(ErrorCode.None, clearStage);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.LoadLoadStageListFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "LoadStageList Exception");

            return new Tuple<ErrorCode, List<ClearData>>(errorCode, null);
        }
    }

    // 선택한 스테이지 검증
    // ClearStage 테이블에서 이전 스테이지 클리어 여부 확인하고 MasterData에서 스테이지 정보 가져오기
    public async Task<Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>> VerifySelectedStageAsync(Int64 userId, Int64 stageCode)
    {
        try
        {
            // 스테이지 존재 여부 확인
            (var errorCode, var stageItem, var stageEnemy) = await CheckStageExistence(stageCode);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>(errorCode, null, null);
            }

            // 스테이지 진입 권한 있는지 확인
            errorCode = await CheckStageAccessPermission(userId, stageCode);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>(errorCode, null, null);
            }

            return new Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>(ErrorCode.None, stageItem, stageEnemy);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.SelectStageFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "SelectStage Exception");

            return new Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>(errorCode, null, null);
        }
    }

    private async Task<Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>> CheckStageExistence(Int64 stageCode)
    {
        var stageItem = _masterDb.StageItemInfo.FindAll(i => i.Code == stageCode);
        var stageEnemy = _masterDb.StageEnemyInfo.FindAll(i => i.Code == stageCode);

        if (stageItem.Count == 0 || stageEnemy.Count == 0)
        {
            return new Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>(ErrorCode.SelectStageFailWrongStage, null, null);
        }

        return new Tuple<ErrorCode, List<StageItem>, List<StageEnemy>>(ErrorCode.None, stageItem, stageEnemy);
    }

    private async Task<ErrorCode> CheckStageAccessPermission(Int64 userId, Int64 stageCode)
    {
        if (stageCode != 1)
        {
            var hasQualified = await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                                                  .Where("BestClearStage", ">=", stageCode - 1).ExistsAsync();

            if (hasQualified == false)
            {
                return ErrorCode.SelectStageFailNotQualified;
            }
        }

        return ErrorCode.None;
    }

    // 던전 클리어 처리
    // redis에 저장하고있었던 획득 목록에 따라 User_Item 테이블에 데이터 추가, User_BasicInformation 테이블 업데이트
    public async Task<Tuple<ErrorCode, List<ItemInfo>, Int64>> ReceiveStageClearRewardAsync(Int64 userId, Int64 stageCode, List<ItemInfo> itemList)
    {
        try
        {
            // 아이템 획득 처리
            (var errorCode, var itemInfo) = await ReceiveItemReward(userId, itemList);
            if (errorCode != ErrorCode.None)
            {
                return new Tuple<ErrorCode, List<ItemInfo>, Int64>(errorCode, null, 0);
            }

            // 경험치 획득 처리
            (errorCode, var obtainExp) = await ReceiveExpReward(userId, stageCode);
            if (errorCode != ErrorCode.None)
            {
                await ReceiveItemRewardRollBack(userId, itemList);

                return new Tuple<ErrorCode, List<ItemInfo>, Int64>(errorCode, null, 0);
            }

            return new Tuple<ErrorCode, List<ItemInfo>, Int64>(ErrorCode.None, itemInfo, obtainExp);
        }
        catch (Exception ex)
        {
            var errorCode = ErrorCode.ReceiveStageClearRewardFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ReceiveStageClearReward Exception");

            return new Tuple<ErrorCode, List<ItemInfo>, Int64>(errorCode, null, 0);
        }
    }

    // 아이템 획득 처리
    // User_Item 테이블에 데이터 추가
    private async Task<Tuple<ErrorCode, List<ItemInfo>>> ReceiveItemReward(Int64 userId, List<ItemInfo> itemList)
    {
        var itemInfo = new List<ItemInfo>();

        try
        {
            foreach (ItemInfo item in itemList)
            {
                var itemType = _masterDb.ItemInfo.Find(i => i.Code == item.ItemCode).Attribute;
                var errorCode = new ErrorCode();

                if (itemType == 4 || itemType == 5) // 마법도구 또는 돈
                {
                    errorCode = await ReceiveConsumableItem(userId, item.ItemCode, item.ItemCount, itemInfo);
                }
                else
                {
                    errorCode = await ReceiveEquipmentItem(userId, item.ItemCode, item.ItemCount, itemInfo);
                }

                if (errorCode != ErrorCode.None)
                {
                    // 롤백
                    await ReceiveItemRewardRollBack(userId, itemInfo);

                    return new Tuple<ErrorCode, List<ItemInfo>>(ErrorCode.ReceiveItemRewardFailInsertItem, null);
                }
            }

            return new Tuple<ErrorCode, List<ItemInfo>>(ErrorCode.None, itemInfo);
        }
        catch (Exception ex)
        {
            // 롤백
            await ReceiveItemRewardRollBack(userId, itemInfo);

            var errorCode = ErrorCode.ReceiveItemRewardFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ReceiveItemReward Exception");

            return new Tuple<ErrorCode, List<ItemInfo>>(errorCode, null);
        }
    }

    private async Task<ErrorCode> ReceiveConsumableItem(Int64 userId, Int64 itemCode, Int64 itemCount, List<ItemInfo> itemInfo)
    {
        var itemId = _idGenerator.CreateId();

        (var errorCode, var newItem) = await InsertUserItemAsync(userId, itemCode, itemCount, itemId);
        if (errorCode != ErrorCode.None)
        {
            return errorCode;
        }

        itemInfo.Add(newItem);

        return ErrorCode.None;
    }

    private async Task<ErrorCode> ReceiveEquipmentItem(Int64 userId, Int64 itemCode, Int64 itemCount, List<ItemInfo> itemInfo)
    {
        for (Int64 i = 0; i < itemCount; i++)
        {
            var itemId = _idGenerator.CreateId();

            (var errorCode, var newItem) = await InsertUserItemAsync(userId, itemCode, 1, itemId);
            if (errorCode != ErrorCode.None)
            {
                return errorCode;
            }

            itemInfo.Add(newItem);
        }

        return ErrorCode.None;
    }

    private async Task ReceiveItemRewardRollBack(Int64 userId, List<ItemInfo> itemInfo)
    {
        for (int i = 0; i <= itemInfo.Count; i++)
        {
            await DeleteUserItemAsync(userId, itemInfo[i].ItemId, itemInfo[i].ItemCount);
        }
    }

    // 경험치 획득 처리
    // User_BasicInformation 테이블 업데이트
    private async Task<Tuple<ErrorCode, Int64>> ReceiveExpReward(Int64 userId, Int64 stageCode)
    {
        var userData = new UserData();

        try
        {
            // 획득 경험치 계산
            var obtainedExp = await CalculateObtainExp(stageCode);

            // 경험치 획득 처리
            (var afterLevel, var afterExp) = await CalculateUserLevelandExp(userId, obtainedExp, userData);

            // 유저 데이터 갱신
            await UpdateUserLevelandExp(userId, afterLevel, afterExp);

            return new Tuple<ErrorCode, Int64>(ErrorCode.None, obtainedExp);
        }
        catch (Exception ex)
        {
            //롤백
            await UpdateUserLevelandExp(userId, userData.Level, userData.Exp);

            var errorCode = ErrorCode.ReceiveExpRewardFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "ReceiveExpReward Exception");

            return new Tuple<ErrorCode, Int64>(errorCode, 0);
        }
    }

    private async Task<Int64> CalculateObtainExp(Int64 stageCode)
    {
        var obtainExp = new Int64();

        foreach (StageEnemy stageEnemy in _masterDb.StageEnemyInfo.FindAll(i => i.Code == stageCode))
        {
            obtainExp += stageEnemy.Exp * stageEnemy.Count;
        }

        return obtainExp;
    }

    private async Task<Tuple<Int64, Int64>> CalculateUserLevelandExp(Int64 userId, Int64 obtainedExp, UserData userData)
    {
        userData = _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                                    .FirstOrDefault<UserData>();

        var exp = userData.Exp + obtainedExp;
        var level = userData.Level;

        Int64 requireExp;
        while (exp >= (requireExp = _masterDb.ExpTableInfo.Find(i => i.Level == level).RequireExp))
        {
            exp -= requireExp;
            level++;
        }

        return new Tuple<Int64, Int64>(level, exp);
    }

    private async Task UpdateUserLevelandExp(Int64 userId, Int64 level, Int64 exp)
    {
        await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                           .UpdateAsync(new { Level = level, Exp = exp });
    }


    // 클리어 정보 처리
    // ClearStage 테이블에 데이터 추가 또는 업데이트 
    public async Task<ErrorCode> UpdateStageClearDataAsync(Int64 userId, Int64 stageCode, Int64 clearRank, TimeSpan clearTime)
    {
        Int64 beforeBestClearStage = 0;
        var beforeClearData = new ClearData();       

        try
        {
            // 최고 스테이지 기록 갱신
            beforeBestClearStage = await UpdateBestClearStage(userId, stageCode);

            // 클리어 정보 갱신
            beforeClearData = await UpdateClearData(userId, stageCode, clearRank, clearTime);

            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            // 롤백
            await UpdateStageClearDataRollBack(userId, stageCode, beforeBestClearStage, beforeClearData);

            var errorCode = ErrorCode.UpdateStageClearDataFailException;

            _logger.ZLogError(LogManager.MakeEventId(errorCode), ex, "UpdateStageClearData Exception");
            
            return errorCode;
        }
    }

    private async Task<Int64> UpdateBestClearStage(Int64 userId, Int64 stageCode)
    {
        var bestClearStage = await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                                                .Select("BestClearStage").FirstOrDefaultAsync<Int64>();

        if (bestClearStage < stageCode)
        {
            await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                               .UpdateAsync(new { BestClearStage = stageCode });
        }

        return bestClearStage;
    }

    private async Task<ClearData> UpdateClearData(Int64 userId, Int64 stageCode, Int64 clearRank, TimeSpan clearTime)
    {
        var beforeClearData = await _queryFactory.Query("User_ClearStage").Where("UserId", userId)
                                                 .Where("StageCode", stageCode).FirstOrDefaultAsync<ClearData>();

        if (beforeClearData == null)
        {
            await _queryFactory.Query("User_ClearStage").InsertAsync(new
            {
                UserId = userId,
                StageCode = stageCode,
                ClearRank = clearRank,
                ClearTime = clearTime
            });
        }
        else if (beforeClearData.ClearRank < clearRank)
        {
            await _queryFactory.Query("User_ClearStage").Where("UserId", userId)
                               .Where("StageCode", stageCode).UpdateAsync(new
                               {
                                   ClearRank = clearRank,
                                   ClearTime = clearTime
                               });
        }
        else if (beforeClearData.ClearRank == clearRank && beforeClearData.ClearTime > clearTime)
        {
            await _queryFactory.Query("User_ClearStage").Where("UserId", userId)
                               .Where("StageCode", stageCode).UpdateAsync(new { ClearTime = clearTime });
        }

        return beforeClearData;
    }

    private async Task UpdateStageClearDataRollBack(Int64 userId, Int64 stageCode, Int64 beforeBestClearStage, ClearData beforeClearData)
    {
        await _queryFactory.Query("User_BasicInformation").Where("UserId", userId)
                           .UpdateAsync(new { BestClearStage = beforeBestClearStage });

        if (beforeClearData == null)
        {
            await _queryFactory.Query("User_ClearStage").Where("UserId", userId)
                               .Where("StageCode", stageCode).DeleteAsync();
        }
        else
        {
            await _queryFactory.Query("User_ClearStage").Where("UserId", userId)
                               .Where("StageCode", stageCode).UpdateAsync(new
                               {
                                   ClearRank = beforeClearData.ClearRank,
                                   ClearTime = beforeClearData.ClearTime
                               });
        }
    }
}


