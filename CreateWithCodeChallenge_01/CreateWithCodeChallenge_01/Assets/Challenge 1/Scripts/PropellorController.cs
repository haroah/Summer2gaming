using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellorController : MonoBehaviour
{
    private float horizontalInput;
    private float turnSpeed = 45.0f;

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");


        // Rotate the player 
        transform.Rotate(Vector3.up, turnSpeed *horizontalInput *Time.deltaTime);
    }
}
