using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TerrainGenerator : MonoBehaviour
{
    [Serializable]
    public class SpawnRule
    {
        public string label = "Rock";
        public GameObject prefab;

        [Min(0)] public int count = 100;
        [Min(0f)] public float minGarageDistance = 30f;
        [Min(0f)] public float minObjectDistance = 5f;
        [Min(0f)] public float edgeDistance = 10f;
        [UnityEngine.Range(0f, 90f)] public float maxSlope = 35f;
        
        public bool alignToGround = true;
        public float yOffset = 0f;
        public Vector2 scaleRange = new Vector2(0.85f, 1.2f);
        
        [Min(1)] public int triesPerObject = 1;
    }
    
    private class Mountain
    {
        public Vector2 pos;
        public float radius;
        public float height;
        public float noiseX;
        public float noiseZ;
    }
    [Header("Seed")]
    public bool randomSeedOnStart = true;
    public int seed;

    [Header("References")]
    public Terrain terrain;
    public Transform player;
    public GameObject garagePrefab;
    public TerrainVisualGenerator terrainVisualGenerator;

    [Header("Map Size")]
    public float mapLength = 1000f;
    public float mapWidth = 1000f;
    public float mapHeight = 180f;

    [Header("Base Terrain")]
    public float baseHeight = 20f;

    [Header("Hills")]
    [Min(1)] public int hillOctaves = 4;
    public float hillFrequency = 2.2f;
    public float hillAmplitude = 22f;
    [Range(0.1f, 1f)] public float hillPersistence = 0.5f;
    public float hillLacunarity = 2f;

    [Header("Small Terrain Details")]
    public float detailFrequency = 12f;
    public float detailAmplitude = 4f;

    [Header("Mountains")]
    public bool generateMountains = true;
    [Min(0)] public int mountainCount = 30;
    public float mountainDistanceFromEdge = 30f;
    public float mountainDistanceFromGarage = 100f;
    public Vector2 mountainRadiusRange = new Vector2(120f, 200f);
    public Vector2 mountainHeightRange = new Vector2(20f, 40f);

    [Header("Mountain Shape")]
    [Range(1.1f, 3f)] public float foothillRadiusMultiplier = 1.8f;
    [Range(0.05f, 0.7f)] public float foothillHeightPercent = 0.4f;
    [Range(0.1f, 0.6f)] public float mountainTopRadiusPercent = 0.4f;
    [Range(0f, 0.4f)] public float mountainIrregularity = 0.12f;

    [Header("Garage")]
    public float garageDistanceFromEdge = 100f;
    public float garageFlatRadius = 22f;
    public float garageBlendRadius = 15f;
    public float garageYOffset = 0.05f;

    [Header("Player Spawn")]
    public Transform playerSpawnPoint;
    public LayerMask groundLayer;
    public float playerSpawnYOffset = 0.1f;

    [Header("Resource Spawning")]
    public SpawnRule[] spawnRules;
    private Random random;
    private TerrainData terrainData;
    private float[,] heights;
    private Vector2 garagePos;
    private Transform generatedRoot;
        
    private readonly List<Mountain> mountains = new List<Mountain>();

    private float hillX;
    private float hillZ;
    private float detailX;
    private float detailZ;

    void Start()
    {
        GenerateWorld();
    }
    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        if (!Setup())
        {
            return;
        }

        if (randomSeedOnStart)
        {
            seed = Environment.TickCount;
        }
            
        random =  new Random(seed);
        terrainData = terrain.terrainData;
        terrainData.size = new Vector3(mapWidth, mapHeight, mapLength);

        CreateGeneratedRoot();
        ClearGeneratedObjects();

        hillX = RandomFloat(0f, 10000f);
        hillZ = RandomFloat(0f, 10000f);
        detailX = RandomFloat(0f, 10000f);
        detailZ = RandomFloat(0f, 10000f);

        garagePos = RandomPoint(garageDistanceFromEdge);

        GenerateMountains();

        heights = GenerateHeights();
        FlattenGarage();
        
        terrainData.SetHeights(0, 0, heights);
        terrain.Flush();

        if (terrainVisualGenerator != null)
        {
            terrainVisualGenerator.GenerateVisuals(terrain, seed);
        }
        
        SpawnGarage();
        SpawnResources();
            
        Physics.SyncTransforms();
    }

    private bool Setup()
    {
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        if (terrain == null)
        {
            return false;
        }

        if (player == null)
        {
            return false;
        }

        if (garagePrefab == null)
        {
            return false;
        }
        return true;
    }

    private void ClearGeneratedObjects()
    {
        for (int i = generatedRoot.childCount - 1; i >= 0; i--)
        {
            GameObject child = generatedRoot.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }

    private void CreateGeneratedRoot()
    {
        if (generatedRoot != null)
        {
            return;
        }
            
        generatedRoot = transform.Find("GeneratedRoot");

        if (generatedRoot == null)
        {
            GameObject root = new GameObject("GeneratedRoot");
            root.transform.SetParent(transform);
            generatedRoot = root.transform;
        }
    }
        
    private Vector2 RandomPoint(float edge)
    {
        return new Vector2(RandomFloat(edge, mapWidth - edge), RandomFloat(edge, mapLength - edge));
    }
    
    private void GenerateMountains()
    { 
        mountains.Clear();

        if (!generateMountains)
        {
            return;
        }

        for (int i = 0; i < mountainCount; i++)
        {
            bool placed = true;

            for (int attempt = 0; attempt < 120; attempt++)
            {
                float radius = RandomFloat(mountainRadiusRange.x, mountainRadiusRange.y);

                float outerRadius = radius * foothillHeightPercent;
                
                Vector2 pos = RandomPoint(mountainDistanceFromEdge);

                if (!CanPlaceMountain(pos, radius, outerRadius))
                {
                    continue;
                }
                
                mountains.Add(new Mountain{pos = pos, radius = radius, height = RandomFloat(mountainHeightRange.x, mountainHeightRange.y), noiseX = RandomFloat(0f, 10000f),  noiseZ = RandomFloat(0f, 10000f) });

                placed = true;
                break;
            }
        }
    }

    private bool CanPlaceMountain(Vector2 pos, float radius, float outerRadius)
    {
        float garageSafeDistance = mountainDistanceFromGarage + garageFlatRadius + outerRadius;

        if (Vector2.Distance(pos, garagePos) < garageSafeDistance)
        {
            return false;
        }

        foreach (Mountain mountain in mountains)
        {
            float minDistance = (radius + mountain.radius) * 0.55f;
            
            if (Vector2.Distance(pos, mountain.pos) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    private float RandomFloat(float min, float max)
    {
        if (max < min)
        {
            float temp = min;
            min = max;
            max = temp;
        }
        return min + (float)random.NextDouble() * (max - min);
    }
        
    private void FlattenGarage()
    {
        int resolution = terrainData.heightmapResolution;
        int centerX = Mathf.RoundToInt(garagePos.x / mapWidth * (resolution - 1));
        int centerZ = Mathf.RoundToInt(garagePos.y / mapLength * (resolution - 1));
        float flatHeight = heights[centerZ,  centerX];
        float totalRadius = garageFlatRadius + garageBlendRadius;
        
        int rangeX = Mathf.CeilToInt(totalRadius / mapWidth * (resolution - 1));
        int rangeZ = Mathf.CeilToInt(totalRadius / mapLength * (resolution - 1));

        for (int z = centerZ - rangeZ; z <= centerZ + rangeZ; z++)
        {
            for (int x = centerX - rangeX; x <= centerX + rangeX; x++)
            {
                if (x < 0 || x >= resolution || z < 0 || z >= resolution)
                {
                    continue;
                }
                
                Vector2 point = new Vector2(x / (float)(resolution - 1) * mapWidth, z / (float)(resolution - 1) * mapLength);
                float distance = Vector2.Distance(point, garagePos);

                if (distance > totalRadius)
                {
                    continue;
                }

                if (distance <= garageFlatRadius)
                {
                    heights[z, x] = flatHeight;
                }
                else
                {
                    float blend = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(garageFlatRadius, totalRadius, distance));
                    heights[z, x] = Mathf.Lerp(flatHeight, heights[z, x], blend);
                }
            }
        }
    }

    private float[,] GenerateHeights()
    {
        int resolution = terrainData.heightmapResolution;

        float[,] result = new float[resolution, resolution];

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float u = x / (float)(resolution - 1);
                float v = z / (float)(resolution - 1);
                
                Vector2 point = new Vector2(u * mapWidth, v * mapLength);

                float mountainHeight = GetMountainHeight(point, out float flatTop);
                float height = baseHeight + GetHillHeight(u, v) + mountainHeight;

                if (flatTop > 0f)
                {
                    height = Mathf.Lerp(height, baseHeight + mountainHeight, flatTop);
                }
                result[z, x] = Mathf.Clamp01(height / mapHeight);
            }
        }
        return result;
    }

    private float GetHillHeight(float u, float v)
    {
        float bigHills = FractalNoise(u, v, hillX, hillZ, hillFrequency, hillOctaves, hillPersistence, hillLacunarity);
        float details = Mathf.PerlinNoise(detailX + u * detailFrequency, detailZ + v * detailFrequency);

        return (bigHills - 0.5f) * 2f * hillAmplitude + (details - 0.5f) * 2f * detailAmplitude;
    }

    private float GetMountainHeight(Vector2 point, out float flatTop)
    {
        float totalHeight = 0f;
        flatTop = 0f;

        foreach (Mountain mountain in mountains)
        {
            float outerRadius = mountain.radius * foothillRadiusMultiplier;
            float topRadius = Mathf.Max(5f, mountain.radius * mountainTopRadiusPercent);
            float distance = Vector2.Distance(point, mountain.pos);

            if (distance >= outerRadius) continue;

            float foothillShape = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(outerRadius, mountain.radius, distance));
            float height = mountain.height * foothillHeightPercent * foothillShape;

            if (distance < mountain.radius)
            {
                float coreShape = distance <= topRadius ? 1f : Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(mountain.radius, topRadius, distance));

                float slopeNoise = FractalNoise(point.x / mountain.radius, point.y / mountain.radius, mountain.noiseX, mountain.noiseZ, 3f, 3, 0.5f, 2f);
                float slopeBlend = Mathf.InverseLerp(topRadius, mountain.radius, distance);

                float noisyScale = Mathf.Lerp(1f - mountainIrregularity, 1f + mountainIrregularity, slopeNoise);
                float scale = Mathf.Lerp(1f, noisyScale, slopeBlend);

                height += mountain.height * (1f - foothillHeightPercent) * coreShape * scale;
            }

            totalHeight += height;

            if (distance <= topRadius)
            {
                flatTop = 1f;
            }
            else
            {
                float blendEnd = topRadius + mountain.radius * 0.15f;

                if (distance < blendEnd)
                {
                    float blend = 1f - Mathf.InverseLerp(topRadius, blendEnd, distance);
                    flatTop = Mathf.Max(flatTop, Mathf.SmoothStep(0f, 1f, blend));
                }
            }
        }
        return totalHeight;
    }

    private float FractalNoise(float x, float z, float offsetX, float offsetZ, float frequency, int octaves, float persistence, float lacunarity)
    {
        float total = 0f;
        float strength = 1f;
        float max = 0f;

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(offsetX + x * frequency, offsetZ + z * frequency) * strength;

            max += strength;
            strength *= persistence;
            frequency *= lacunarity;
        }
        return total / max;
    }

    private void SpawnGarage()
    {
        Vector3 pos = GetWorldPosition(garagePos.x, garagePos.y, garageYOffset);
        Quaternion rotation = Quaternion.Euler(0f, RandomFloat(0f, 360f), 0f);
        GameObject garage = Instantiate(garagePrefab, pos, rotation, CreateGroup("Garage"));

        if (playerSpawnPoint != null)
        {
            MovePlayer(GetSafePlayerSpawnPosition(playerSpawnPoint.position));
            return;
        }

        Vector3 fallbackSpawn = garage.transform.position + garage.transform.forward * 3f;
        MovePlayer(GetSafePlayerSpawnPosition(fallbackSpawn));
    }

    private void MovePlayer(Vector3 pos)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb == null)
        {
            player.position = pos;
            return;
        }

        rb.position = pos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private Transform CreateGroup(string name)
    {
        GameObject group = new GameObject(name);
        group.transform.SetParent(generatedRoot);

        return group.transform;
    }

    private Vector3 GetSafePlayerSpawnPosition(Vector3 wantedWorldPosition)
    {
        Vector3 rayStart = wantedWorldPosition + Vector3.up * 100f;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 300f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            return new Vector3(wantedWorldPosition.x, hit.point.y + GetPlayerRootGroundOffset() + playerSpawnYOffset, wantedWorldPosition.z);
        }

        float localX = wantedWorldPosition.x - terrain.transform.position.x;
        float localZ = wantedWorldPosition.z - terrain.transform.position.z;
        float u = Mathf.Clamp01(localX / mapWidth);
        float v = Mathf.Clamp01(localZ / mapLength);
        float groundHeight = terrainData.GetInterpolatedHeight(u, v);
        float groundY = terrain.transform.position.y + groundHeight;
        return new Vector3(wantedWorldPosition.x, groundY + GetPlayerRootGroundOffset() + playerSpawnYOffset, wantedWorldPosition.z);
    }
    
    private float GetPlayerRootGroundOffset()
    {
        CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();

        if (capsule == null)
        {
            return 0f;
        }

        float bottomLocalY = capsule.center.y - capsule.height * 0.5f;
        return -bottomLocalY;
    }
    
    private Vector3 GetWorldPosition(float x, float z, float yOffset)
    {
        float u = x / mapWidth;
        float v = z / mapLength;
        float groundHeight = terrainData.GetInterpolatedHeight(u, v);

        return new Vector3(terrain.transform.position.x + x, terrain.transform.position.y + groundHeight + yOffset, terrain.transform.position.z + z);
    }

    private void SpawnResources()
    {
        if (spawnRules == null)
        {
            return;
        }

        foreach (SpawnRule rule in spawnRules)
        {
            if (rule.prefab == null || rule.count <= 0)
            {
                continue;
            }

            Transform group = CreateGroup(rule.label);
            List<Vector2> placed = new List<Vector2>();

            for (int i = 0; i < rule.count; i++)
            {
                if (!TryFindResource(rule, placed, out Vector3 pos, out Quaternion rotation, out Vector2 localPos))
                {
                    continue;    
                }

                GameObject obj = Instantiate(rule.prefab, pos, rotation, group);
                obj.transform.localScale *= RandomFloat(rule.scaleRange.x, rule.scaleRange.y);

                placed.Add(localPos);
            }
        }
    }

    private bool TryFindResource(SpawnRule rule, List<Vector2> placed, out Vector3 worldPos, out Quaternion rotation, out Vector2 localPos)
    {
        for (int i = 0; i < rule.triesPerObject; i++)
        {
            Vector2 point = RandomPoint(rule.edgeDistance);

            if (Vector2.Distance(point, garagePos) < rule.minGarageDistance)
            {
                continue;
            }

            bool tooClose = false;

            foreach (Vector2 other in placed)
            {
                if (Vector2.Distance(point, other) >= rule.minObjectDistance)
                {
                    continue;
                }

                tooClose = true;
                break;
            }

            if (tooClose) continue;

            float u = point.x / mapWidth;
            float v = point.y / mapLength;

            Vector3 normal = terrainData.GetInterpolatedNormal(u, v);

            if (Vector3.Angle(normal, Vector3.up) > rule.maxSlope)
            {
                continue;
            }

            worldPos = GetWorldPosition(point.x, point.y, rule.yOffset);

            Quaternion yaw = Quaternion.Euler(0f, RandomFloat(0f, 360f), 0f);
            rotation = rule.alignToGround ? Quaternion.FromToRotation(Vector3.up, normal) * yaw : yaw;

            localPos = point;
            return true;
        }

        worldPos = Vector3.zero;
        rotation = Quaternion.identity;
        localPos = Vector2.zero;

        return false;
    }
}
