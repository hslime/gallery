using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGallery : MonoBehaviour
{
    public SFPSC_FPSCamera fpsCamera;
    public SFPSC_PlayerMovement playerMovement;

    public RawImage rawImage;
    public Text title;
    public Text desc;
    public Image bg;

    public Vector2 origImageSize;

    static public UIGallery instance;

    private void Awake()
    {
        instance = this;

        origImageSize = rawImage.rectTransform.sizeDelta;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Debug.Log("GetMouseButtonDown(0)");
        }
        
    }

    public void OnClickBackGround()
    {
        Debug.Log("OnClickBackGround()");
        gameObject.SetActive(false);
    }
}
