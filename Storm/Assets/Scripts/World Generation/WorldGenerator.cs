using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Tile_ {
        // public Tile tile;
        public TileBase tile;
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
    [Range(1, 10)]
    public int octaves = 3;
    public float amplitude = 10;
    public float frequency = 1;
    public float scale = 1;
    [Range(0, 1)]
    public float lacunarity = 1;
    public Vector2Int offset = Vector2Int.zero;


    [HideInInspector]
    public int xPos;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        // seed = (int)Random.Range(-0xFFFFFFFF, 0xFFFFFFFF);
        Random.InitState(seed);
        xPos = 0;
    }

    void Update()
    {
        tilemap.ClearAllTiles();
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
            float noiseVal = GetNoiseVal(x);

            tilemap.SetTile(new Vector3Int(x, Mathf.RoundToInt(noiseVal)), tiles[0].tile);
            tilemap.SetTile(new Vector3Int(x, chunkBottomLimit), tiles[^1].tile);
        }
    }

    float GetNoiseVal(int x)
    {
        float noiseVal = 0, scl = scale / 10, amp = amplitude;
        for (int i = 1; i <= octaves; i++)
        {
            float rawVal = Mathf.Clamp01(Mathf.PerlinNoise((x + offset.x + 0xFFFF) * scl, seed)) * amp + offset.y;

            scl *= frequency;
            amp *= lacunarity;

            noiseVal += rawVal;
        }

        return noiseVal;
    }
}