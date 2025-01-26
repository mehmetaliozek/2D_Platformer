using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private Sprite pause;

    [SerializeField]
    private Sprite play;

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

    public void ChangeButtonImage(Button button)
    {
        if (isPaused)
        {
            button.image.sprite = play;
        }
        else
        {
            button.image.sprite = pause;
        }
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

    IEnumerator GameOverCoroutine(float duration)
    {
        float elapsed = 0f;

        gameOverPanel.SetActive(true);

        Image gameOverImage = gameOverPanel.GetComponent<Image>();
        Color startedColor = gameOverImage.color.WithAlpha(1);
        TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();

        while (elapsed <= duration)
        {
            gameOverImage.color = Color.black.WithAlpha(Mathf.Clamp01(elapsed / duration));
            gameOverText.color = Color.white.WithAlpha(elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameOverImage.color = startedColor;
        gameOverText.color = Color.white;
        yield return new WaitForSeconds(duration);
        SceneLaoder(1);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case Tag.Player:
                StartCoroutine(GameOverCoroutine(1f));
                break;
            case Tag.Goblin:
                collision.gameObject.SetActive(false);
                collision.gameObject.GetComponent<Goblin>().RemoveThis();
                break;
        }
    }
}