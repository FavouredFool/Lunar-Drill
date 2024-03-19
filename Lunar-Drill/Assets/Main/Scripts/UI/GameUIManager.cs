using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] private OptionsMenuUtilities _optionsMenuUtilities;

    //--- Private Fields ------------------------

    //--- Unity Methods ------------------------

    //--- Public Methods ------------------------

    /* Opens options panel. */
    public void OpenOptions()
    {
        _optionsMenuUtilities.Open();
    }

    //--- Private Methods ------------------------
}
