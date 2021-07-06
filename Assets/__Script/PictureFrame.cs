using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class PictureFrame : MonoBehaviour
{
    public string Title;
    [TextArea]
    public string Desc;

    public int Order;

    private Texture mainTexture;
    private Bounds bounds;
    private bool isInitBound = false;


    static List<PictureFrame> lists = new List<PictureFrame>();
    static PictureFrame current;

    private void Awake()
    {
        lists.Add(this);
        lists.OrderBy(x => x.Order);

        FindTexture();
    }

    public static void NextPicture(bool isNext)
    {
        if (current == null)
            return;

        int index = lists.IndexOf(current);
        index += (isNext) ? 1 : -1;

        if (index >= lists.Count)
            index = 0;
        if (index < 0)
            index = lists.Count - 1;

        current.ZoomStop();
        lists[index].ZoomStart();
    }

    void FindTexture()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (isInitBound == false)
            {
                isInitBound = true;
                bounds = renderers[i].bounds;
            }
            else
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            for (int m = 0; m < renderers[i].sharedMaterials.Length; ++m)
            {
                Material mat = renderers[i].sharedMaterials[m];
                if (mat == null)
                    continue;

                if (mat.mainTexture == null)
                    continue;

                if (mat.mainTexture.name.ToLower().StartsWith("type_") == false)
                    continue;

                mainTexture = mat.mainTexture;
            }
        }

        if (mainTexture == null)
        {
            Debug.LogWarning("failed found 'Type_' texture");
        }
    }

    bool isDown = false;

    private void OnMouseDown()
    {
        isDown = true;

        Debug.Log(string.Format("OnMouseDown {0}", name));
    }

    private void OnMouseUp()
    {
        if (isDown == false)
            return;
        isDown = false;

        if (Main.instance.IsLock() == true)
            return;

        Debug.Log(string.Format("OnMouseUp {0}", name));

        ZoomStart();
    }

    public void ZoomStart()
    {
        if (onZooming != null)
            ZoomStop();

        onZooming = StartCoroutine(OnZooming());
    }

    public void ZoomStop()
    {
        if (onZooming == null)
            return;

        StopCoroutine(onZooming);
        onZooming = null;
    }

    Coroutine onZooming = null;
    float posMultify = 1.2f;

    IEnumerator OnZooming()
    {
        //if (Main.instance.uiGallery.gameObject.activeSelf == true)
        //{
        //    yield break;
        //}

        current = this;

        Main.instance.Lock(true);
        var uiGallery = Main.instance.uiGallery;
        var uiJoystick = Main.instance.uiJoystick;

        Camera mcam = Camera.main;
        Transform camtrans = mcam.transform;

        Vector3 origPos = camtrans.position;
        Quaternion origRot = camtrans.rotation;
        
        AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        float elapsedTime = 0f;

        float size = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);

        Vector3 forward = transform.up;
        Vector3 targetPos = transform.position - forward * size * posMultify;
        Quaternion targetRot = Quaternion.LookRotation(forward);


        // zoom in

        uiJoystick.SetActive(false);

        while (elapsedTime < 1f)
        {
            float ratio = curve.Evaluate(elapsedTime);

            camtrans.position = Vector3.Lerp(origPos, targetPos, ratio);
            camtrans.rotation = Quaternion.Lerp(origRot, targetRot, ratio);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camtrans.position = targetPos;
        camtrans.rotation = targetRot;

        RectTransform rt = uiGallery.rawImage.rectTransform;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;
        float textureWidth = mainTexture.width;
        float textureHeight = mainTexture.height;
        float textureAspect = textureWidth / textureHeight;
        if (screenAspect < textureAspect)
        {
            rt.sizeDelta = new Vector2(screenWidth, screenWidth * textureHeight / textureWidth);
        }
        else
        {
            rt.sizeDelta = new Vector2(screenHeight * textureWidth / textureHeight, screenHeight);
        }
        
        //if (mainTexture.width > mainTexture.height)
        //{
        //    rt.sizeDelta = new Vector2(uiGallery.origImageSize.x, uiGallery.origImageSize.y * mainTexture.height / mainTexture.width);
        //}
        //else
        //{
        //    rt.sizeDelta = new Vector2(uiGallery.origImageSize.x * mainTexture.width / mainTexture.height, uiGallery.origImageSize.y);
        //}

        uiGallery.rawImage.texture = mainTexture;
        uiGallery.title.text = Title;
        uiGallery.desc.text = Desc;
        uiGallery.gameObject.SetActive(true);

        while (uiGallery.gameObject.activeSelf == true)
            yield return null;

        // zoom out

        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float ratio = curve.Evaluate(elapsedTime);

            camtrans.position = Vector3.Lerp(targetPos, origPos, ratio);
            camtrans.rotation = Quaternion.Lerp(targetRot, origRot, ratio);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Main.instance.Lock(false);
        uiJoystick.SetActive(true);

        yield return null;

        camtrans.position = origPos;
        camtrans.rotation = origRot;

        onZooming = null;
    }
}
