using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VFX_Blood : MonoBehaviour
{
    [SerializeField]
    private GameObject bloodParticle;

    [SerializeField]
    private Image canvasBloodGo;

    private GameObject tempBloodGo;


    public float bloodCanvasDuration = 3.0f;

    public Transform spawnBlood;

    public void InstantiateBlood(Transform parentTransform)
    {
        tempBloodGo = Instantiate(bloodParticle, parentTransform);
        StartCoroutine(FadeBlood());
    }

    IEnumerator FadeBlood()
    {
        Color fixedColor = canvasBloodGo.color;
        fixedColor.a = 1;
        canvasBloodGo.color = fixedColor;

        canvasBloodGo.CrossFadeAlpha(1, 0f, false);

        yield return new WaitForSeconds(bloodCanvasDuration);
        
        canvasBloodGo.CrossFadeAlpha(0, 1f, false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            InstantiateBlood(spawnBlood);
        }
    }
}
