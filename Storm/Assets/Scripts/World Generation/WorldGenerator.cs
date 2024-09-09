using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Tile_ {
        // public Tile tile;
        public Tile tile;
        [Range(0,1)]
        public float depth;
    }

    Tilemap tilemap;
    // Dictionary<Vector3Int, Tile> playerChanges = new();
    Dictionary<Vector3, Tile> playerChanges = new();

    public Tile_[] tiles;
    [Tooltip("The actual size of the chunk will be twice this value")]
    public int chunkSize = 25;
    public int chunkBottomLimit = -15;
    [Space]
    [Header("Noise Variables")]
    public int seed;
    [Tooltip("Number of layered noise functions")]
    [Range(1, 10)]
    public int octaves = 3;
    public float amplitude = 10;
    [Tooltip("How zoomed out (< 10) or zoomed in (> 10) the first noise layer is")]
    public float scale = 1;
    [Tooltip("How much to zoom out each of the octaves")]
    public float frequency = 1;
    [Tooltip("How much influence each octave has -> higher = more fine details")]
    [Range(0, 1)]
    public float lacunarity = 1;
    public Vector2Int offset = Vector2Int.zero;

    // [HideInInspector]
    public int xPos;

    int previous_xPos;
    bool update = true;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        // seed = (int)Random.Range(-0xFFFFFFFF, 0xFFFFFFFF);
        Random.InitState(seed);
        xPos = 0;

        playerChanges.Clear();

        // BuildWorld();
    }

    public void DeleteTileAt(Vector3 pos)
    {
        playerChanges.Add(pos, null);
        update = true;
        // DeleteTile(tilemap.WorldToCell(pos));
    }

    // void DeleteTile(Vector3Int tile)
    // {
    //     playerChanges.Add(tile, null);
    // }

    public void ModifyTileAt(Vector3 pos, Tile newTile)
    {
        playerChanges.Add(pos, newTile);
        update = true;
        // ModifyTile(tilemap.WorldToCell(pos), newTile);
    }

    // void ModifyTile(Vector3Int tile, Tile newTile)
    // {
    //     playerChanges.Add(tile, newTile);
    // }

    void Update()
    {
        if (previous_xPos != xPos)
            update = true;

        if (update){
            tilemap.ClearAllTiles();
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

            BuildWorld();

            previous_xPos = xPos;
            update = false;
        }
    }


    void BuildWorld()
    {
        CheckArguments();

        for (int x = -chunkSize; x < chunkSize; x++)
        {
            int noiseVal = Mathf.RoundToInt(GetNoiseVal(x));

            // tilemap.SetTile(new Vector3Int(x, noiseVal), tiles[0].tile);
            FillColumn(x, noiseVal);

            // tilemap.SetTile(new Vector3Int(x, chunkBottomLimit), tiles[^1].tile);
        }
    }

    void CheckArguments()
    {
        if (chunkSize <= 0)
            chunkSize = 1;

        if (octaves <= 0)
            octaves = 1;

        if (amplitude < Mathf.Epsilon)
            amplitude = Mathf.Epsilon;

        if (frequency < Mathf.Epsilon)
            frequency = Mathf.Epsilon;

        if (scale < Mathf.Epsilon)
            scale = Mathf.Epsilon;
    }

    void FillColumn(int x, int maxHeight)
    {
        Tile_[] _tiles = tiles.ToList().OrderBy(x => x.depth).Reverse().ToArray();

        for (int y = chunkBottomLimit; y <= maxHeight; y++)
        {
            if (playerChanges.Any(v => tilemap.WorldToCell(v.Key).Equals(new Vector3Int(x, y))))
            {
                Tile playerChange = playerChanges.First(v => tilemap.WorldToCell(v.Key).Equals(new Vector3Int(x, y))).Value;

                if(playerChange != null)
                    tilemap.SetTile(new Vector3Int(x, y), playerChange);

                continue;
            }

            float depth = Remap(y, chunkBottomLimit, maxHeight, 0, 1);

            TileBase availableTile = tiles.ToList().FirstOrDefault(x => x.depth <= depth).tile;
            // TileBase[] availableTiles = tiles.ToList().FindAll(x => x.depth < depth).Select(x => x.tile).ToArray();


            // if (availableTiles.Length == 0)
            // {
            //     Debug.LogError("There probably isn't a tile with a depth of 0!", this);
            //     continue;
            // }

            // int randomTile = Random.Range(0, availableTiles.Length - 1);
            tilemap.SetTile(new Vector3Int(x, y), availableTile);
        }
    }

    float GetNoiseVal(int x)
    {
        float noiseVal = 0, scl = scale / 10, amp = amplitude;
        for (int i = 1; i <= octaves; i++)
        {
            float rawVal = Mathf.Clamp01(Mathf.PerlinNoise((x + offset.x + xPos + 0xFFFF) * scl, seed)) * amp + offset.y;

            scl *= frequency;
            amp *= lacunarity;

            noiseVal += rawVal;
        }

        return noiseVal;
    }

    float Remap(float val, float fromMin, float fromMax, float toMin, float toMax) => toMin + (val-fromMin)*(toMax-toMin)/(fromMax-fromMin);
}