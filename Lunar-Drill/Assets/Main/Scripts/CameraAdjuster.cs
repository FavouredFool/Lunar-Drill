using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public static CameraAdjuster main;
    [SerializeField]Camera[] cameras;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        Adjust();
    }

    Vector2 lastScreenResolution;
    bool lastFullScreen;
    private void Update()
    {
        Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
        bool fullScreen = Screen.fullScreen;

        if (screenResolution!=lastScreenResolution || fullScreen!=lastFullScreen)
            Adjust();

        lastScreenResolution = screenResolution;
        lastFullScreen = fullScreen;
    }

    public void Adjust()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 1 / 1;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        foreach (Camera camera in cameras)
        {
            Rect rect = camera.rect;

            // if scaled height is less than current height, add letterbox
            if (scaleheight < 1.0f)
            {
                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;

                camera.rect = rect;
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleheight;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }
        }
    }

}
