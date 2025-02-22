using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public float holdTime = 2.0f; // Time in seconds to hold 'R' before resetting
    private float holdTimer = 0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime)
            {
                ResetLevel();
            }
        }
        else
        {
            holdTimer = 0f; // Reset the timer if 'R' is released
        }
    }

    void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}