using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : FarmAnimal
{
    [SerializeField] private GameObject cowPrefab;
    private CowGrowthStage _currentGrowthStage;

    protected override void Initial()
    {
        _currentGrowthStage = 0;
        base.Initial();
        ApplyStage(_currentGrowthStage.ToString());
    }
    public override void FedTimeHandler(int minute)
    {
        if (!isFed) return;
        fedTimeCounter += minute;
        if (_currentGrowthStage != CowGrowthStage.Baby)
        {
            if (fedTimeCounter > _animalInfo.FedTimesNeededToGrow)
            {
                fedTimeCounter = 0;
                IncreaseGrowStage();
            }
        }
        else
        {
            if (fedTimeCounter > _animalInfo.FedTimesNeededToMakeProduct)
            {
                fedTimeCounter = 0;
                MakeProduct();
            }
        }
    }
    protected override void ApplyStage(string stage)
    {
        base.ApplyStage(stage);
    }
    public override void IncreaseGrowStage()
    {
        int next = (int)_currentGrowthStage + 1;
        int max = System.Enum.GetValues(typeof(CowGrowthStage)).Length - 1;
        _currentGrowthStage = (CowGrowthStage)Mathf.Min(next, max);

        ApplyStage(_currentGrowthStage.ToString());
    }



}
