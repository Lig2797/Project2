using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
[GenerateSerializationForTypeAttribute(typeof(CropData))]
public struct CropData : INetworkSerializable
{
    // --- Growth State ---
    public int CurrentStage;
    public int TimeToChangeStage;
    public int StageTimeCounter;
    public bool NeedChangeStage;

    // --- Metadata ---
    public int StagesCount;
    public ESeason Season;
    public FixedString64Bytes CropSeedName;
    public FixedString64Bytes CropProductName;
    public bool CanReHarvest;

    // --- Harvest Parameters ---
    public int QuantityFertilizedLevel;
    public int QualityFertilizedLevel;
    public int FastGrowFertilizedLevel;


    // --- Constructor ---
    public CropData(
        int stagesCount,
        int timeToChangeStage,
        ESeason season,
        string cropSeedName,
        string cropProductName,
        bool canReHarvest)
    {
        CurrentStage = 1;
        TimeToChangeStage = timeToChangeStage;
        StageTimeCounter = 0;
        NeedChangeStage = false;

        StagesCount = stagesCount;
        Season = season;
        CropSeedName = cropSeedName;
        CropProductName = cropProductName;
        CanReHarvest = canReHarvest;

        QuantityFertilizedLevel = 0;
        QualityFertilizedLevel = 0;
        FastGrowFertilizedLevel = 1;
    }

    // --- Logic ---
    public void GrowthTimeUpdate(int minutes)
    {
        if (IsFullyGrown()) return;

        minutes *= FastGrowFertilizedLevel;
        StageTimeCounter += minutes;

        if (StageTimeCounter >= TimeToChangeStage)
        {
            NeedChangeStage = true;
            CurrentStage++;
            StageTimeCounter = 0;
        }
    }

    public bool IsFullyGrown()
    {
        return CurrentStage >= StagesCount;
    }

    // --- Network Serialization ---
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CurrentStage);
        serializer.SerializeValue(ref TimeToChangeStage);
        serializer.SerializeValue(ref StageTimeCounter);
        serializer.SerializeValue(ref NeedChangeStage);

        serializer.SerializeValue(ref StagesCount);
        serializer.SerializeValue(ref Season);
        serializer.SerializeValue(ref CropSeedName);
        serializer.SerializeValue(ref CropProductName);
        serializer.SerializeValue(ref CanReHarvest);

        serializer.SerializeValue(ref QuantityFertilizedLevel);
        serializer.SerializeValue(ref QualityFertilizedLevel);
        serializer.SerializeValue(ref FastGrowFertilizedLevel);

    }


}
