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

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        // seed = (int)Random.Range(-0xFFFFFFFF, 0xFFFFFFFF);
        Random.InitState(seed);
        xPos = 0;

        // BuildWorld();
    }

    void Update()
    {
        tilemap.ClearAllTiles();
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
        // temporary to visualize changes
        BuildWorld();
    }

    void BuildWorld()
    {
        CheckArguments();
        CreateFirstLayer();
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

    void CreateFirstLayer()
    {
        for (int x = -chunkSize; x < chunkSize; x++)
        {
            int noiseVal = Mathf.RoundToInt(GetNoiseVal(x));

            tilemap.SetTile(new Vector3Int(x, noiseVal), tiles[0].tile);
            FillColumn(x, noiseVal);

            tilemap.SetTile(new Vector3Int(x, chunkBottomLimit), tiles[^1].tile);
        }
    }

    void FillColumn(int x, int y)
    {
        Tile_[] _tiles = tiles.ToList().OrderBy(x => x.depth).Reverse().ToArray();

        for (int i = chunkBottomLimit + 1; i < y; i++)
        {
            float depth = Remap(i, chunkBottomLimit, y, 0, 1);

            TileBase availableTile = tiles.ToList().FirstOrDefault(x => x.depth < depth).tile;
            // TileBase[] availableTiles = tiles.ToList().FindAll(x => x.depth < depth).Select(x => x.tile).ToArray();


            // if (availableTiles.Length == 0)
            // {
            //     Debug.LogError("There probably isn't a tile with a depth of 0!", this);
            //     continue;
            // }

            // int randomTile = Random.Range(0, availableTiles.Length - 1);
            tilemap.SetTile(new Vector3Int(x, i), availableTile);
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