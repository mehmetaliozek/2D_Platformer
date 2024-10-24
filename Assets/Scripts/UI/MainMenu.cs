using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartButtonOnClick()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitButtonOnclick()
    {
        Application.Quit();
    }
}