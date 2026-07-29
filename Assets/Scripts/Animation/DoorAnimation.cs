using System;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    
    private Animator animator;

    private bool isOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
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
