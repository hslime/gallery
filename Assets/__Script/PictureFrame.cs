using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using UnityEngine.EventSystems;

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
        lists = lists.OrderBy(x => x.Order).ToList();

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
        lists[index].ZoomStart(true);
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
        Vector3 touchPos = Vector3.zero;
#if UNITY_EDITOR
        touchPos = Input.mousePosition;
#else
        if (Input.touchCount > 0)
            touchPos = Input.GetTouch(0).position;
        else
            return;
#endif

        if (IsPointerOverUIObject(touchPos) == true)
            return;

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

        Main.instance.PlaySelect();

        ZoomStart(false);
    }

    public bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }


    public void ZoomStart(bool isImmediate)
    {
        if (onZooming != null)
            ZoomStop();

        onZooming = StartCoroutine(OnZooming(isImmediate));
    }

    public void ZoomStop()
    {
        if (onZooming == null)
            return;

        StopCoroutine(onZooming);
        onZooming = null;

        var playerObj = Main.instance.Player;
        var playerTrans = playerObj.transform;

        Vector3 standingPos;
        Quaternion standingRot;
        GetPositionAndRotation(false, out standingPos, out standingRot);

        playerTrans.position = standingPos;
        playerTrans.rotation = standingRot;
    }

    Coroutine onZooming = null;

    void GetPositionAndRotation(bool isView, out Vector3 pos, out Quaternion rot)
    {
        float size = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
        Vector3 forward = transform.up;
        float multifly = (isView == true) ? 0.9f : 1.4f;
        pos = transform.position - forward * size * multifly;
        if (isView == false)
            pos.y = Main.instance.DefaultPlayerHeight;

        rot = Quaternion.LookRotation(forward);
    }

    IEnumerator OnZooming(bool isImmediate)
    {
        //if (Main.instance.uiGallery.gameObject.activeSelf == true)
        //{
        //    yield break;
        //}

        current = this;

        Main.instance.Lock(true);
        var uiGallery = Main.instance.uiGallery;
        var uiJoystick = Main.instance.uiJoystick;

        var playerObj = Main.instance.Player;
        var playerTrans = playerObj.transform;
        Vector3 origPos = playerTrans.position;
        Quaternion origRot = playerTrans.rotation;

        AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        float elapsedTime = 0f;

        Vector3 targetPos;
        Quaternion targetRot;
        GetPositionAndRotation(true, out targetPos, out targetRot);

        Camera cam = Camera.main;
        Transform camTrans = cam.transform;
        Quaternion camOrigRot = camTrans.localRotation;
        Quaternion camTargetRot = Quaternion.identity;

        // zoom in

        uiJoystick.SetActive(false);

        if (isImmediate == false)
        {
            while (elapsedTime < 1f)
            {
                float ratio = curve.Evaluate(elapsedTime);

                playerTrans.position = Vector3.Lerp(origPos, targetPos, ratio);
                playerTrans.rotation = Quaternion.Lerp(origRot, targetRot, ratio);
                camTrans.localRotation = Quaternion.Lerp(camOrigRot, camTargetRot, ratio);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        playerTrans.position = targetPos;
        playerTrans.rotation = targetRot;
        camTrans.localRotation = camTargetRot;

        RectTransform rt = uiGallery.rawImage.rectTransform;
        float screenWidth = 1280f;
        float screenHeight = 720f;
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

        uiGallery.rawImage.texture = mainTexture;
        uiGallery.title.text = Title;
        uiGallery.desc.text = Desc;
        uiGallery.gameObject.SetActive(true);

        while (uiGallery.gameObject.activeSelf == true)
            yield return null;

        // zoom out

        Vector3 standingPos;
        Quaternion standingRot;
        GetPositionAndRotation(false, out standingPos, out standingRot);

        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float ratio = curve.Evaluate(elapsedTime);

            playerTrans.position = Vector3.Lerp(targetPos, standingPos, ratio);
            playerTrans.rotation = Quaternion.Lerp(targetRot, standingRot, ratio);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Main.instance.Lock(false);
        uiJoystick.SetActive(true);

        yield return null;

        playerTrans.position = standingPos;
        playerTrans.rotation = standingRot;

        onZooming = null;
    }

#if UNITY_EDITOR

    static int frameCount = 0;
    static Dictionary<int, int> orders = new Dictionary<int, int>();

    private void OnDrawGizmos()
    {
        if (frameCount != Time.frameCount)
        {
            orders.Clear();

            PictureFrame[] frames = GameObject.FindObjectsOfType<PictureFrame>();
            for (int i = 0; i < frames.Length; ++i)
            {
                if (orders.ContainsKey(frames[i].Order) == true)
                    orders[frames[i].Order]++;
                else
                    orders.Add(frames[i].Order, 1);
            }

            frameCount = Time.frameCount;
        }

        if (orders.ContainsKey(Order) == true)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = (orders[Order] <= 1) ? Color.green : Color.red;
            Handles.Label(transform.position, $"{Title} : {Order}", style);
        }
        else
        {
            Handles.Label(transform.position, $"{Title} : {Order}");
        }
    }
#endif
}
