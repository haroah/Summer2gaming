using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Load the 1st scene of t
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}
