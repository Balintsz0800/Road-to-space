using System;
using UnityEngine;

public class Mining : MonoBehaviour
{
    public static Mining instance;

    public float Range = 5f;
    public RaycastHit savedHit;
    
    public bool isAttacking = false;

    private void Awake()
    {
        instance = this;
    }

    public void EndAttack()
    {
        isAttacking = false;
        Debug.Log("End attack");
    }
}
