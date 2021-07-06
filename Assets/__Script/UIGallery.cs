using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGallery : MonoBehaviour
{
    public RawImage rawImage;
    public Text title;
    public Text desc;
    public Image bg;

    private void Awake()
    {
    }

    public void OnClickPrev()
    {
        Debug.Log("OnClickPrev()");
        PictureFrame.NextPicture(false);
    }

    public void OnClickNext()
    {
        Debug.Log("OnClickNext()");
        PictureFrame.NextPicture(true);
    }

    public void OnClickClose()
    {
        Debug.Log("OnClickClose()");
        gameObject.SetActive(false);
    }
}
