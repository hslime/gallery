using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    public CanvasGroup MainTitle;
    public float MainTitleDisappearTime = 1f;

    public CanvasGroup Guide;
    public float GuideDurationTime = 3f;
    public float GuideDisappearTime = 1.5f;


    private Animator uiAnimator;
    

    Coroutine onDisappear;


    private void Awake()
    {
        uiAnimator = GetComponent<Animator>();

        Guide.alpha = 0f;

        onDisappear = null;
    }

    private void Start()
    {
        Main.instance.Lock(true);
    }

    public void OnClickBackground()
    {
        if (onDisappear != null)
            return;

        Debug.Log("OnClickBackground()");

        Main.instance.PlaySelect();

        //if (IsPlayingAnim() == true)
        //    return;

        onDisappear = StartCoroutine(OnDisappear());
    }

    private bool IsPlayingAnim()
    {
        AnimatorStateInfo info;
        if (uiAnimator.IsInTransition(0))
            info = uiAnimator.GetNextAnimatorStateInfo(0);
        else
            info = uiAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime < 1f)
            return true;

        return false;
    }

    IEnumerator OnDisappear()
    {
        Debug.Log(string.Format("OnDisappear() begin {0}", Time.time));

        //Main.instance.Lock(true);

        float ratio = 1f;
        Animator camAnimator = Camera.main.GetComponent<Animator>();

        float time = MainTitleDisappearTime;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            ratio = Mathf.Max(0f, time / MainTitleDisappearTime);

            MainTitle.alpha = ratio;
            camAnimator.speed = ratio;

            yield return null;
        }

        camAnimator.enabled = false;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;


        // 
        Guide.alpha = 1f;

        yield return new WaitForSeconds(GuideDurationTime);

        time = GuideDisappearTime;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            ratio = Mathf.Max(0f, time / MainTitleDisappearTime);

            Guide.alpha = ratio;

            yield return null;
        }

        Guide.alpha = 0f;


        gameObject.SetActive(false);
        Main.instance.Lock(false);

        Main.instance.uiJoystick.SetActive(true);

        onDisappear = null;

        Debug.Log(string.Format("OnDisappear() end {0}", Time.time));
    }
}
