using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    public float disappearTime = 1f;

    private Animator uiAnimator;
    private CanvasGroup group;

    Coroutine onDisappear;


    private void Awake()
    {
        uiAnimator = GetComponent<Animator>();
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;

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

        if (IsPlayingAnim() == true)
            return;

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

        float time = disappearTime;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            ratio = Mathf.Max(0f, time / disappearTime);

            group.alpha = ratio;
            camAnimator.speed = ratio;

            yield return null;
        }

        camAnimator.enabled = false;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;

        gameObject.SetActive(false);
        Main.instance.Lock(false);

        Main.instance.uiJoystick.SetActive(true);

        onDisappear = null;

        Debug.Log(string.Format("OnDisappear() end {0}", Time.time));
    }
}
