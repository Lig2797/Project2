using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAnimalManager : PersistentSingleton<FarmAnimalManager>
{
    [SerializeField]
    private List<FarmAnimal> _farmAnimals = new List<FarmAnimal>();

    private void OnEnable()
    {
        EnviromentalStatusManager.OnTimeIncrease += IncreaseFedTime;
    }

    private void IncreaseFedTime(int minute)
    {
        foreach (var animal in _farmAnimals)
        {
            animal.FedTimeHandler(minute);
        }
    }

    public void RegisterAnimal(FarmAnimal animal)
    {
        if (!_farmAnimals.Contains(animal))
            _farmAnimals.Add(animal);
    }

    public void UnregisterAnimal(FarmAnimal animal)
    {
        if (_farmAnimals.Contains(animal))
            _farmAnimals.Remove(animal);
    }

}
