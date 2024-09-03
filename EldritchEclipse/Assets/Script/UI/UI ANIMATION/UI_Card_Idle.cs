using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Card_Idle : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    [Header("Idle + Hover animation")]
    [SerializeField]
    float autoTiltAmt;
    [SerializeField]
    float manualTiltAmt,tiltSpeed,scale,scaleSpeed;
        
    float tick = GameVariables.TimeTick;
 
    Coroutine ani_Cor;
    bool mouseHover;

    [SerializeField]
    Transform card;
    float timeInHover = 0;

    [Header("Spinning animation")]
    [SerializeField]
    int rotSpeed;
    bool reachedSlot;
    [SerializeField]
    GameObject cardBack,shadow;

    [SerializeField]
    Camera cam;

    [SerializeField]
    RectMask2D mask;

    [Header("Calls")]
    public UnityEvent Events;

    UnityAction t;
    public void Initialize()
    {
        t = new(hehe);
        Events.AddListener(t);
    }
    void hehe()
    {
        Debug.Log("ttt");
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnAnimation());
    }

    private void OnDisable()
    {
        StopCoroutine(ani_Cor);
    }

    IEnumerator SpawnAnimation() 
    {
        card.localPosition = new(-2000, 0, 0);
        float time = 0;
        StartCoroutine(SpinAnimation());
        yield return new WaitForSeconds(1f);
        while (time < 1)
        {
            card.localPosition = Vector3.Lerp(card.localPosition,Vector3.zero, time);
            time += tick;
            yield return new WaitForSeconds(tick);
        }
        reachedSlot = true;
        ani_Cor = StartCoroutine(Animation());
    }

    IEnumerator SpinAnimation()
    {
        int rotationCounter = 0;

        while (true)
        {
            card.transform.Rotate(new(0,rotSpeed,0));
            rotationCounter += rotSpeed;

            if(rotationCounter % 180 == 0)
            {
                cardBack.SetActive(!cardBack.activeSelf);
            }

            if (reachedSlot)
            {
                cardBack.SetActive(false);
                break;
            }

            yield return new WaitForSeconds(tick);
        }
    }

    IEnumerator Animation()
    {
        float x = 0;
        float y = 0;

        float time = 0;
        while (true)
        {

            Vector3 mousePos = cam.ScreenToWorldPoint(new(Input.mousePosition.x,Input.mousePosition.y,cam.nearClipPlane));
            Vector2 offset = mousePos - transform.position;
            //Debug.DrawRay(transform.position, offset,Color.red);
            float tiltX = mouseHover? offset.y * -1 * manualTiltAmt : 0;
            float tiltY = mouseHover? offset.x * manualTiltAmt : 0;

            x = Mathf.Sin(time);
            y = Mathf.Cos(time);

            float lerpX = Mathf.LerpAngle(card.eulerAngles.x, tiltX + (x * autoTiltAmt), tiltSpeed * tick);
            float lerpY = Mathf.LerpAngle(card.eulerAngles.y, tiltY + (y * autoTiltAmt), tiltSpeed * tick);
            //float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * tick);

            card.eulerAngles = new(lerpX, lerpY, 0);
            timeInHover = mouseHover ? Mathf.Clamp01(timeInHover + tick * scaleSpeed) : Mathf.Clamp01(timeInHover - tick * scaleSpeed);
            float lerpScale = Mathf.Lerp(1, scale, timeInHover);
            card.localScale = new(lerpScale, lerpScale, lerpScale);

            time += tick;
            yield return new WaitForSeconds(tick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseHover = true;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseHover = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0;
        float timeL = 0.3f;
        float endVal = 400;
        Vector4 z = mask.padding;
        float x = endVal - z.x;
        float inc = x / (timeL / tick);
        while (time < timeL)
        {
            Vector4 v = mask.padding;
            Vector4 def = new(v.x,v.y,v.z,v.w);
            def.y += inc;
            mask.padding = def;
            time += tick;
            yield return new WaitForSeconds(tick);
        }
    }
}
