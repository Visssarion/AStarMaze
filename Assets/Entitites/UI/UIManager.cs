
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Text _enemiesCounterText;
    [SerializeField] private GameObject _winScreen;
    private int _enemiesCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        InitializeEnemiesCounter(3);
    }

    public void InitializeEnemiesCounter(int totalEnemies)
    {
        _enemiesCount = totalEnemies;
        UpdateCounterText();
    }

    public void EnemyDead()
    {
        _enemiesCount--;
        UpdateCounterText();

        if (_enemiesCount <= 0)
        {
            ShowWinScreen();
        }
    }

    private void UpdateCounterText()
    {
        if (_enemiesCounterText != null)
        {
            _enemiesCounterText.text = $"Enemies left: {_enemiesCount}";
        }
    }

    private void ShowWinScreen()
    {
        if (_winScreen != null)
        {
            _winScreen.SetActive(true);
        }
    }
}