using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Animator animator;
    public Transform bossPosition;
    
    public void GoOpen()
    {
        animator.Play("Open");
    }

    public void GoClose()
    {
        animator.Play("Close");
    }

    
}
