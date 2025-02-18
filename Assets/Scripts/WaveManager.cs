using System;
using System.Collections;
using UnityEngine;
using TMPro; // ✅ Import TextMeshPro

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int poolOneCount; // ✅ Rename as "Minions"
        public int poolTwoCount; // ✅ Rename as "Tanks"
        public float phaseTwoDelay = 2f;
        public float phaseTwoDuration = 10f;

        public override string ToString()
        {
            return "Wave";
        }
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    private int currentWaveIndex = 0; // ✅ Start at Wave 1 (0-based index)
    private bool waveReady = true;
    private bool phaseTwoActive = false;
    private int timeLeft; // ✅ Track time left in Phase 2 (now an int)

    [Header("References")]
    public EnemySpawner enemySpawner;
    public TMP_Text waveNumberText; // ✅ Text for wave number
    public TMP_Text unitCountText;  // ✅ Text for Minions & Tanks count

    public static event Action OnWaveStart;
    public static event Action OnPhaseOneComplete;
    public static event Action OnWaveEnd;

    /// <summary>
    /// UI Button OnClick() to start the **next wave in order**.
    /// </summary>
    public void StartWaveFromButton()
    {
        if (!TilemapAStarPathfinder.pathValid)
        {
            Debug.LogWarning("Wave cannot start! Path validation failed.");
            return;
        }

        if (currentWaveIndex >= waves.Length)
        {
            Debug.LogWarning("No more waves left. Game Completed!");
            return;
        }

        if (waveReady)
        {
            Debug.Log($"Starting Wave {currentWaveIndex + 1}");
            StartCoroutine(StartWave(currentWaveIndex));
        }
        else
        {
            Debug.LogWarning("Wave cannot start yet! Previous wave is still active.");
        }
    }

    /// <summary>
    /// Updates TMP texts for the next wave (before pressing Start Wave).
    /// </summary>
    private void UpdateWaveUI()
    {
        if (waveNumberText != null)
            waveNumberText.text = $"Wave {currentWaveIndex + 1}"; // ✅ Update wave number TMP

        if (unitCountText != null && currentWaveIndex < waves.Length)
        {
            Wave selectedWave = waves[currentWaveIndex];
            unitCountText.text = $"Minions: {selectedWave.poolOneCount}\nTanks: {selectedWave.poolTwoCount}"; // ✅ Update Minions & Tanks count TMP
        }
    }

    /// <summary>
    /// Starts the **next wave in order**.
    /// </summary>
    private void Start()
    {
        waveReady = true;
        UpdateWaveUI(); // ✅ Update UI at game start
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
    }

    /// <summary>
    /// Called when **Phase 1 ends (all enemies cleared)**.
    /// </summary>
    public void OnWavePhaseOneComplete()
    {
        if (phaseTwoActive) return;

        Debug.Log($"Phase 1 complete for Wave {currentWaveIndex + 1}. Phase 2 starts in {waves[currentWaveIndex].phaseTwoDelay} seconds.");
        OnPhaseOneComplete?.Invoke();

        StartCoroutine(StartPhaseTwo(waves[currentWaveIndex].phaseTwoDelay, waves[currentWaveIndex].phaseTwoDuration));
    }

    /// <summary>
    /// Starts Phase 2 with a delay and automatically ends the wave after duration.
    /// </summary>
    private IEnumerator StartPhaseTwo(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"Phase 2 started for Wave {currentWaveIndex + 1}, lasting {duration} seconds.");

        phaseTwoActive = true;
        timeLeft = Mathf.CeilToInt(duration); // ✅ Convert duration to an integer for countdown

        StartCoroutine(UpdatePhaseTwoUI()); // ✅ Start updating UI every second

        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        EndWave();
    }

    /// <summary>
    /// Updates TMP UI every second to show remaining survival time.
    /// </summary>
    private IEnumerator UpdatePhaseTwoUI()
    {
        while (phaseTwoActive && timeLeft > 0)
        {
            if (unitCountText != null)
            {
                unitCountText.text = $"Survive for: {timeLeft} seconds"; // ✅ Display rounded countdown
            }

            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Ends the wave and **automatically prepares the next wave**.
    /// </summary>
    private void EndWave()
    {
        Debug.Log($"Wave {currentWaveIndex + 1} ended.");
        phaseTwoActive = false;
        waveReady = true;

        OnWaveEnd?.Invoke();

        // ✅ Move to the next wave ONLY if it's not the last wave
        if (currentWaveIndex + 1 < waves.Length)
        {
            currentWaveIndex++;
            UpdateWaveUI(); // ✅ Update UI for the next wave
            Debug.Log($"Next wave is Wave {currentWaveIndex + 1}. Press the button to start.");
        }
        else
        {
            Debug.Log("All waves completed! No more waves left.");
        }
    }
}
