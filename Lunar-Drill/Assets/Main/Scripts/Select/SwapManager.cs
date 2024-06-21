using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour
{
    public void Swap()
    {
        if (PlayerConnectController.Swap())
        {
            PreparationInterface.instance.SwapPlayerInfo();
            Rumble.instance?.RumbleFeedback();
        }

    }
}
