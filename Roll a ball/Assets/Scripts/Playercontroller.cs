using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class Playercontroller : MonoBehaviour
{
    // Sirealize fields lets your edit private variables in the Unity Editor
    [SerializeField, Tooltip("How fast the player moves")]
    private float _moveSpeed = 5.0f;

    [SerializeField, Tooltip("The force which the player can jump")]
    private float _jumpForce = 10.0f;
    
    [SerializeField, Tooltip("The force which the player is pulled back to the ground")]
    private float _gravity = 10.0f;

    [SerializeField, Tooltip("he CharactorController on this")]
    private CharacterController _pController;

    // The Current direction the player is moving in
    private Vector3 _moveDirection;


    // Start is called before the first frame update
    private void Start()
    {
        // Assisting the var to the CharacterController on the player
       _pController = GetComponent<CharacterController>(); 

    }

    // Update is called once per frame
    private void Update()
    {
        float _xInput = Input.GetAxis("Horizontal");
        float _zInput = Input.GetAxis("Vertical");
                                        //  x    y     z
        Vector3 movement = new Vector3 (_xInput, 0, _zInput);

        movement = transform.TransformDirection(movement) * _moveSpeed; // Coverts the Vector3 from local space to world space

        if (_pController.isGrounded) // If the player is on the ground ... 
        {
            _moveDirection = movement;

            if (Input.GetButton("Jump"))  
            {
                _moveDirection.y = _jumpForce; // If the player hits the space bar while grounded ...
            }
            
        } else
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }
    _pController.Move(_moveDirection * Time.deltaTime);
    }
}   
