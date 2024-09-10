using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct LayerTile
    {
        public TileBase tile;
        [Range(0, 1)]
        public float depth;
    }

    [System.Serializable]
    public struct FeatureTile
    {
        public Tile tile;
        [Tooltip("A spawn chance of 0 will spawn only once when the world is created at the spawn point.")]
        [Range(0, 1)]
        public float spawnChance;
        public uint width;
    }

    public LayerTile[] tiles;
    [Tooltip("The actual size of the chunk will be twice this value")]
    public int chunkSize = 25;
    public int chunkBottomLimit = -15;
    public FeatureTile[] features;
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
    public Tilemap featuresTilemap;
    public Tilemap groundTilemap;

    Dictionary<Vector3, Tile> playerChanges = new();
    Dictionary<Vector3Int, Tile> featuresLayer = new();
    // [HideInInspector]
    public int xPos;
    int previous_xPos;
    bool update = true;
    int previousFeatures_xPos;
    bool updateFeatures = true;
    int[] heightMap;

    void Start()
    {
        Random.InitState(seed);
        xPos = 0;

        playerChanges.Clear();
    }

    public void DeleteTileAt(Vector3 pos)
    {
        if (!playerChanges.Any(x => x.Key.Equals(pos)))
        {
            playerChanges.Add(pos, null);
            update = true;
        }
    }

    public void ModifyTileAt(Vector3 pos, Tile newTile)
    {
        if (!playerChanges.Any(x => x.Key.Equals(pos)))
        {
            playerChanges.Add(pos, newTile);
            update = true;
        }
    }

    void Update()
    {
        if (previous_xPos != xPos)
            update = true;
        if (Mathf.Abs(previousFeatures_xPos - xPos) >= 5)
        {
            updateFeatures = true;
            previousFeatures_xPos = xPos;
        }

        if (update || updateFeatures)
        {
            BuildWorld(updateFeatures);
            update = false;
            updateFeatures = false;
        }
    }


    void BuildWorld(bool updateFeatures)
    {
        featuresTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();
        groundTilemap.transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
        previous_xPos = xPos;

        CheckArguments();

        heightMap = new int[2 * chunkSize];
        for (int x = -chunkSize; x < chunkSize; x++)
        {
            int noiseVal = Mathf.RoundToInt(GetNoiseVal(x));
            heightMap[x + chunkSize] = noiseVal;

            FillColumn(x, noiseVal);
        }

        if (updateFeatures)
            PlaceFeatures(heightMap);
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
        LayerTile[] _tiles = tiles.ToList().OrderBy(x => x.depth).Reverse().ToArray();

        for (int y = chunkBottomLimit; y <= maxHeight; y++)
        {
            if (playerChanges.Any(v => groundTilemap.WorldToCell(v.Key).Equals(new Vector3Int(x, y))))
            {
                Tile playerChange = playerChanges.First(v => groundTilemap.WorldToCell(v.Key).Equals(new Vector3Int(x, y))).Value;

                if (playerChange != null)
                    groundTilemap.SetTile(new Vector3Int(x, y), playerChange);

                continue;
            }

            float depth = Remap(y, chunkBottomLimit, maxHeight, 0, 1);

            var firstOption = tiles.ToList().FirstOrDefault(x => x.depth <= depth);
            List<TileBase> availableTiles = new()
            {
                firstOption.tile
            };
            availableTiles.AddRange(tiles.ToList().FindAll(x => x.depth == firstOption.depth).Select(x => x.tile));

            var worldpos = Vector3Int.FloorToInt(groundTilemap.CellToWorld(new Vector3Int(x, y)));

            System.Random rng = new(seed + worldpos.GetHashCode());
            groundTilemap.SetTile(new Vector3Int(x, y), availableTiles[rng.Next(availableTiles.Count)]);
        }
    }

    void PlaceFeatures(int[] heightMap)
    {
        for (int x = 0, flatSize; x < 2 * chunkSize - 1; x += flatSize)
        {
            Vector3Int currentCellPos = featuresTilemap.WorldToCell(groundTilemap.CellToWorld(new Vector3Int(x, heightMap[x] + 1)));
            if (featuresLayer.Any(v => v.Key.Equals(currentCellPos)))
                break;

            flatSize = 1;
            for (int i = x + 1, j = 1; i < 2 * chunkSize; i++, j++)
            {
                if (featuresLayer.Any(v => v.Key.Equals(new Vector3Int(currentCellPos.x + j, currentCellPos.y))))
                    break;

                if (heightMap[i] == heightMap[x])
                    flatSize++;
                else
                    break;
            }

            FeatureTile[] potentialFeatures = features.ToList().FindAll(v => v.width <= flatSize).OrderBy(v => v.width).ToArray();
            if (potentialFeatures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    featuresLayer.Add(new Vector3Int(currentCellPos.x + i, currentCellPos.y), null);
                continue;
            }

            System.Random rng = new(seed + xPos);
            float chance = (float)rng.NextDouble();

            potentialFeatures = potentialFeatures.ToList().FindAll(v => v.spawnChance > chance).ToArray();
            if (potentialFeatures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    featuresLayer.Add(new Vector3Int(currentCellPos.x + i, currentCellPos.y), null);
                continue;
            }

            Tile selectedFeature = potentialFeatures[rng.Next(potentialFeatures.Length)].tile;
            featuresTilemap.SetTile(currentCellPos, selectedFeature);

            featuresLayer.Add(currentCellPos, selectedFeature);
            for (int i = 1; i < flatSize; i++)
                featuresLayer.Add(new Vector3Int(currentCellPos.x + i, currentCellPos.y), null);
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

    float Remap(float val, float fromMin, float fromMax, float toMin, float toMax) => toMin + (val - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}