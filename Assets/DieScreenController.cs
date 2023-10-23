using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DieScreenController : MonoBehaviour
{
    [SerializeField]
    Image blackground;
    [SerializeField]
    TextMeshProUGUI text;

    Color _bgAlpha;
    Color _textAlpha;

    float timer = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _bgAlpha = blackground.color;
        _bgAlpha.a = 0.75f;

        _textAlpha = text.color;
        _textAlpha.a = 1f;
    }

    // Update is called once per frame
    void Update()
    { 
        blackground.color = Vector4.MoveTowards(blackground.color, _bgAlpha, Time.deltaTime);
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            text.fontSize += Time.deltaTime * 5;
            text.color = Vector4.MoveTowards(text.color, _textAlpha, Time.deltaTime);
        }
    }
}
