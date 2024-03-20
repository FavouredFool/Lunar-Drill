using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDisplay : MonoBehaviour
{
    [SerializeField] UIHeart[] hearts;
    [SerializeField] bool reverse;

    public void RefreshHearts(int amount)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            bool on = i < amount;

            int index= reverse ? hearts.Length-1-i : i;

            hearts[index].Toggle(on);
        }
    }
}
