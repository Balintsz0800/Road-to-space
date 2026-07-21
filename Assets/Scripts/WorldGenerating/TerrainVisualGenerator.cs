using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainVisualGenerator : MonoBehaviour
{
    [Header("Terrain Layers")]
    public TerrainLayer grassLayer;
    public TerrainLayer dirtLayer;
    public TerrainLayer rockLayer;

    [Header("Texture Resolution")]
    [Range(64, 512)]
    public int alphamapResolution = 256;

    [Header("Grass Texture Look")]
    [Range(0f, 1f)]
    public float baseGrassStrength = 1f;

    [Header("Dirt")]
    public float dirtNoiseFrequency = 5f;

    [Range(0f, 1f)]
    public float dirtNoiseThreshold = 0.72f;

    public float dirtSlopeStart = 14f;
    public float dirtSlopeEnd = 30f;

    [Range(0f, 1f)]
    public float dirtAmount = 0.25f;

    [Header("Rock")]
    public float rockSlopeStart = 35f;
    public float rockSlopeEnd = 58f;

    [Range(0f, 1f)]
    public float rockHeightStart = 0.68f;

    [Range(0f, 1f)]
    public float rockHeightEnd = 0.9f;

    [Range(0f, 1f)]
    public float rockAmount = 0.85f;

    [Header("Soft Grass Variation")]
    public float blendNoiseFrequency = 12f;

    [Range(0f, 1f)]
    public float blendNoiseAmount = 0.18f;

    [Header("Grass Model Spawning")]
    public bool spawnGrassModels = true;

    public GameObject grassPrefab;

    [Min(0)]
    public int grassCount = 700;

    [Min(1)]
    public int triesPerGrass = 50;

    public float grassDistanceFromEdge = 20f;
    public float minDistanceBetweenGrass = 2.5f;
    public float grassMaxSlope = 40f;

    [Range(0f, 1f)]
    public float grassMinHeight = 0f;

    [Range(0f, 1f)]
    public float grassMaxHeight = 1f;

    public bool useGrassNoise = true;
    public float grassNoiseFrequency = 9f;

    [Range(0f, 1f)]
    public float grassNoiseThreshold = 0.45f;

    public Vector2 grassScaleRange = new Vector2(0.8f, 1.25f);

    [Header("Grass Performance")]
    public bool disableGrassColliders = true;
    public bool disableGrassShadows = true;
    public bool makeGrassStatic = true;

    [Header("Terrain Performance")]
    public float terrainPixelError = 14f;
    public float terrainBaseMapDistance = 100f;
    public bool drawInstanced = true;

    private Transform grassRoot;
    private readonly List<Vector2> placedGrassPositions = new List<Vector2>();

    public void GenerateVisuals(Terrain targetTerrain, int worldSeed)
    {
        if (targetTerrain == null)
        {
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;

        ApplyPerformanceSettings(targetTerrain);
        PaintTerrain(terrainData, worldSeed);

        if (spawnGrassModels)
        {
            SpawnGrassModels(targetTerrain, terrainData, worldSeed);
        }
        else
        {
            ClearGrass();
        }

        targetTerrain.Flush();
    }

    private void ApplyPerformanceSettings(Terrain targetTerrain)
    {
        targetTerrain.heightmapPixelError = terrainPixelError;
        targetTerrain.basemapDistance = terrainBaseMapDistance;
        targetTerrain.detailObjectDistance = 0f;
        targetTerrain.detailObjectDensity = 0f;
        targetTerrain.drawInstanced = drawInstanced;
    }

    private void PaintTerrain(TerrainData terrainData, int worldSeed)
    {
        if (grassLayer == null || dirtLayer == null || rockLayer == null)
        {
            return;
        }

        int resolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(alphamapResolution), 64, 512);

        terrainData.alphamapResolution = resolution;
        terrainData.terrainLayers = new TerrainLayer[]
        {
            grassLayer,
            dirtLayer,
            rockLayer
        };

        float[,,] alphamaps = new float[resolution, resolution, 3];

        float dirtOffsetX = worldSeed * 0.031f;
        float dirtOffsetZ = worldSeed * 0.043f;
        float blendOffsetX = worldSeed * 0.061f;
        float blendOffsetZ = worldSeed * 0.079f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float normalizedX = x / (float)(resolution - 1);
                float normalizedY = y / (float)(resolution - 1);

                Vector3 normal = terrainData.GetInterpolatedNormal(normalizedX, normalizedY);
                float slope = Vector3.Angle(normal, Vector3.up);
                float normalizedHeight = terrainData.GetInterpolatedHeight(normalizedX, normalizedY) / terrainData.size.y;

                float dirtNoise = Mathf.PerlinNoise( dirtOffsetX + normalizedX * dirtNoiseFrequency, dirtOffsetZ + normalizedY * dirtNoiseFrequency);

                float blendNoise = Mathf.PerlinNoise( blendOffsetX + normalizedX * blendNoiseFrequency, blendOffsetZ + normalizedY * blendNoiseFrequency);

                float dirtNoiseWeight = SmoothRange(dirtNoise, dirtNoiseThreshold, 1f);
                float dirtSlopeWeight = SmoothRange(slope, dirtSlopeStart, dirtSlopeEnd);
                float dirtWeight = Mathf.Clamp01(dirtNoiseWeight * 0.65f + dirtSlopeWeight * 0.35f) * dirtAmount;

                float rockSlopeWeight = SmoothRange(slope, rockSlopeStart, rockSlopeEnd);
                float rockHeightWeight = SmoothRange(normalizedHeight, rockHeightStart, rockHeightEnd);
                float rockWeight = Mathf.Max(rockSlopeWeight, rockHeightWeight) * rockAmount;

                float softGrassVariation = Mathf.Lerp( 1f - blendNoiseAmount, 1f + blendNoiseAmount, blendNoise);

                float grassWeight = baseGrassStrength * softGrassVariation;

                dirtWeight *= 1f - rockWeight;
                grassWeight *= 1f - dirtWeight;
                grassWeight *= 1f - rockWeight;

                float total = grassWeight + dirtWeight + rockWeight;

                if (total <= 0f)
                {
                    alphamaps[y, x, 0] = 1f;
                    alphamaps[y, x, 1] = 0f;
                    alphamaps[y, x, 2] = 0f;
                    continue;
                }

                alphamaps[y, x, 0] = grassWeight / total;
                alphamaps[y, x, 1] = dirtWeight / total;
                alphamaps[y, x, 2] = rockWeight / total;
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    private void SpawnGrassModels(Terrain targetTerrain, TerrainData terrainData, int worldSeed)
    {
        if (grassPrefab == null)
        {
            ClearGrass();
            return;
        }

        CreateGrassRoot();
        ClearGrass();
        placedGrassPositions.Clear();

        System.Random random = new System.Random(worldSeed + 92817);

        float mapWidth = terrainData.size.x;
        float mapLength = terrainData.size.z;
        float noiseOffsetX = worldSeed * 0.021f;
        float noiseOffsetZ = worldSeed * 0.037f;

        int failedTooClose = 0;
        int failedHeight = 0;
        int failedSlope = 0;
        int failedNoise = 0;

        for (int i = 0; i < grassCount; i++)
        {
            bool spawned = false;

            for (int attempt = 0; attempt < triesPerGrass; attempt++)
            {
                float localX = RandomRange(random, grassDistanceFromEdge, mapWidth - grassDistanceFromEdge);
                float localZ = RandomRange(random, grassDistanceFromEdge, mapLength - grassDistanceFromEdge);
                Vector2 candidate = new Vector2(localX, localZ);

                if (IsGrassTooClose(candidate))
                {
                    failedTooClose++;
                    continue;
                }

                float normalizedX = localX / mapWidth;
                float normalizedZ = localZ / mapLength;
                float terrainHeight = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
                float normalizedHeight = terrainHeight / terrainData.size.y;

                if (normalizedHeight < grassMinHeight || normalizedHeight > grassMaxHeight)
                {
                    failedHeight++;
                    continue;
                }

                Vector3 normal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
                float slope = Vector3.Angle(normal, Vector3.up);

                if (slope > grassMaxSlope)
                {
                    failedSlope++;
                    continue;
                }

                if (useGrassNoise)
                {
                    float noise = Mathf.PerlinNoise(
                        noiseOffsetX + normalizedX * grassNoiseFrequency,
                        noiseOffsetZ + normalizedZ * grassNoiseFrequency
                    );

                    if (noise < grassNoiseThreshold)
                    {
                        failedNoise++;
                        continue;
                    }
                }

                Vector3 worldPosition = new Vector3( targetTerrain.transform.position.x + localX, targetTerrain.transform.position.y + terrainHeight, targetTerrain.transform.position.z + localZ
                );

                Quaternion rotation = Quaternion.Euler(0f, RandomRange(random, 0f, 360f), 0f);
                GameObject grass = Instantiate(grassPrefab, worldPosition, rotation, grassRoot);
                float scale = RandomRange(random, grassScaleRange.x, grassScaleRange.y);

                grass.transform.localScale *= scale;
                OptimizeGrassObject(grass);
                placedGrassPositions.Add(candidate);

                spawned = true;
                break;
            }

            if (!spawned)
            {
                continue;
            }
        }

        Debug.Log("TerrainVisualGenerator grass spawned: " + placedGrassPositions.Count + " | TooClose: " + failedTooClose + " | Height: " + failedHeight + " | Slope: " + failedSlope + " | Noise: " + failedNoise);
    }

    private void CreateGrassRoot()
    {
        Transform existing = transform.Find("GeneratedVisualGrass");

        if (existing != null)
        {
            grassRoot = existing;
            return;
        }

        GameObject root = new GameObject("GeneratedVisualGrass");
        root.transform.SetParent(transform);
        grassRoot = root.transform;
    }

    private void ClearGrass()
    {
        if (grassRoot == null)
        {
            Transform existing = transform.Find("GeneratedVisualGrass");

            if (existing != null)
            {
                grassRoot = existing;
            }
            else
            {
                return;
            }
        }

        for (int i = grassRoot.childCount - 1; i >= 0; i--)
        {
            GameObject child = grassRoot.GetChild(i).gameObject;

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

    private bool IsGrassTooClose(Vector2 candidate)
    {
        foreach (Vector2 placed in placedGrassPositions)
        {
            if (Vector2.Distance(candidate, placed) < minDistanceBetweenGrass)
            {
                return true;
            }
        }

        return false;
    }

    private void OptimizeGrassObject(GameObject grass)
    {
        if (disableGrassColliders)
        {
            Collider[] colliders = grass.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }

        if (disableGrassShadows)
        {
            Renderer[] renderers = grass.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        if (makeGrassStatic)
        {
            grass.isStatic = true;
        }
    }

    private float SmoothRange(float value, float start, float end)
    {
        if (Mathf.Approximately(start, end))
        {
            return value >= end ? 1f : 0f;
        }

        return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(start, end, value));
    }

    private float RandomRange(System.Random random, float min, float max)
    {
        if (max < min)
        {
            float temp = min;
            min = max;
            max = temp;
        }

        return min + (float)random.NextDouble() * (max - min);
    }
}