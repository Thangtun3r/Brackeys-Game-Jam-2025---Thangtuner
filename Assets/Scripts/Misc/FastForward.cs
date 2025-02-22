using UnityEngine;

public class FastForward : MonoBehaviour
{
    [SerializeField] private float fastForwardSpeed = 3.0f; // Speed multiplier when fast forwarding
    private float normalTimeScale = 1.0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.F)) // Hold 'F' to fast forward
        {
            Time.timeScale = fastForwardSpeed;
        }
        else
        {
            Time.timeScale = normalTimeScale;
        }

        // Ensures fixed updates keep working properly
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}