using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Button btn;

    [SerializeField]
    private GameObject lockImage;

    public void SetLevelButton(int levelIndex, bool isNotLocked)
    {
        text.text = levelIndex.ToString();
        if (isNotLocked)
        {
            lockImage.SetActive(false);
            btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene($"Level {levelIndex}");
            });
        }
    }
}
