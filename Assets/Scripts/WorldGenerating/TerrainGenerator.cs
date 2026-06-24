using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TerrainGenerator : MonoBehaviour
{
    public class SpawnRule
    {
        public string groupName = "Rock";
        public GameObject prefab;

        [Min(0)] public int count = 100;
        [Min(0f)] public float minGarageDistance = 30f;
        [Min(0f)] public float minObjectDistance = 5f;
        [Min(0f)] public float edgeDistance = 10f;
        [UnityEngine.Range(0f, 90f)] public float maxSlope = 35f;
        
        public bool alignToGround = true;
        public float YOffset = 0f;
        public Vector2 scaleRange = new Vector2(0.85f, 1.2f);
        
        [Min(1)] public int triesPerObject = 1;
    }
    
    private class Mountain
    {
        public Vector2 pos;
        public float radius;
        public float height;
        public float noiseX;
        public float noiseY;
    }
        
    public bool randomSeedOnStart = true;
    public int seed;
        
    public Terrain terrain;
    public Transform player;
    public GameObject garagePrefab;

    public TerrainVisualGenerator terrainVisualGenerator;

    public float mapLength = 1000f;
    public float mapWidth = 1000f;
    public float mapHeight = 180f;

    public float baseHeight = 20f;
    [Min(1)] public int hillOctaves = 4;
    public float hillFrequency = 2.2f;
    public float hillAmplitude = 22f;
        
    [Range(0.1f, 1f)] public float hillPersistence = 0.5f;
        
    public float hillLacunarity = 2f;
    public float detailFrequency = 12f;
    public float detailAmplitude = 4f;
        
    public bool generateMountains = true;
    [Min(0)] public int mountainCount = 30;
    public float mountainDistanceFromEdge = 30f;
    public float mountainDistanceFromGarage = 100f;
        
    public Vector2 mountainRadiusRange = new Vector2(120f, 200f);
    public Vector2 mountainHeightRange = new Vector2(20f, 40f);
        
    [Range(1.1f, 3)] public float foothillRadiusMultiplier = 1.8f;
    [Range(0.05f, 0.7f)] public float foothillHeightMultiplier = 0.4f;
    [Range(0.1f, 0.6f)] public float mountainTopRadiusPercent = 0.4f;
    [Range(0f, 0.4f)] public float mountainIrregularity = 0.12f;

    public float garageDistanceFromEdge = 100f;
    public float garageFlatRadius = 22f;
    public float garageBlendRadius = 15f;
    public float garageYOffset = 0.05f;
    public string playerSpawnName = "PlayerSpawn";
        
    public SpawnRule[] spawnRules;
    private System.Random random;
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

    private void GenerateWorld()
    {
        if (!Setup())
        {
            return;
        }

        if (randomSeedOnStart)
        {
            seed = Environment.TickCount;
        }
            
        random =  new System.Random(seed);
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

        heights = GeneratedHeights();
        FlattenGarage();
            
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
        
    }

    private float RandomFloat(float f, float f1)
    {
        throw new NotImplementedException();
    }
        
    private void FlattenGarage()
    {
        throw new NotImplementedException();
    }

    private float[,] GeneratedHeights()
    {
        throw new NotImplementedException();
    }
}
