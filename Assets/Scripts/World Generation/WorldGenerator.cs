using System.Collections.Generic;
using System.Linq;
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
        // TODO: implement
        [Tooltip("A spawn chance of 0 will spawn only once when the world is created at the spawn point.")]
        [Range(0, 1)]
        public float spawnChance;
        public uint width;
    }

    [System.Serializable]
    public struct StructureObject
    {
        public GameObject structure;
        public Vector3 offset;
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
        public Vector3 size;
        [Range(0, 1)]
        public float spawnChance;
        public bool buried;
    }

    public LayerTile[] tiles;
    public FeatureTile[] features;
    public StructureObject[] structures;
    public LootContainer[] lootContainers;
    public Tile slopeRight;
    public Tile belowSlopeRight;
    public Tile slopeLeft;
    public Tile belowSlopeLeft;
    [Tooltip("The actual size of the chunk will be twice this value")]
    public int chunkSize = 25;
    public int heightMapSize = 50;
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

    public System.Action OnTerrainGenerated;
    [HideInInspector]

    List<Vector3> playerChanges = new();
    List<int> regionsWithLoot = new();
    List<int> featuresLayer = new();
    List<int> structuresLayer = new();
    // Dictionary<int, (Vector3, GameObject)> lootContainersHolder = new();
    // [HideInInspector]
    public int xPos;
    int previous_xPos;
    bool update = true;
    int previousStructures_xPos;
    int previousFeatures_xPos;
    int previousLoot_xPos;
    bool updateFeatures = true;
    bool updateStructures = true;
    bool updateLoot = true;
    bool spawnPlayer = false;
    bool gameStarted = false;
    int[] heightMap;

    public void StartGeneration()
    {
        spawnPlayer = true;
        Init();
        gameStarted = true;
    }

    public void StopGeneration()
    {
        gameStarted = false;
    }

    void Update()
    {
        if (gameStarted)
            Loop();
    }

    void Init()
    {
        xPos = 0;

        playerChanges.Clear();

        Random.InitState(seed);
    }

    public bool DeleteTileAt(Vector3 pos)
    {
        if (!playerChanges.Any(x => x.Equals(pos)))
        {
            playerChanges.Add(pos);
            update = true;
        }
        if (groundTilemap.GetTile(groundTilemap.WorldToCell(pos)) != null)
            return true;

        return false;
    }

    // public void ModifyTileAt(Vector3 pos, Tile newTile)
    // {
    //     if (!playerChanges.Any(x => x.Key.Equals(pos)))
    //     {
    //         playerChanges.Add(pos, newTile);
    //         update = true;
    //     }
    // }

    void Loop()
    {
        if (previous_xPos != xPos)
            update = true;

        if (Mathf.Abs(previousFeatures_xPos - xPos) >= 15)
        {
            updateFeatures = true;
            previousFeatures_xPos = xPos;
        }

        if (Mathf.Abs(previousStructures_xPos - xPos) >= 30)
        {
            updateStructures = true;
            previousStructures_xPos = xPos;
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
            updateStructures = false;
            updateLoot = false;
        }
    }

    void OnPlayerMove(Vector3 movement, Vector3 position, Vector3 velocity) => xPos = Mathf.RoundToInt(position.x);

    void BuildWorld()
    {
        groundTilemap.ClearAllTiles();
        groundTilemap.transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
        previous_xPos = xPos;

        CheckArguments();

        heightMap = new int[2 * heightMapSize];
        for (int x = -heightMapSize; x < heightMapSize; x++)
            heightMap[x + heightMapSize] = Mathf.RoundToInt(GetNoiseVal(x)); ;

        FillWorld(heightMap[(heightMapSize - chunkSize)..(heightMapSize + chunkSize)]);
        AddSlopes(heightMap[(heightMapSize - chunkSize)..(heightMapSize + chunkSize)]);

        if (updateFeatures)
            PlaceFeatures(heightMap);
        if (updateStructures)
            PlaceStructures(heightMap);
        if (updateLoot)
            PlaceLoot(heightMap);
        if (spawnPlayer)
        {
            spawnPlayer = false;
            while (groundTilemap == null || GameManager.Instance == null)
            {
                Debug.Log(groundTilemap);
            }
            GameManager.Instance.SpawnPlayer(groundTilemap.CellToWorld(new Vector3Int(0, heightMap[heightMapSize] + 3)));
            GameManager.Instance.player.GetComponent<PlayerMovement>().OnPlayerMove += OnPlayerMove;
        }

        OnTerrainGenerated?.Invoke();
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

    void FillWorld(int[] heightMap)
    {
        LayerTile[] _tiles = tiles.ToList().OrderBy(x => x.depth).Reverse().ToArray();

        for (int x = -chunkSize; x < chunkSize; x++)
        {
            int maxHeight = heightMap[x + chunkSize];
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


                System.Random rng = new(worldpos.GetHashCode());
                rng = new(seed + rng.Next() + xPos + x);
                groundTilemap.SetTile(new Vector3Int(x, y), availableTiles[rng.Next(availableTiles.Count)]);
            }
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
        for (int x = -heightMapSize, flatSize; x < heightMapSize - 1; x += flatSize + 1)
        {
            flatSize = 0;
            Vector3Int currentCellPos = featuresTilemap.WorldToCell(groundTilemap.CellToWorld(new Vector3Int(x, heightMap[x + heightMapSize] + 1)));
            if (featuresLayer.Any(v => v == currentCellPos.x))
            {
                // Tile feature = featuresLayer.First(v => v.Key.Equals(currentCellPos)).Value;
                // if (feature != null)
                //     featuresTilemap.SetTile(currentCellPos, feature);
                continue;
            }

            for (int i = x + 1, j = 1; i < heightMapSize; i++, j++)
            {
                if (featuresLayer.Any(v => v == (currentCellPos.x + j)))
                    break;

                if (heightMap[i + heightMapSize] == heightMap[x + heightMapSize])
                    flatSize++;
                else
                    break;
            }

            FeatureTile[] potentialFeatures = features.ToList().FindAll(v => v.width <= flatSize).ToArray();
            if (potentialFeatures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    featuresLayer.Add(currentCellPos.x + i);
                continue;
            }

            float chance = Random.value;

            potentialFeatures = potentialFeatures.ToList().FindAll(v => v.spawnChance >= chance).ToArray();
            if (potentialFeatures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    featuresLayer.Add(currentCellPos.x + i);
                continue;
            }

            int random = Random.Range(0, potentialFeatures.Length);
            Tile selectedFeature = potentialFeatures[random].tile;

            featuresTilemap.SetTile(currentCellPos, selectedFeature);

            for (int i = 0; i < flatSize; i++)
                featuresLayer.Add(currentCellPos.x + i);
        }
    }

    // duplicate function
    void PlaceStructures(int[] heightMap)
    {
        for (int x = -heightMapSize, flatSize; x < heightMapSize - 1; x += flatSize)
        {
            flatSize = 1;
            Vector3Int currentCellPos = featuresTilemap.WorldToCell(groundTilemap.CellToWorld(new Vector3Int(x, heightMap[x + heightMapSize] + 1)));
            if (structuresLayer.Any(v => v == currentCellPos.x))
            {
                // Tile feature = structuresLayer.First(v => v.Key.Equals(currentCellPos)).Value;
                // if (feature != null)
                //     structuresTilemap.SetTile(currentCellPos, feature);
                continue;
            }

            int heightJumps = 0;
            for (int i = x + 1, j = 1; i < heightMapSize; i++, j++)
            {
                if (structuresLayer.Any(v => v == (currentCellPos.x + j)))
                    break;

                if (heightMap[i + heightMapSize] != heightMap[x + heightMapSize])
                {
                    if (heightMap[i + heightMapSize] < heightMap[x + heightMapSize])
                        heightJumps--;
                    else
                        heightJumps++;

                    if (Mathf.Abs(heightJumps) > 1)
                        break;
                }
                flatSize++;
            }

            StructureObject[] potentialStructures = structures.ToList().FindAll(v => v.width <= flatSize).ToArray();
            if (potentialStructures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    structuresLayer.Add(currentCellPos.x + i);
                continue;
            }

            float chance = Random.value;

            potentialStructures = potentialStructures.ToList().FindAll(v => v.spawnChance >= chance).ToArray();
            if (potentialStructures.Length == 0)
            {
                for (int i = 0; i < flatSize; i++)
                    structuresLayer.Add(currentCellPos.x + i);
                continue;
            }

            int random = Random.Range(0, potentialStructures.Length);
            GameObject selectedStructure = potentialStructures[random].structure;
            flatSize = (int)potentialStructures[random].width;

            Instantiate(selectedStructure, groundTilemap.CellToWorld(new Vector3Int(x, heightMap[x + heightMapSize] + 1)) + potentialStructures[random].offset, Quaternion.identity, transform);

            for (int i = 0; i < flatSize; i++)
                structuresLayer.Add(currentCellPos.x + i);
        }
    }

    void PlaceLoot(int[] heightMap)
    {
        for (int x = -heightMapSize; x < heightMapSize; x += minSpaceBetweenLootContainers)
        {
            int currentRegion = Mathf.FloorToInt((float)(x + xPos) / minSpaceBetweenLootContainers);
            if (regionsWithLoot.Contains(currentRegion))
                continue;

            // (Vector3 pos, GameObject container) = lootContainersHolder[currentRegion];
            // Instantiate(container, pos, Quaternion.identity, transform);

            if (Random.value > 0.4)
            {
                regionsWithLoot.Add(currentRegion);
                continue;
            }

            // Buried
            {
                LootContainer[] buriedContainers = lootContainers.ToList().FindAll(x => x.buried).ToArray();

                float chance = Random.value;
                LootContainer[] potentialContainers = buriedContainers.ToList().FindAll(x => x.spawnChance > chance).ToArray();

                if (potentialContainers.Length != 0)
                {

                    int yPos = Random.Range(chunkBottomLimit + 5, heightMap[x + heightMapSize] - 3);
                    Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, yPos));

                    DeleteTileAt(worldPos);
                    groundTilemap.SetTile(new Vector3Int(x, yPos), null);

                    int random = Random.Range(0, potentialContainers.Length);
                    Vector3 size = potentialContainers[random].size;
                    GameObject container = Instantiate(potentialContainers[random].containerPrefab, worldPos + groundTilemap.tileAnchor + (size - Vector3.right - Vector3.up) / 2, Quaternion.identity, transform);
                    container.GetComponent<LootChest>().lootTable = potentialContainers[random].lootTables[Random.Range(0, potentialContainers[random].lootTables.Length)];

                    for (int i = 0; i < size.x; i++)
                    {
                        for (int j = 0; j < size.y; j++)
                        {
                            Vector2 pos = new(worldPos.x + i, worldPos.y + j);
                            DeleteTileAt(pos);
                            // groundTilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y), null);
                        }
                    }

                    regionsWithLoot.Add(currentRegion);
                    continue;
                }
            }

            // Surface
            {
                LootContainer[] surfaceContainers = lootContainers.ToList().FindAll(x => !x.buried).ToArray();

                int highestFlatSize = -1, highestFlatSizex = Mathf.Max(currentRegion * minSpaceBetweenLootContainers - xPos, -heightMapSize);
                for (int i = highestFlatSizex + 1, flatSize = 1, currentx = i; i < Mathf.Min((currentRegion + 1) * minSpaceBetweenLootContainers - xPos, heightMapSize); i++)
                {
                    if (heightMap[i + heightMapSize] == heightMap[i + heightMapSize - 1])
                        flatSize++;
                    else
                    {
                        if (flatSize > highestFlatSize)
                        {
                            highestFlatSize = flatSize;
                            highestFlatSizex = currentx;
                            currentx = i;
                            flatSize = 1;
                        }
                    }
                }

                LootContainer[] potentialContainers = surfaceContainers.ToList().FindAll(x => x.size.x <= highestFlatSize).ToArray();
                if (potentialContainers.Length == 0)
                    continue;

                float chance = Random.value;
                potentialContainers = surfaceContainers.ToList().FindAll(x => x.spawnChance > chance).ToArray();
                if (potentialContainers.Length == 0)
                    continue;

                Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(highestFlatSizex, heightMap[highestFlatSizex + heightMapSize] + 1));

                int random = Random.Range(0, potentialContainers.Length);
                Vector3 size = potentialContainers[random].size;
                GameObject container = Instantiate(potentialContainers[random].containerPrefab, worldPos + groundTilemap.tileAnchor + (size - Vector3.right - Vector3.up) / 2, Quaternion.identity, transform);
                container.GetComponent<LootChest>().lootTable = potentialContainers[random].lootTables[Random.Range(0, potentialContainers[random].lootTables.Length)];

                regionsWithLoot.Add(currentRegion);
                continue;
            }
        }
    }

    float GetNoiseVal(int x)
    {
        var noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        float noiseVal = 0, scl = scale / 10, amp = amplitude;
        for (int i = 1; i <= octaves; i++)
        {
            float rawVal = noise.GetNoise((x + offset.x + xPos) * scl, seed) + 1;
            rawVal = rawVal / 2 * amp + offset.y;

            scl *= frequency;
            amp *= lacunarity;

            noiseVal += rawVal;
        }

        return noiseVal;
    }

    float Remap(float val, float fromMin, float fromMax, float toMin, float toMax) => toMin + (val - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}