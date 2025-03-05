using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrcHitBox : MonoBehaviour
{
    private Orc parentOrc;

    void Start()
    {
        parentOrc = GetComponentInParent<Orc>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && parentOrc != null)
        {
            // Start attack when player enters detection range
            parentOrc.GetComponent<CapsuleCollider2D>().enabled = true;
        }
    }
}
