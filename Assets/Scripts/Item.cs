using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Item : ScriptableObject
    {
        public TileBase tile;
        public itemType type;
        public InputActionType actionType;
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
            Mining
        } 

    }