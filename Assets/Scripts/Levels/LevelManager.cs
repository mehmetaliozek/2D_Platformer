using System;
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
    private GameObject sceneLoadPanel;

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

    private void Start()
    {
        SetSceneLoadPanelTitle(SceneManager.GetActiveScene().name);
        StartCoroutine(FadeAndLoadSceneCoroutine(1.5f, 1f, 0f, -1));
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

    public void CompleteLevel()
    {
        PanelOpener(winPanel);
    }

    public void SetSceneLoadPanelTitle(string title)
    {
        sceneLoadPanel.GetComponentInChildren<TextMeshProUGUI>().text = title;
    }

    public void SceneLaoder(int sceneLoadOptionsIndex)
    {
        Time.timeScale = 1;
        int activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneLoadOptions sceneLoadOptions = (SceneLoadOptions)sceneLoadOptionsIndex;
        switch (sceneLoadOptions)
        {
            case SceneLoadOptions.Main:
                SceneManager.LoadScene(0);
                break;
            case SceneLoadOptions.Restart:
                SceneManager.LoadScene(activeSceneBuildIndex);
                break;
            case SceneLoadOptions.Next:
                if (SceneManager.sceneCountInBuildSettings == activeSceneBuildIndex + 1) SceneManager.LoadScene(0);
                else SceneManager.LoadScene(activeSceneBuildIndex + 1);
                break;
            case SceneLoadOptions.LoadDeactive:
                sceneLoadPanel.SetActive(false);
                break;
        }
    }

    IEnumerator FadeCoroutine(MaskableGraphic go, float startValue, float targetValue, float duration, Action callback)
    {
        float elapsed = 0;
        while (elapsed <= duration)
        {
            float clamp = Mathf.LerpUnclamped(startValue, targetValue, elapsed / duration);
            go.color = go.color.WithAlpha(clamp);
            elapsed += Time.deltaTime;
            yield return null;

        }
        go.color = go.color.WithAlpha(targetValue);
        callback();
    }

    IEnumerator FadeAndLoadSceneCoroutine(float duration, float startAlpha, float endAlpha, int sceneIndex)
    {
        CoroutineTracker ct = new CoroutineTracker();

        sceneLoadPanel.SetActive(true);
        int imageIndex = ct.Register();
        StartCoroutine(FadeCoroutine(sceneLoadPanel.GetComponent<Image>(), startAlpha, endAlpha, duration, () => ct.Complete(imageIndex)));

        int textIndex = ct.Register();
        StartCoroutine(FadeCoroutine(sceneLoadPanel.GetComponentInChildren<TextMeshProUGUI>(), startAlpha, endAlpha, duration, () => ct.Complete(textIndex)));

        yield return new WaitUntil(() => ct.AllCompleted());

        SceneLaoder(sceneIndex);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case Tag.Player:
                SetSceneLoadPanelTitle("Game Over");
                StartCoroutine(FadeAndLoadSceneCoroutine(1.5f, 0f, 1f, 1));
                break;
            case Tag.Goblin:
                collision.gameObject.SetActive(false);
                collision.gameObject.GetComponent<Goblin>().RemoveThis();
                break;
        }
    }
}