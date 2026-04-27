using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("jingjoak");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}