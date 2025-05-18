
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

public class CropManager : Singleton<CropManager>
{
    public Tilemap cropTilemap;

    private SerializableDictionary<Vector3Int, CropData> _plantedCrops = new SerializableDictionary<Vector3Int, CropData>();
    public SerializableDictionary<Vector3Int, CropData> PlantedCrops
    {
        get { return _plantedCrops; }
        set { _plantedCrops = value; }
    }

    private void OnEnable()
    {
        EnviromentalStatusManager.OnTimeIncrease += UpdateCropsGrowthTime;
    }

    private void OnDisable()
    {
        EnviromentalStatusManager.OnTimeIncrease -= UpdateCropsGrowthTime;
    }


    public void TryModifyCrop(Vector3Int plantPosition, string cropName, int stage)
    {
        RequestToModifyCropServerRpc(plantPosition, cropName, stage);
    }

    [ServerRpc]
    private void RequestToModifyCropServerRpc(Vector3Int plantPosition, string cropName, int stage)
    {
        ModifyCropClientRpc(plantPosition, cropName, stage);
    }

    [ClientRpc]
    private void ModifyCropClientRpc(Vector3Int plantPosition, string cropName, int stage)
    {
        if(cropName == null) ChangeCropStage(plantPosition, null, 0); // null = delete
        else
        {
            Item cropItem = ItemDatabase.Instance.GetItemByName(cropName);
            if (stage == 1)
                PlantCrop(plantPosition, cropItem);
            else ChangeCropStage(plantPosition, cropItem, stage);
        }
        
    }

    public void PlantCrop(Vector3Int plantPosition, Item crop)
    {
        if (!_plantedCrops.ContainsKey(plantPosition))
        {
            CropData newCrop = new CropData(crop.CropSetting.TimeToGrowth, crop.CropSetting.growthStages, crop.CropSetting.season, crop.CropSetting.cropProductName, crop.CropSetting.canReHarvest, crop.CropSetting.numOfProductCouldDrop, crop.CropSetting.ratioForEachNum);

            _plantedCrops.Add(plantPosition, newCrop);
            cropTilemap.SetTile(plantPosition, newCrop.growthStages[1]);
            Debug.Log($"add crop tile at {plantPosition}");
        }
    }

    private void ChangeCropStage(Vector3Int plantPosition, Item crop, int stage)
    {
        if(crop == null)
        {
            _plantedCrops.Remove(plantPosition);
            cropTilemap.SetTile(plantPosition, null);
        }
        else
        cropTilemap.SetTile(plantPosition, crop.CropSetting.growthStages[stage]);
    }

    public bool TryToHarverst(Vector3Int pos)
    {
        if (PlantedCrops.ContainsKey(pos) && PlantedCrops[pos].IsFullyGrown())
        {
            string cropProductName = PlantedCrops[pos].cropProductName;
            int level = UtilsClass.PickOneByRatio(PlantedCrops[pos].level, PlantedCrops[pos].ratio);
            Item newCropProduct = ItemDatabase.Instance.GetItemByName(cropProductName);
            newCropProduct.image = newCropProduct.cropLevelImage[level - 1];

            int numOfProduct = UtilsClass.PickOneByRatio(PlantedCrops[pos].numOfProductCouldDrop, PlantedCrops[pos].ratioForEachNum);

            ItemWorld crop = new ItemWorld(System.Guid.NewGuid().ToString(), newCropProduct, numOfProduct, pos);
            ItemWorldManager.Instance.DropItemIntoWorld(crop, false, false);
            if (PlantedCrops[pos].CanReHarvest)
            {
                TryModifyCrop(pos, cropProductName + " Seed", PlantedCrops[pos]._currentStage-1);
            }
            else
            {
                RemoveCrop(pos);
            }
            return true;
        }
        return false;
    }

    public void UpdateCropsGrowthTime(int minute)
    {
        foreach (var crop in _plantedCrops.ToList())
        {
            var cropInfo = crop.Value;
            cropInfo.isWatered = TileManager.Instance.WateredTiles.ContainsKey(crop.Key);
            if (!cropInfo.IsFullyGrown())
            {
                if (cropInfo.season != EnviromentalStatusManager.Instance.eStarus.SeasonStatus)
                {
                    Debug.Log(EnviromentalStatusManager.Instance.eStarus.SeasonStatus);
                    Debug.Log("crop dead");
                    TryModifyCrop(crop.Key, cropInfo.cropProductName, 0);
                }
                if (cropInfo.isWatered)
                {
                    cropInfo.GrowthTimeUpdate(minute);
                }
                if (cropInfo.needChangeStage)
                {
                    cropInfo.needChangeStage = false;
                    int currentStage = cropInfo._currentStage;
                    TryModifyCrop(crop.Key, cropInfo.cropProductName, currentStage);
                }
            }
            else
            {
                Debug.Log("crop fully grew");
            }

        }
    }

    

    public void RemoveCrop(Vector3Int pos)
    {
        TryModifyCrop(pos, null, 0);
    }

    public void LoadCrops(SerializableDictionary<Vector3Int, CropData> crops)
    {
        PlantedCrops = crops;
        foreach (var crop in PlantedCrops)
        {
            TryModifyCrop(crop.Key, crop.Value.cropProductName + " Seed", crop.Value._currentStage);
        }
    }
}
