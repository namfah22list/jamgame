using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    public GameObject videoScreen;
    public VideoPlayer videoPlayer;

    public void StartGame()
    {
        videoScreen.SetActive(true);
        videoPlayer.Play();

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("jingjoak");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}