using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonController : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject creditsScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("SceneTest");
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
