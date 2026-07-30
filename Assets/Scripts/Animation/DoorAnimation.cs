using System;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    
    private Animator animator;

    private bool isOpen = false;
    
    public static DoorAnimation instance;

    void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleDoor()
    {
        if (!isOpen)
        {
            OpenDoor();
        }
        else if (isOpen)
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        animator.SetBool("isOpen", true);
        isOpen = true;
    }

    void CloseDoor()
    {
        animator.SetBool("isOpen", false);
        isOpen = false;
    }
}
