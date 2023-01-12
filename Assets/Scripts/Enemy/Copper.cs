using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copper : MonoBehaviour
{
    [SerializeField] private GameObject g_left;
    [SerializeField] private GameObject g_right;
    [SerializeField] private GameObject g_leftGrounded;
    [SerializeField] private GameObject g_rightGrounded;

    private PlayerGroundedHandler leftHandler;
    private PlayerGroundedHandler rightHandler;
    private PlayerGroundedHandler leftGroundedHandler;
    private PlayerGroundedHandler rightGroundedHandler;

    private bool left, leftGrounded, right, rightGrounded;

    // Start is called before the first frame update
    void Start()
    {
        leftHandler = g_left.GetComponent<PlayerGroundedHandler>();
        rightHandler = g_right.GetComponent<PlayerGroundedHandler>();
        leftGroundedHandler = g_leftGrounded.GetComponent<PlayerGroundedHandler>();
        rightGroundedHandler = g_rightGrounded.GetComponent<PlayerGroundedHandler>();
        leftHandler.OnGrounded += OnLeft;
        rightHandler.OnGrounded += OnRight;
        leftGroundedHandler.OnGrounded += OnLeftGround;
        rightGroundedHandler.OnGrounded += OnRightGround;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLeft()
    {
        
    }
    private void OnRight()
    {

    }
    private void OnLeftGround()
    {

    }
    private void OnRightGround()
    {

    }
}
