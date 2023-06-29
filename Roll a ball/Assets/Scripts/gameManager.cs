using UnityEngine.SceneManagement;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    private bool _gameHasEnded; // True or False if game has ended
    
    void Start() // Start is called before the first frame update
    {
        _gameHasEnded = false;
    }

    public void EndGame() // Called by player when dies
    {
        if (_gameHasEnded == false)
        {
            _gameHasEnded = true;
            Invoke("ReloadCurrentScene", 2f);
        }
    }

    // Update is called once per frame
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reloads current sceen
    }
}
