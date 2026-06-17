using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
    {
        public ItemType itemtype;
        public GameObject itemPrefab;
        public GameObject handPrefab;
        public int durability;
        
        public Vector3 handPosition;
        public Vector3 handRotation;
        public Vector3 handScale = Vector3.one;
        
        public bool stackable = true;
        public Sprite image;

        public enum ItemType
        {
            Rock,
            Iron,
            Fuel,
            Tool,
            BuildingPart,
            Workbanch
        }
    }