using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Map Size & Parameters")]
    public int width = 100;
    public int height = 100;
    [Range(0, 100)] public int randomFillPercent = 45;
    public int smoothIterations = 5;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public Tilemap railsTilemap;

    [Header("Tiles")]
    public RuleTile groundTile;
    public RuleTile wallTile;
    public TileBase railTile;

    private int[,] map;

    void Start()
    {
        GenerateMap();
        SmoothMap();
        EnsureMinimumGroundSize();
        RemoveTrappedGround();
        DrawTiles();
        PlaceRails();
    }

    // 🔧 Step 1: Random Fill
    void GenerateMap()
    {
        map = new int[width, height];
        System.Random rng = new System.Random();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 0; // ← fill everything with ground

        // Then carve walls randomly
        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
                if (rng.Next(100) < randomFillPercent)
                    map[x, y] = 1; // wall
    }


    // 🔧 Step 2: Cellular Automata
    void SmoothMap()
    {
        for (int i = 0; i < smoothIterations; i++)
        {
            int[,] newMap = new int[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int neighbors = GetWallCountAround(x, y);
                    newMap[x, y] = neighbors > 4 ? 1 : (neighbors < 4 ? 0 : map[x, y]);
                }
            map = newMap;
        }
    }

    int GetWallCountAround(int x, int y)
    {
        int count = 0;
        for (int nx = x - 1; nx <= x + 1; nx++)
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx == x && ny == y) continue;
                if (nx < 0 || ny < 0 || nx >= width || ny >= height) count++;
                else if (map[nx, ny] == 1) count++;
            }
        return count;
    }

    // 🔧 Step 3: Remove Thin Ground (Enforce 2x2 ground shapes)
    void EnsureMinimumGroundSize()
    {
        for (int x = 1; x < width - 2; x++)
            for (int y = 1; y < height - 2; y++)
                if (map[x, y] == 0)
                {
                    bool is2x2 =
                        map[x, y] == 0 &&
                        map[x + 1, y] == 0 &&
                        map[x, y + 1] == 0 &&
                        map[x + 1, y + 1] == 0;
                    if (!is2x2)
                        map[x, y] = 1;
                }
    }

    // 🔧 Step 4: Prevent player-trap boxes
    void RemoveTrappedGround()
    {
        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
                if (map[x, y] == 0)
                {
                    bool hasOpenNeighbor = false;
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int nx = x + dx;
                            int ny = y + dy;
                            if (map[nx, ny] == 0)
                            {
                                hasOpenNeighbor = true;
                                break;
                            }
                        }
                    if (!hasOpenNeighbor)
                        map[x, y] = 1; // Fill in to prevent trap
                }
    }

    // 🔧 Step 5: Draw ground and wall tiles with 2x2 wall enforcement
    void DrawTiles()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                groundTilemap.SetTile(pos, groundTile); // ← always place ground

                // Only place walls in large enough blobs
                if (map[x, y] == 1 && IsWallSolid(x, y))
                    wallTilemap.SetTile(pos, wallTile);


            }
        }
    }
    bool IsInWallBlob(int x, int y, int size)
    {
        for (int dx = 0; dx < size; dx++)
            for (int dy = 0; dy < size; dy++)
                if (x + dx >= width || y + dy >= height || map[x + dx, y + dy] != 1)
                    return false;

        return true;
    }
    bool IsWallSolid(int x, int y)
    {
        // Must be at least a 2x2 chunk (including this tile)
        bool partOf2x2 =
            InBounds(x + 1, y) && InBounds(x, y + 1) && InBounds(x + 1, y + 1) &&
            map[x, y] == 1 &&
            map[x + 1, y] == 1 &&
            map[x, y + 1] == 1 &&
            map[x + 1, y + 1] == 1;

        // Optional: Require some wall to left/right/top/bottom for thickness
        int directNeighborCount = 0;
        if (InBounds(x + 1, y) && map[x + 1, y] == 1) directNeighborCount++;
        if (InBounds(x - 1, y) && map[x - 1, y] == 1) directNeighborCount++;
        if (InBounds(x, y + 1) && map[x, y + 1] == 1) directNeighborCount++;
        if (InBounds(x, y - 1) && map[x, y - 1] == 1) directNeighborCount++;

        return partOf2x2 || directNeighborCount >= 3;
    }
    bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;



    // 🔧 Check if ground is directly below a wall (for wall-top blending)
    bool HasGroundBelow(int x, int y)
    {
        return y > 0 && map[x, y - 1] == 0;
    }

    // 🔧 Step 6: Place rails randomly on ground tiles that have a wall above
    void PlaceRails()
    {
        railsTilemap.ClearAllTiles();

        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
                if (map[x, y] == 0 && map[x, y + 1] == 1 && Random.value < 0.05f)
                    railsTilemap.SetTile(new Vector3Int(x, y, 0), railTile);
    }
}
