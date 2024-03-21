using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSpriteSwitch : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<Sprite> _sprites;

    [SerializeField] private float _switchRateInSwitchesPerSecond;

    //--- Private Fields ------------------------

    private int _currentSpriteIdx = 0;
    private float _timeSinceLastSwitch = 0;

    //--- Unity Methods ------------------------

    private void Update()
    {
        _timeSinceLastSwitch += Time.deltaTime;
        if (_timeSinceLastSwitch >= 1 / _switchRateInSwitchesPerSecond)
        {
            if (_currentSpriteIdx == _sprites.Count() - 1)
            {
                _currentSpriteIdx = 0;
            }
            else
            {
                _currentSpriteIdx++;
            }

            _spriteRenderer.sprite = _sprites[_currentSpriteIdx];
            _timeSinceLastSwitch = 0;
        }
    }

    //--- Public Methods ------------------------

    //--- Private Methods ------------------------
}
