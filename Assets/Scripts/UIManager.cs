using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private string MainGameScene = "";
    [SerializeField]
    private GameObject creditCanvas;
    [SerializeField]
    private GameObject mainCanvas;

    private bool b_creditsActive = false;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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

    public void OnButtonHover()
    {
        audioSource.Play();
    }
}
