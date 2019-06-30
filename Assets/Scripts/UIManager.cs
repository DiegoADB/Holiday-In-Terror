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

    public AudioClip playStart;
    public AudioClip hoverButton;
    public AudioClip pressButton;
    public AudioClip pressBack;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(PlayStart());
    }
    
    public void OnExitButtonClicked()
    {
        audioSource.PlayOneShot(pressButton, 0.7f);
        Application.Quit();
    }

    public void OnCreditButtonClicked()
    {
        audioSource.PlayOneShot(pressButton, 0.7f);
        b_creditsActive = !b_creditsActive;
        creditCanvas.SetActive(b_creditsActive);
        mainCanvas.SetActive(!b_creditsActive);
    }

    public void OnButtonHover()
    {
    }

    IEnumerator PlayStart()
    {
        audioSource.PlayOneShot(playStart, 0.7f);
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(MainGameScene);
    }

    
}
