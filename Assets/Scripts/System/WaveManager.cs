using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int poolOneCount; // Renamed as "Minions"
        public int poolTwoCount; // Renamed as "Tanks"
        public float phaseTwoDelay = 2f;
        public float phaseTwoDuration = 10f;

        public override string ToString()
        {
            return "Wave";
        }
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    private int currentWaveIndex = 0; // 0-based index (Wave 1)
    private bool waveReady = true;
    private bool phaseTwoActive = false;
    private int timeLeft; // Time left in Phase 2

    [Header("References")]
    public EnemySpawner enemySpawner;
    public TMP_Text waveNumberText; // Displays wave number
    public TMP_Text enemyCountText;  // Displays Minions & Tanks count
    public TMP_Text timeLeftText;    // Displays the countdown during Phase Two

    [Header("Wave Error UI")]
    public TMP_Text waveErrorText; // Error text for wave errors
    public Animator waveErrorAnimator; // Animator for error UI (should have bool parameter "isError")

    public static event Action OnWaveStart;
    public static event Action OnPhaseOneComplete;
    public static event Action OnWaveEnd;

    /// <summary>
    /// UI Button OnClick() to start the next wave.
    /// </summary>
    public void StartWaveFromButton()
    {
        // If the required tile path is not valid, show error and do not start wave.
        if (!TilemapAStarPathfinder.pathValid)
        {
            ShowWaveError("Not enough TILES REQUIRED");
            return;
        }
        // Otherwise clear any previous error.
        ClearWaveError();

        if (currentWaveIndex >= waves.Length)
        {
            return;
        }

        if (waveReady)
        {
            Debug.Log($"Starting Wave {currentWaveIndex + 1}");
            StartCoroutine(StartWave(currentWaveIndex));
        }
        else
        {
            // You can add additional logic here if needed.
        }
    }

    /// <summary>
    /// Updates TMP texts for the next wave.
    /// </summary>
    private void UpdateWaveUI()
    {
        if (waveNumberText != null)
            waveNumberText.text = $"{currentWaveIndex + 1}";

        if (enemyCountText != null && currentWaveIndex < waves.Length)
        {
            Wave selectedWave = waves[currentWaveIndex];
            enemyCountText.text = $"{selectedWave.poolOneCount}\n{selectedWave.poolTwoCount}";
        }
    }

    private void Start()
    {
        waveReady = true;
        UpdateWaveUI();
    }

    private IEnumerator StartWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waves.Length)
        {
            Debug.LogWarning("Invalid wave index!");
            yield break;
        }

        waveReady = false;
        phaseTwoActive = false;

        Wave selectedWave = waves[waveIndex];
        Debug.Log($"Starting Wave {waveIndex + 1}: Minions = {selectedWave.poolOneCount}, Tanks = {selectedWave.poolTwoCount}");

        OnWaveStart?.Invoke();

        enemySpawner.StartWave(selectedWave.poolOneCount, selectedWave.poolTwoCount);

        yield break;
    }

    /// <summary>
    /// Called when Phase 1 ends (all enemies cleared).
    /// </summary>
    public void OnWavePhaseOneComplete()
    {
        if (phaseTwoActive) return;

        Debug.Log($"Phase 1 complete for Wave {currentWaveIndex + 1}. Phase 2 starts in {waves[currentWaveIndex].phaseTwoDelay} seconds.");
        OnPhaseOneComplete?.Invoke();

        StartCoroutine(StartPhaseTwo(waves[currentWaveIndex].phaseTwoDelay, waves[currentWaveIndex].phaseTwoDuration));
    }

    /// <summary>
    /// Starts Phase 2 after a delay and runs a countdown.
    /// </summary>
    private IEnumerator StartPhaseTwo(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"Phase 2 started for Wave {currentWaveIndex + 1}, lasting {duration} seconds.");

        phaseTwoActive = true;
        timeLeft = Mathf.CeilToInt(duration);

        StartCoroutine(UpdatePhaseTwoUI());

        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        EndWave();
    }

    /// <summary>
    /// Updates UI every second to show the remaining time in Phase 2.
    /// </summary>
    private IEnumerator UpdatePhaseTwoUI()
    {
        while (phaseTwoActive && timeLeft > 0)
        {
            if (timeLeftText != null)
            {
                timeLeftText.text = $"{timeLeft} seconds";
            }
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Ends the wave and automatically prepares for the next wave.
    /// This is your original final function, which has been preserved.
    /// </summary>
    private void EndWave()
    {
        Debug.Log($"Wave {currentWaveIndex + 1} ended.");
        phaseTwoActive = false;
        waveReady = true;

        OnWaveEnd?.Invoke();

        if (currentWaveIndex + 1 < waves.Length)
        {
            currentWaveIndex++;
            UpdateWaveUI();
            Debug.Log($"Next wave is Wave {currentWaveIndex + 1}. Press the button to start.");
        }
        else
        {
            Debug.Log("All waves completed! No more waves left.");
        }
    }

    // Helper functions to update the wave error UI
    void ShowWaveError(string msg)
    {
        if (waveErrorText != null)
            waveErrorText.text = msg;

        if (waveErrorAnimator != null)
            waveErrorAnimator.SetBool("isError", true);
        StartCoroutine(swithcOffAnimation());
    }

    void ClearWaveError()
    {
        if (waveErrorText != null)
            waveErrorText.text = "";
    }

    IEnumerator swithcOffAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        if (waveErrorAnimator != null)
            waveErrorAnimator.SetBool("isError", false);
    }
}
