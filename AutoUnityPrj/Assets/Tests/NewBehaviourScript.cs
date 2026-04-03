using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public float m_Speed = 5f;
    PlayerInput playerInput;
    public Vector2 moveInput;
    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }


    void FixedUpdate()
    {
        //Store user input as a movement vector
        Vector3 m_Input = playerInput.actions["Move"].ReadValue<Vector2>();
        m_Input.z = m_Input.y;
        m_Input.y = 0f;

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        m_Rigidbody.MovePosition(transform.position + m_Input * Time.fixedDeltaTime * m_Speed);
    }
}
