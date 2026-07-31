using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool _isGrounded = false;
    private void OnTriggerEnter(Collider other)
    {
        _isGrounded = true;
        Debug.Log(other.name);
    }
    private void OnTriggerExit(Collider other)
    {
        _isGrounded = false;
    }
    public bool isGrounded() => _isGrounded;
}
