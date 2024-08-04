using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UI_Card_Idle : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField]
    float autoTiltAmt,manualTiltAmt,tiltSpeed,scale,scaleSpeed;
        
    float tick = GameVariables.TimeTick;
 
    Coroutine ani_Cor;
    bool mouseHover;

    [SerializeField]
    Transform card;
    float timeInHover = 0;
    public void Initialize()
    {

    }

    private void OnEnable()
    {
        ani_Cor = StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        StopCoroutine(ani_Cor);
    }

    IEnumerator Animation()
    {
        float x = 0;
        float y = 0;

        float time = 0;
        while (true)
        {           
            Vector3 offset = transform.position - Input.mousePosition;
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
}