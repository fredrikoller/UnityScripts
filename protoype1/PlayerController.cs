using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float Speed = 20.0f;
    private float turnSpeed = 45.0f;
    private float horizontalInput;
    private float forwardInput;

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Move the vehicle based on vertical input
        transform.Translate(Vector3.forward * Time.deltaTime * Speed * forwardInput);

        // Rotate the vehicle based on horizontal input
        transform.Rotate(Vector3.up,  turnSpeed * horizontalInput * Time.deltaTime);
    }
}
