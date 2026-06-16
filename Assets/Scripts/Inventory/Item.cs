using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
    {
        public ItemType itemtype;
        public ActionType actionType;
        public GameObject itemPrefab;
        public GameObject handPrefab;
        public int durability;
        
        public bool stackable = true;
        public Sprite image;

        public enum ItemType
        {
            Rock,
            Iron,
            Fuel,
            Tool,
            BuildingPart
        }

        public enum ActionType
        {
            Mining,
        } 

    }