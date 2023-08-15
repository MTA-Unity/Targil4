using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _ballStartPosition;
    [SerializeField] private Transform _ballRB;
    [SerializeField] private float _gameTimeInSeconds = 60f;
    
    [Header("Canvas")] 
    [SerializeField] private TextMeshProUGUI _timerTMP;
    [SerializeField] private TextMeshProUGUI _youWinTMP;
    [SerializeField] private TextMeshProUGUI _youLoseTMP;

    private bool _gameEnabled;
    private float _remainingTime;

    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        ResetGame();
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ResetGame();
        }

        if (_gameEnabled)
        {
            _remainingTime -= Time.deltaTime;
            if (_remainingTime > 0)
            {
                _timerTMP.text = $"Time Remaining: {_remainingTime:N2}";
            }
            else
            {
                LoseGame();
            }
        }
    }

    
    private void LoseGame()
    {
        SetGameEnabled(false);
        _youLoseTMP.gameObject.SetActive(true);
    }
    
    public void WinGame()
    {
        SetGameEnabled(false);
        _youWinTMP.gameObject.SetActive(true);
    }

    private void ResetGame()
    {
        _remainingTime = _gameTimeInSeconds;
        _ballRB.position = _ballStartPosition.position;
        GyroController.Instance.ResetOrientation();
        SetGameEnabled(true);
        
        _youWinTMP.gameObject.SetActive(false);
        _youLoseTMP.gameObject.SetActive(false);
    }

    private void SetGameEnabled(bool value)
    {
        _gameEnabled = value;
        GyroController.Instance.SetGameEnabled(value);
    }
}
