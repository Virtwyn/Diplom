using TMPro;
using UnityEngine;

public class KeyCount : MonoBehaviour
{
    public static KeyCount Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private string keyTextPrefix = "x ";

    [Header("Настройки")]
    [SerializeField] private int startKeys = 0;

    private int _currentKeys;

    public int CurrentKeys => _currentKeys;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _currentKeys = Mathf.Max(0, startKeys);
        RefreshUI();
    }

    public void AddKey(int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }

        _currentKeys += amount;
        RefreshUI();
    }

    public bool HasKeys(int amount = 1)
    {
        if (amount <= 0)
        {
            return true;
        }

        return _currentKeys >= amount;
    }

    public bool TrySpendKeys(int amount = 1)
    {
        if (!HasKeys(amount))
        {
            return false;
        }

        _currentKeys -= amount;
        RefreshUI();
        return true;
    }

    private void RefreshUI()
    {
        if (keyCountText != null)
        {
            keyCountText.text = keyTextPrefix + _currentKeys;
        }
    }
}
