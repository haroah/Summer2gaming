using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class Playercontroller : MonoBehaviour
{
    // Sirealize fields lets your edit private variables in the Unity Editor
    [SerializeField, Tooltip("How fast the player moves")]
    private float _moveSpeed = 10.0f;

    [SerializeField, Tooltip("he CharactorController on this")]
    private CharacterController _pController;

    private int _score; 

    // The Current direction the player is moving in
    public Vector3 _moveDirection;



    // Start is called before the first frame update
    private void Start()
    {
        _score = 0; 
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

        _moveDirection = movement;
       
        _pController.Move(_moveDirection * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
   {
        switch (other.gameObject.tag)
        {
            case "Pickup":
                Destroy(other.gameObject);
                _score += 1;
                break;

            case "Enemy":
                FindObjectOfType<gameManager>().EndGame(); // Find the game manager and tell it to reset the scene
                gameObject.SetActive(false);
                break;
        }
   }

}   
