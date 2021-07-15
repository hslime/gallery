using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    static public Main instance;

    public GameObject Player;
    [HideInInspector]
    public float DefaultPlayerHeight;

    public UITitle uiTitle;
    public UIGallery uiGallery;
    public GameObject uiJoystick;

    private bool isLock = false;


    private void Awake()
    {
        instance = this;

        uiTitle.gameObject.SetActive(true);
        uiGallery.gameObject.SetActive(false);
        uiJoystick.SetActive(false);

        DefaultPlayerHeight = Player.transform.position.y;
    }

    public bool IsLock()
    {
        return isLock;
    }

    public void Lock(bool isLock)
    {
        this.isLock = isLock;
    }
    
}
