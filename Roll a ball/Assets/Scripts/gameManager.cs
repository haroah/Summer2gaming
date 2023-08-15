using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    private bool _gameHasEnded; // True or False if game has ended
    public TextMeshProUGUI timerText; // text object to display the timer
    [SerializeField]
    private float _gameTimer = 0; // Tracker for how long the player is playing the level

    public float highScore;
    void Start() // Start is called before the first frame update
    {
        highScore = PlayerPrefs.GetFloat("Highscore", 0);
        _gameHasEnded = false;
        _gameTimer = 0;
        timerText.text = _gameTimer.ToString("F3");
    }

    void Update()
    {
        _gameTimer += 1 * Time.deltaTime;
        timerText.text = _gameTimer.ToString("F3");
    }

    public void EndGame() // Called by player when dies
    {
        if (_gameHasEnded == false)
        {
            _gameHasEnded = true;
            Invoke("ReloadCurrentScene", 2f);

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

    // Update is called once per frame
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reloads current sceen
    }
}
