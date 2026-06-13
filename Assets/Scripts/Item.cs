using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
    {
        public TileBase tile;
        public itemType type;
        public ActionType actionType;
        public Vector2Int range = new Vector2Int(5, 4);
        
        public bool stackable = true;
        
        public Sprite image;

        public enum itemType
        {
            Tool,
            BuildingPart
        }

        public enum ActionType
        {
            Mining,
        } 

    }