using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 6.0f;
    [SerializeField] bool LockCursor = true;
    [SerializeField] float walkspeed = 6.0f;

    float cameraPitch = 0.0f;
    CharacterController controller = null;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if(LockCursor);
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }


    void UpdateMouseLook()
    {
        Vector2 mouseDelta = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
    }

    public void Die()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the mouse
        Cursor.visible = true;                  // Make the mosue visible
        this.enabled = false;                   // Disbale this script
    }


    void UpdateMovement()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir.Normalize();
        
        Vector3 velocity = (transform.forward * inputDir.y + transform.right *  inputDir.x) * walkspeed;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "key fragments")
        {
            other.gameObject.SetActive(false);
            gameManager.Instance.IncreaseKeyCount();
        }
        
        if (other.gameObject.tag == "goal Door")
        {
           gameManager.Instance.CheckEndGame();
        }
    }
}
