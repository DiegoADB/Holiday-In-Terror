using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private string MainGameScene = "";
    [SerializeField]
    private GameObject creditCanvas;
    [SerializeField]
    private GameObject mainCanvas;

    private bool b_creditsActive = false;


    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(MainGameScene);
    }
    
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OnCreditButtonClicked()
    {
        b_creditsActive = !b_creditsActive;
        creditCanvas.SetActive(b_creditsActive);
        mainCanvas.SetActive(!b_creditsActive);
    }
}
