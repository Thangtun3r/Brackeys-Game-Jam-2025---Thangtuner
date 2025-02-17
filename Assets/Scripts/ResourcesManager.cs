using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public int _coins;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the manager persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    public int GetCoins()
    {
        return _coins;
    }

    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            _coins += amount;
            Debug.Log($"Coins increased: {_coins}");
        }
    }

    public bool SpendCoins(int amount)
    {
        if (amount > 0 && _coins >= amount)
        {
            _coins -= amount;
            Debug.Log($"Coins spent: {_coins}");
            return true;
        }
        Debug.LogWarning("Not enough coins or invalid amount.");
        return false;
    }
}