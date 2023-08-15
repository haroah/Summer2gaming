using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance{get; private set;} // singletoen
    private bool _gameHasEnded; // True or False if game has ended
    public TextMeshProUGUI timerText; // text object to display the timer
    [SerializeField]
    private float _gameTimer = 0; // Tracker for how long the player is playing the level
    [SerializeField] private GameObject _gameOverUI;
    public float highScore;

    private void Awake()
    {
        if (Instance != null) // if our Instace is not empty
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start() // Start is called before the first frame update
    {
        highScore = PlayerPrefs.GetFloat("Highscore", 0);
        _gameHasEnded = false;
        _gameTimer = 0;
        timerText.text = _gameTimer.ToString("F3");
        _gameOverUI.SetActive(false);
    }

    void Update()
    {
        if(_gameHasEnded) return;
        _gameTimer += 1 * Time.deltaTime;
        timerText.text = _gameTimer.ToString("F3");

    }

    public void EndGame() // Called by player when dies
    {
        if (_gameHasEnded == false)
        {
            _gameHasEnded = true;
            _gameOverUI.SetActive(true);

            // Save Highscore if aplicable
            SaveScore();
        }
    }

    private void SaveScore()
    {
        // Grab the current high score
        float highScore = PlayerPrefs.GetFloat("Highscore", 0);
        // If current score is better that the high score
        if (_gameTimer < highScore);
        {
            // save the new Highsore
            PlayerPrefs.SetFloat("Highscore", _gameTimer);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reloads current sceen
    }
}
