using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCropma : MonoBehaviour
{
    public Tilemap[] tilemaps;
    public Tilemap farm;
    public Tilemap watered;
    public Tilemap crop;



    private void Start()
    {
        TileManager.Instance.AddTilemap(farm);
        TileManager.Instance.AddTilemap(watered);
        CropManager.Instance.SetTilemap(crop);

        TileTargeter tileTargeter = GameObject.FindGameObjectWithTag("Player").GetComponent<TileTargeter>();
        tileTargeter.SetTilemap(tilemaps);
    }
}
