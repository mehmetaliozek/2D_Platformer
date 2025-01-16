using UnityEngine;

public class LevelScene : MonoBehaviour
{
    [SerializeField]
    private GameObject levelButton;
    [SerializeField]
    private GameObject levelPanel;

    private int openedLevelCount = 0;

    private void Start()
    {
        openedLevelCount = PlayerPrefs.GetInt(PlayerPrefKey.OpenedLevelCount, 1);
        for (int i = 1; i <= 10; i++)
        {
            GameObject lvlBtn = Instantiate(levelButton, levelPanel.transform);
            lvlBtn.GetComponent<LevelButton>().SetLevelButton(i, i <= openedLevelCount);
        }
    }
}
