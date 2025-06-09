using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : FarmAnimal
{
    [SerializeField] private GameObject sheepPrefab;
    private SheepGrowthStage _currentGrowthStage;
    [SerializeField] private bool _canMakeProduct = false;

    protected override void Initial()
    {
        _currentGrowthStage = 0;
        base.Initial();
        base.ApplyStage(_currentGrowthStage.ToString());
    }

    protected override void MakeProduct()
    {
        _canMakeProduct = true;

    }
    [ContextMenu("shave hair")]
    protected override void GetProduct()
    {
        if (_canMakeProduct)
        {
            _canMakeProduct = false;
            Debug.Log("Got hair");
            DecreaseGrowStage();
        }
    }
    protected override void InteractWithAnimal()
    {

    }

    protected override void ApplyStage(string stage)
    {
        base.ApplyStage(stage);
    }

    public override void FedTimeHandler(int minute)
    {
        if (!isFed || _currentGrowthStage == SheepGrowthStage.Haired) return;
        fedTimeCounter += minute;
        if(_currentGrowthStage != SheepGrowthStage.Haired)
        {
            if (fedTimeCounter > _animalInfo.FedTimesNeededToGrow)
            {
                fedTimeCounter = 0;
                IncreaseGrowStage();
            }
        }
    }

    public override void IncreaseGrowStage()
    {
        int next = (int)_currentGrowthStage + 1;
        int max = System.Enum.GetValues(typeof(SheepGrowthStage)).Length - 1;
        _currentGrowthStage = (SheepGrowthStage)Mathf.Min(next, max);

        base.ApplyStage(_currentGrowthStage.ToString()); 
        isFed = false;
    }

    private void DecreaseGrowStage()
    {
        int prev = (int)_currentGrowthStage - 1;
        int min = 0;
        _currentGrowthStage = (SheepGrowthStage)Mathf.Max(prev, min);

        base.ApplyStage(_currentGrowthStage.ToString());
    }
}
