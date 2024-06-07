using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighlightTextOutlines : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _texts;
    [SerializeField] private List<Color> _textHighlightColors;



    /* Sets outline color of text given by index in list. */
    public void HighlightOutline(int idx)
    {
        _texts[idx].gameObject.SetActive(false); // have to do this, cause idk
        _texts[idx].outlineColor = _textHighlightColors[idx];
        _texts[idx].gameObject.SetActive(true);
    }

    /* Resets outline color of text given by index in list. */
    public void NoHighlightOutline(int idx)
    {
        _texts[idx].gameObject.SetActive(false); // have to do this, cause idk
        _texts[idx].outlineColor = _textHighlightColors[_textHighlightColors.Count - 1];
        _texts[idx].gameObject.SetActive(true);
    }
}
