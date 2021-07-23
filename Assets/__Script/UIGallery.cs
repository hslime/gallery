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
        Main.instance.PlaySelect();
        PictureFrame.NextPicture(false);
    }

    public void OnClickNext()
    {
        Debug.Log("OnClickNext()");
        Main.instance.PlaySelect();
        PictureFrame.NextPicture(true);
    }

    public void OnClickClose()
    {
        Debug.Log("OnClickClose()");
        Main.instance.PlayClose();
        gameObject.SetActive(false);
    }
}
