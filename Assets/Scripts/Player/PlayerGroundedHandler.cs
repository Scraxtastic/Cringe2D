using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerGroundedHandler : MonoBehaviour
{
    [SerializeField] private string[] tagsForActivation = new string[] { "Map" };
    public event Action OnGrounded;
    protected void InvokeOnGrounded()
    {
        OnGrounded?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        foreach (string currentTag in tagsForActivation)
        {
            if (collision.CompareTag(currentTag))
            {
                InvokeOnGrounded();
                return;
            }
        }
    }

}
