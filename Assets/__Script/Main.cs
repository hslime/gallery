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

    public float fadeBGM = 5f;
    public AudioSource BGM;
    public AudioSource Close;
    public AudioSource Select;

    private bool isLock = false;


    private void Awake()
    {
        instance = this;

        uiTitle.gameObject.SetActive(true);
        uiGallery.gameObject.SetActive(false);
        uiJoystick.SetActive(false);

        DefaultPlayerHeight = Player.transform.position.y;
        StartCoroutine(OnPlayBGM());
    }

    IEnumerator OnPlayBGM()
    {
        BGM.volume = 0f;
        BGM.Play();

        float delta = 0f;
        while (delta < fadeBGM)
        {
            delta += Time.deltaTime;
            BGM.volume = delta / fadeBGM;
            yield return null;
        }

        BGM.volume = 1f;
    }

    public bool IsLock()
    {
        return isLock;
    }

    public void Lock(bool isLock)
    {
        this.isLock = isLock;
    }

    public void PlayClose()
    {
        Close.Play();
    }

    public void PlaySelect()
    {
        Select.Play();
    }
}
