using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
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
        // TODO: implement
        [Tooltip("A spawn chance of 0 will spawn only once when the world is created at the spawn point.")]
        [Range(0, 1)]
        public float spawnChance;
        public uint width;
    }

    [System.Serializable]
    public struct LootContainer
    {
        public GameObject containerPrefab;
        public LootTable[] lootTables;
        [Range(0, 1)]
        public float spawnChance;
        public bool buried;
    }

    public GameObject playerObject;
    public LayerTile[] tiles;
    public FeatureTile[] features;
    public LootContainer[] lootContainers;
    public Tile slopeRight;
    public Tile belowSlopeRight;
    public Tile slopeLeft;
    public Tile belowSlopeLeft;
    [Tooltip("The actual size of the chunk will be twice this value")]
    public int chunkSize = 25;
    public int chunkBottomLimit = -15;
    public int minSpaceBetweenLootContainers = 10;
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

    List<Vector3> playerChanges = new();
    List<int> regionsWithLoot = new();
    Dictionary<Vector3Int, Tile> featuresLayer = new();
    // Dictionary<int, (Vector3, GameObject)> lootContainersHolder = new();
    // [HideInInspector]
    public int xPos;
    int previous_xPos;
    bool update = true;
    int previousFeatures_xPos;
    int previousLoot_xPos;
    bool updateFeatures = true;
    bool updateLoot = true;
    int[] heightMap;

    void Start()
    {
        Random.InitState(seed);
        xPos = 0;

        playerChanges.Clear();

        playerObject.GetComponent<PlayerMovement>().OnPlayerMove += OnPlayerMove;
    }

    public void DeleteTileAt(Vector3 pos)
    {
        if (!playerChanges.Any(x => x.Equals(pos)))
        {
            playerChanges.Add(pos);
            update = true;
        }
    }

    // public void ModifyTileAt(Vector3 pos, Tile newTile)
    // {
    //     if (!playerChanges.Any(x => x.Key.Equals(pos)))
    //     {
    //         playerChanges.Add(pos, newTile);
    //         update = true;
    //     }
    // }

    void Update()
    {
        if (previous_xPos != xPos)
            update = true;
        if (Mathf.Abs(previousFeatures_xPos - xPos) >= 5)
        {
            updateFeatures = true;
            previousFeatures_xPos = xPos;
        }

        if (Mathf.Abs(previousLoot_xPos - xPos) >= minSpaceBetweenLootContainers)
        {
            updateLoot = true;
            previousLoot_xPos = xPos;
        }

        if (update || updateFeatures || updateLoot)
        {
            BuildWorld();
            update = false;
            updateFeatures = false;
        }
    }

    void OnPlayerMove(Vector3 movement, Vector3 position, Vector3 velocity) => xPos = Mathf.RoundToInt(position.x);

    void BuildWorld()
    {
        // featuresTilemap.ClearAllTiles();
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

        heightMap = AddSlopes(heightMap);
        if (updateLoot)
            PlaceLoot(heightMap);
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
            if (playerChanges.Any(v => groundTilemap.WorldToCell(v).Equals(new Vector3Int(x, y))))
                continue;
            // {
            //     Tile playerChange = playerChanges.First(v => groundTilemap.WorldToCell(v).Equals(new Vector3Int(x, y))).Value;
            //     if (playerChange != null)
            //         groundTilemap.SetTile(new Vector3Int(x, y), playerChange);
            // }

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

    int[] AddSlopes(int[] heightMap)
    {
        for (int x = 1, lastHeight = heightMap[0]; x < 2 * chunkSize; x++)
        {
            int currentHeight = heightMap[x];
            if (currentHeight < lastHeight)
            {
                heightMap[x] = lastHeight;
                if (!playerChanges.Any(v => groundTilemap.WorldToCell(v).Equals(new Vector3Int(x - chunkSize, lastHeight))))
                    groundTilemap.SetTile(new Vector3Int(x - chunkSize, lastHeight), slopeLeft);
                if (!playerChanges.Any(v => groundTilemap.WorldToCell(v).Equals(new Vector3Int(x - chunkSize, currentHeight))))
                    groundTilemap.SetTile(new Vector3Int(x - chunkSize, currentHeight), belowSlopeLeft);
            }
            else if (currentHeight > lastHeight)
            {
                heightMap[x - 1] = currentHeight;
                if (!playerChanges.Any(v => groundTilemap.WorldToCell(v).Equals(new Vector3Int(x - chunkSize - 1, currentHeight))))
                    groundTilemap.SetTile(new Vector3Int(x - chunkSize - 1, currentHeight), slopeRight);
                if (!playerChanges.Any(v => groundTilemap.WorldToCell(v).Equals(new Vector3Int(x - chunkSize - 1, lastHeight))))
                    groundTilemap.SetTile(new Vector3Int(x - chunkSize - 1, lastHeight), belowSlopeRight);
            }

            lastHeight = currentHeight;
        }

        return heightMap;
    }

    void PlaceFeatures(int[] heightMap)
    {
        for (int x = -chunkSize, flatSize; x < chunkSize - 1; x += flatSize)
        {
            flatSize = 1;
            Vector3Int currentCellPos = featuresTilemap.WorldToCell(groundTilemap.CellToWorld(new Vector3Int(x, heightMap[x + chunkSize] + 1)));
            if (featuresLayer.Any(v => v.Key.Equals(currentCellPos)))
            {
                // Tile feature = featuresLayer.First(v => v.Key.Equals(currentCellPos)).Value;
                // if (feature != null)
                //     featuresTilemap.SetTile(currentCellPos, feature);
                continue;
            }

            for (int i = x + 1, j = 1; i < chunkSize; i++, j++)
            {
                if (featuresLayer.Any(v => v.Key.Equals(new Vector3Int(currentCellPos.x + j, currentCellPos.y))))
                    break;

                if (heightMap[i + chunkSize] == heightMap[x + chunkSize])
                    flatSize++;
                else
                    break;
            }

            FeatureTile[] potentialFeatures = features.ToList().FindAll(v => v.width <= flatSize).OrderBy(v => v.width).Reverse().ToArray();
            if (potentialFeatures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    featuresLayer.Add(new Vector3Int(currentCellPos.x + i, currentCellPos.y), null);
                continue;
            }

            float chance = Random.value;
            if (chance <= 0.242 && flatSize >= 8)
                Debug.Log(chance);

            potentialFeatures = potentialFeatures.ToList().FindAll(v => v.spawnChance >= chance).ToArray();
            if (potentialFeatures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    featuresLayer.Add(new Vector3Int(currentCellPos.x + i, currentCellPos.y), null);
                continue;
            }

            Tile selectedFeature = potentialFeatures[0].tile;
            featuresTilemap.SetTile(currentCellPos, selectedFeature);

            featuresLayer.Add(currentCellPos, selectedFeature);
            for (int i = 1; i < flatSize; i++)
                featuresLayer.Add(new Vector3Int(currentCellPos.x + i, currentCellPos.y), null);
        }
    }

    void PlaceLoot(int[] heightMap)
    {
        for (int x = -chunkSize; x < chunkSize; x += minSpaceBetweenLootContainers)
        {
            int currentRegion = Mathf.FloorToInt((float)(x + xPos) / minSpaceBetweenLootContainers);
            if (regionsWithLoot.Contains(currentRegion))
                continue;

            System.Random rng = new(seed + currentRegion);
            // (Vector3 pos, GameObject container) = lootContainersHolder[currentRegion];
            // Instantiate(container, pos, Quaternion.identity, transform);

            if (rng.NextDouble() > 0.4)
            {
                regionsWithLoot.Add(currentRegion);
                continue;
            }

            // Surface
            {
                LootContainer[] surfaceContainers = lootContainers.ToList().FindAll(x => !x.buried).ToArray();

                float chance = (float)rng.NextDouble();
                LootContainer[] potentialContainers = surfaceContainers.ToList().FindAll(x => x.spawnChance > chance).ToArray();

                if (potentialContainers.Length != 0)
                {
                    Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, heightMap[x + chunkSize] + 1));

                    DeleteTileAt(worldPos);
                    groundTilemap.SetTile(new Vector3Int(x, heightMap[x + chunkSize]), null);

                    int random = Random.Range(0, potentialContainers.Length);
                    GameObject container = Instantiate(potentialContainers[random].containerPrefab, worldPos + groundTilemap.tileAnchor, Quaternion.identity, transform);
                    container.GetComponent<LootChest>().lootTable = potentialContainers[random].lootTables[Random.Range(0, potentialContainers[random].lootTables.Length)];

                    regionsWithLoot.Add(currentRegion);
                    continue;
                }
            }

            // Buried
            {
                LootContainer[] buriedContainers = lootContainers.ToList().FindAll(x => x.buried).ToArray();

                float chance = (float)rng.NextDouble();
                LootContainer[] potentialContainers = buriedContainers.ToList().FindAll(x => x.spawnChance > chance).ToArray();

                if (potentialContainers.Length == 0)
                    continue;

                int yPos = Random.Range(chunkBottomLimit + 5, heightMap[x + chunkSize] - 3);
                Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, yPos));

                DeleteTileAt(worldPos);
                groundTilemap.SetTile(new Vector3Int(x, yPos), null);

                int random = Random.Range(0, potentialContainers.Length);
                GameObject container = Instantiate(potentialContainers[random].containerPrefab, worldPos + groundTilemap.tileAnchor, Quaternion.identity, transform);
                container.GetComponent<LootChest>().lootTable = potentialContainers[random].lootTables[Random.Range(0, potentialContainers[random].lootTables.Length)];

                regionsWithLoot.Add(currentRegion);
                break;
            }
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