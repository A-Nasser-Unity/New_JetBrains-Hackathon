using UnityEngine;

public class PianoKey : MonoBehaviour
{
    [Header("Key Settings")]
    public KeyCode keyToPress = KeyCode.Space;
    public float pressedYPosition = -0.1f;
    
    private float originalYPosition;
    private float moveSpeed = 20f;
    private bool isPressed = false;

    void Start()
    {
        // Store the original Y position
        originalYPosition = transform.position.y;
    }

    void Update()
    {
        // Check if key is pressed
        if (Input.GetKeyDown(keyToPress))
        {
            isPressed = true;
        }
        
        // Check if key is released
        if (Input.GetKeyUp(keyToPress))
        {
            isPressed = false;
        }
        
        // Move the object based on key state
        float targetY = isPressed ? pressedYPosition : originalYPosition;
        Vector3 currentPos = transform.position;
        float newY = Mathf.MoveTowards(currentPos.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(currentPos.x, newY, currentPos.z);
    }
}