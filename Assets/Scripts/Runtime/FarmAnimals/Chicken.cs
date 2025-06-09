using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : FarmAnimal
{
    [SerializeField] private GameObject chickenPrefab;
    private ChickenGrowthStage _currentGrowthStage;

    protected override void Initial()
    {
        _currentGrowthStage = 0;
        base.Initial();
        ApplyStage(_currentGrowthStage.ToString());
        CanMove.Value = false;
    }
    public override void FedTimeHandler(int minute)
    {
        if (_currentGrowthStage == 0)
        {
            fedTimeCounter += minute;
            if (fedTimeCounter > _animalInfo.FedTimesNeededToGrow)
            {
                fedTimeCounter = 0;
                _animator.Play("AboutToHatch");
            }
        }
        else
        {
            if (!isFed) return;
            fedTimeCounter += minute;
            if (_currentGrowthStage != ChickenGrowthStage.Mature)
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
        
    }

    protected override void Eat()
    {
        if (_currentGrowthStage == ChickenGrowthStage.Egg)
            return;

        base.Eat();

    }

    public override void IncreaseGrowStage()
    {
        int next = (int)_currentGrowthStage + 1;
        int max = System.Enum.GetValues(typeof(ChickenGrowthStage)).Length - 1;
        _currentGrowthStage = (ChickenGrowthStage)Mathf.Min(next, max);

        if((int)_currentGrowthStage == 1)
            CanMove.Value = true;

        ApplyStage(_currentGrowthStage.ToString());
        isFed = false;
    }

    protected override void ApplyStage(string stage)
    {
        base.ApplyStage(stage);
    }

    protected override void MakeProduct()
    {
        var newAnimal = Instantiate(chickenPrefab, transform.position, Quaternion.identity);
        newAnimal.GetComponent<Chicken>().Initial();
    }

}
