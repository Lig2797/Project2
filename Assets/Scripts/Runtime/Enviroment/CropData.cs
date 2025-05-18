using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CropData
{
    [SerializeField] public int _currentStage;
    [SerializeField] public int growthTimeLeft;
    [SerializeField] public bool isWatered;
    [SerializeField] public int timeToChangeStage;
    [SerializeField] public int stageTimeCounter;
    [SerializeField] public bool needChangeStage;
    [SerializeField] public ESeason season;
    [SerializeField] public TileBase[] growthStages;
    [SerializeField] public string cropProductName;
    [SerializeField] public int[] level; // crop se co 4 level
    [SerializeField] public float[] ratio;
    [SerializeField] public bool CanReHarvest;
    [SerializeField] public int[] numOfProductCouldDrop;
    [SerializeField] public float[] ratioForEachNum;
    // them he thong chon level cua crop khi thu hoach
    public CropData(int growthTime, TileBase[] growthStages, ESeason season, string cropProductName, bool canReHarvest, int[] numOfProductCouldDrop, float[] ratioForEachNum)
    {
        needChangeStage = false;
        _currentStage = 0;
        isWatered = false;
        growthTimeLeft = growthTime;
        this.growthStages = growthStages;
        timeToChangeStage = growthTime / growthStages.Length;
        stageTimeCounter = 0;
        this.season = season;
        this.cropProductName = cropProductName;
        level = new int[] { 1, 2, 3, 4 };
        ratio = new float[] { 100, 0, 0, 0 };
        CanReHarvest = canReHarvest;
        this.numOfProductCouldDrop = numOfProductCouldDrop;
        this.ratioForEachNum = ratioForEachNum;
    }

    public void GrowthTimeUpdate(int minute)
    {
        growthTimeLeft -= minute;
        stageTimeCounter += minute;

        if(stageTimeCounter >= timeToChangeStage && _currentStage < growthStages.Length - 1)
        {
            needChangeStage = true;
            _currentStage++;
            stageTimeCounter = 0;
        }
    }
    public bool IsFullyGrown()
    {
        return _currentStage == growthStages.Length - 1;
    }
}
