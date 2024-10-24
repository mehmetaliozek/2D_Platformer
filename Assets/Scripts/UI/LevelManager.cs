using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject winPanel;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PanelOpener(GameObject panel)
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        panel.SetActive(isPaused);
    }

    public void SceneLaoder(int sceneLoadOptionsIndex)
    {
        Time.timeScale = 1;
        int activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneLoadOptions sceneLoadOptions = (SceneLoadOptions)sceneLoadOptionsIndex;
        switch (sceneLoadOptions)
        {
            case SceneLoadOptions.Restart:
                SceneManager.LoadScene(activeSceneBuildIndex);
                break;
            case SceneLoadOptions.Next:
                if (SceneManager.sceneCountInBuildSettings == activeSceneBuildIndex + 1) SceneManager.LoadScene(0);
                else SceneManager.LoadScene(activeSceneBuildIndex + 1);
                break;
            case SceneLoadOptions.Main:
                SceneManager.LoadScene(0);
                break;
        }
    }

    public void CompleteLevel()
    {
        PanelOpener(winPanel);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
        {
            PanelOpener(gameOverPanel);
        }
        else if (other.CompareTag(Tag.Goblin))
        {
            other.gameObject.SetActive(false);
        }
    }
}