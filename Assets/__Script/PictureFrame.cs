using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class PictureFrame : MonoBehaviour
{
    public string Title;
    [TextArea]
    public string Desc;

    private Texture mainTexture;

    private void Awake()
    {
        FindTexture();
    }

    private Bounds bounds;
    private bool isInitBound = false;

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
    }

    private void OnMouseUp()
    {
        if (isDown == false)
            return;
        isDown = false;

        Debug.Log(string.Format("OnMouseUp {0}", name));

        if (onTouched != null)
            return;

        onTouched = StartCoroutine(OnTouched());
    }

    Coroutine onTouched = null;
    float posMultify = 2f;

    IEnumerator OnTouched()
    {
        if (UIGallery.instance.gameObject.activeSelf == true)
        {
            yield break;
        }

        yield return null;

        UIGallery.instance.fpsCamera.enabled = false;
        UIGallery.instance.playerMovement.enabled = false;


        Camera mcam = Camera.main;
        Transform mtrans = mcam.transform;

        Vector3 origPos = mtrans.position;
        Quaternion origRot = mtrans.rotation;
        
        AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        float elapsedTime = 0f;

        float size = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);

        Vector3 forward = transform.up;
        Vector3 targetPos = transform.position - forward * size * posMultify;
        Quaternion targetRot = Quaternion.LookRotation(forward);

        Color origBgColor = UIGallery.instance.bg.color;
        Color targetBgColor = origBgColor;

        // zoom in

        while (elapsedTime < 1f)
        {
            float ratio = curve.Evaluate(elapsedTime);

            mtrans.position = Vector3.Lerp(origPos, targetPos, ratio);
            mtrans.rotation = Quaternion.Lerp(origRot, targetRot, ratio);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mtrans.position = targetPos;
        mtrans.rotation = targetRot;

        RectTransform rt = UIGallery.instance.rawImage.rectTransform;
        if (mainTexture.width > mainTexture.height)
        {
            rt.sizeDelta = new Vector2(UIGallery.instance.origImageSize.x, UIGallery.instance.origImageSize.y * mainTexture.height / mainTexture.width);
        }
        else
        {
            rt.sizeDelta = new Vector2(UIGallery.instance.origImageSize.x * mainTexture.width / mainTexture.height, UIGallery.instance.origImageSize.y);
        }


        UIGallery.instance.rawImage.texture = mainTexture;
        UIGallery.instance.title.text = Title;
        UIGallery.instance.desc.text = Desc;
        UIGallery.instance.gameObject.SetActive(true);

        while (UIGallery.instance.gameObject.activeSelf == true)
            yield return null;


        // zoom out

        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float ratio = curve.Evaluate(elapsedTime);

            mtrans.position = Vector3.Lerp(targetPos, origPos, ratio);
            mtrans.rotation = Quaternion.Lerp(targetRot, origRot, ratio);
            //bgColor = Vector3.Lerp(targetPos, origPos, ratio);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mtrans.position = origPos;
        mtrans.rotation = origRot;


        UIGallery.instance.fpsCamera.enabled = true;
        UIGallery.instance.playerMovement.enabled = true;

        onTouched = null;
    }
}
