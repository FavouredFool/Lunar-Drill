using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class SelectScreen : MonoBehaviour
{
    //UI ELEMENTS

    public GameObject[] EmptyObjects, SoloObjects, MultiObjects;
    public GameObject[] LunaUp_DrillianDown, LunaDown_DrillianUp;

    public GameObject P1Ready, P1Swap;
    public GameObject P2Ready, P2Swap;

    public SpriteRenderer
        ReadySoloIndicator,
        ReadyIndicator,
        SwapIndicator;

    public void SetEmpty()
    {
        foreach (GameObject g in SoloObjects)
            g.SetActive(false);
        foreach (GameObject g in MultiObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(true);

        foreach (GameObject g in LunaUp_DrillianDown)
            g.SetActive(false);
        foreach (GameObject g in LunaDown_DrillianUp)
            g.SetActive(false);
    }
    public void SetSolo()
    {
        foreach (GameObject g in MultiObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(false);
        foreach (GameObject g in SoloObjects)
            g.SetActive(true);

        foreach (GameObject g in LunaUp_DrillianDown)
            g.SetActive(false);
        foreach (GameObject g in LunaDown_DrillianUp)
            g.SetActive(false);
    }
    public void SetMulti(bool Lup_Ddown)
    {
        foreach (GameObject g in SoloObjects)
            g.SetActive(false);
        foreach (GameObject g in EmptyObjects)
            g.SetActive(false);
        foreach (GameObject g in MultiObjects)
            g.SetActive(true);

        foreach (GameObject g in LunaUp_DrillianDown)
            g.SetActive(Lup_Ddown);
        foreach (GameObject g in LunaDown_DrillianUp)
            g.SetActive(!Lup_Ddown);
    }

    public void RefreshReady(bool p1, bool p2, float time)
    {
        P1Ready.SetActive(p1);
        P2Ready.SetActive(p2);

        ReadyIndicator.color = 
            Color.Lerp(Color.white, Color.magenta, time / ConnectManager.AgreeTime);
        ReadySoloIndicator.color = ReadyIndicator.color;
    }
    public void RefreshSwap(bool p1, bool p2, float time)
    {
        P1Swap.SetActive(p1);
        P2Swap.SetActive(p2);

        SwapIndicator.color = 
            Color.Lerp(Color.white, Color.magenta, time / ConnectManager.AgreeTime);
    }

    public void Play()
    {
        ReadyIndicator.color = Color.red;
        ReadySoloIndicator.color=Color.red;
        P1Ready.SetActive(false);
        P2Ready.SetActive(false);
    }
    public void Swap()
    {
        SwapIndicator.color = Color.white;
        P1Swap.SetActive(false);
        P2Swap.SetActive(false);
    }
}
