using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSpriteIterator : MonoBehaviour
{
    #region --- Exposed Fields ---
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite[] _sprites;
    #endregion

    #region --- Public Fields ---
    public float Fps = 18;
    #endregion

    #region --- Private Fields ---
    float _timer = 0;
    int _index = 0;
    #endregion

    #region --- Unity Methods ---
    private void Update()
    {
        if (Time.timeScale == 0)
            _timer += Time.unscaledDeltaTime;
        else
            _timer += Time.deltaTime;

        _timer += Time.unscaledDeltaTime;
        if (_timer >= (float)1 / Fps)
        {
            _timer = 0;
            _index = (_index + 1) % _sprites.Length;
            _spriteRenderer.sprite = _sprites[_index];
        }
    }
    #endregion

}
