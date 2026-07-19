using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public string sceneName = "Simple";

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}