using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance{get; private set;} // singletoen
    private bool _gameHasEnded; // True or False if game has ended
    public TextMeshProUGUI timerText, gamefinishtimetext; // text object to display the timer
    [SerializeField]
    private float _gameTimer = 0; // Tracker for how long the player is playing the level
    [SerializeField] private GameObject _gameOverUI, _winGameUI;
    public float highScore;
    [SerializeField] private int playerkeys;
    [SerializeField] private int keysInLevel;
    [SerializeField] private GameObject goalDoor;
    [SerializeField] private Transform keyParentTransform;

    public void IncreaseKeyCount()
    {
        playerkeys += 1;
    }

    public void CheckEndGame()
    {
        // Check if player has all keys + End Game
        if (playerkeys == keysInLevel)
        {
            WinGame();
        }
        else 
        {
            // player doesnt have enough keys
            Debug.Log("Not enough Keys");
        }
    }


    private void Awake()
    {
        if (Instance != null) // if our Instace is not empty
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private int CountkeysInLevel()
    {
        int count = 0;
        // select Parent Keys, loop through children, count children
        foreach (Transform key in keyParentTransform) // For each Key object nested under the key parent object...
        {
            // Add 1 to the total amound of keys
            count += 1;
        }

        return count;
    }

    void Start() // Start is called before the first frame update
    {
        playerkeys = 0;
        keysInLevel = CountkeysInLevel();
        highScore = PlayerPrefs.GetFloat("Highscore", 0);
        _gameHasEnded = false;
        _gameTimer = 0;
        timerText.text = _gameTimer.ToString("F3");
        _gameOverUI.SetActive(false);
        _winGameUI.SetActive(false);
    }

    void Update()
    {
        if(_gameHasEnded) return;
        _gameTimer += 1 * Time.deltaTime;
        timerText.text = _gameTimer.ToString("F3");

    }

    public void WinGame() // Called by Manager when player checks for enough keys
     {
        if (_gameHasEnded == false)
        {
            _gameHasEnded = true;
            _winGameUI.SetActive(true);

            // Save Highscore if aplicable
            SaveScore();
            Cursor.lockState = CursorLockMode.None; // Unlock the mouse
            Cursor.visible = true;
            gamefinishtimetext.text = "Your Time Was " +_gameTimer.ToString("F3");
        }
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
