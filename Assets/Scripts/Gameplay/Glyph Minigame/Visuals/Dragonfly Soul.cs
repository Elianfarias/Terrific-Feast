using System.Collections;
using UnityEngine;

public class DragonflySoul: MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void animationStart()
    {
        gameObject.SetActive(true);
        animator.Play("SoulDragonfly");
    } 
}
