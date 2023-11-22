using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitRemover : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;
    void removeHit()
    {
        handAnimator.SetBool("hit", false);
    }
}
