using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    private Vector2 pos;
    ScrollRect scroll;

    private void Start()
    {
        scroll = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        pos = new Vector2(0f, Time.deltaTime * 100f);
        scroll.content.localPosition = new Vector2(pos.x, scroll.content.localPosition.y + pos.y);
    }

    private void OnEnable()
    {
        scroll = GetComponent<ScrollRect>();
        scroll.content.localPosition = new Vector2(0, -750);
    }

}
