using UnityEngine;
using UnityEngine.Video;
public class CH_BrandSplashScreen : MonoBehaviour
{

    [SerializeField] private bool CH_isSplashScreenShowed = false;
    [SerializeField] private VideoPlayer CH_videoPlayer;
    [SerializeField] private VideoClip CH_videoClip;

    [SerializeField] private GameObject CH_canvas;
    [SerializeField] private GameObject AudioRain;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Update()
    {
        if (CH_videoPlayer.isPlaying == false)
        {
            TurnOn_Canvas();
            AudioRain.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    void TurnOn_Canvas()
    {
        CH_canvas.SetActive(true);
    }

}
