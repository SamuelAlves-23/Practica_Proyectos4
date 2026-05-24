using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonController : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject creditsScreen;

    public void LoadScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void CreditsScene()
    {
        creditsScreen.SetActive(true);
        titleScreen.SetActive(false);
    }

    public void TitleScene()
    {
        creditsScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

}
