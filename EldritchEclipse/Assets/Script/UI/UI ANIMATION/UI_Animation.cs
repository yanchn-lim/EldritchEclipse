using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EasingFunctions;

[System.Serializable]
public abstract class UI_Animation
{
    public float Time;
    public float Speed;
    public EasingFunction EasingFunc;

    public abstract void Update();
    
    protected float Ease(float x)
    {
        switch (EasingFunc)
        {
            case EasingFunction.EASE_IN_SINE:
                return EaseInSine(x);
            case EasingFunction.EASE_OUT_SINE:
                return EaseOutSine(x);
            case EasingFunction.EASE_INOUT_SINE:
                return EaseInOutSine(x);

            case EasingFunction.EASE_IN_QUAD:
                return EaseInQuad(x);      
            case EasingFunction.EASE_OUT_QUAD:
                return EaseOutQuad(x);
            case EasingFunction.EASE_INOUT_QUAD:
                return EaseInOutQuad(x);

            case EasingFunction.EASE_IN_CUBIC:
                return EaseInCubic(x);
            case EasingFunction.EASE_OUT_CUBIC:
                return EaseOutCubic(x);
            case EasingFunction.EASE_INOUT_CUBIC:
                return EaseInOutCubic(x);

            case EasingFunction.EASE_IN_QUARTIC:
                return EaseInQuart(x);
            case EasingFunction.EASE_OUT_QUARTIC:
                return EaseOutQuart(x);
            case EasingFunction.EASE_INOUT_QUARTIC:
                return EaseInOutQuart(x);

            case EasingFunction.EASE_IN_QUINTIC:
                return EaseInQuint(x);
            case EasingFunction.EASE_OUT_QUINTIC:
                return EaseOutQuint(x);
            case EasingFunction.EASE_INOUT_QUINTIC:
                return EaseInOutQuint(x);

            case EasingFunction.EASE_IN_EXPO:
                return EaseInExpo(x);
            case EasingFunction.EASE_OUT_EXPO:
                return EaseOutExpo(x);
            case EasingFunction.EASE_INOUT_EXPO:
                return EaseInOutExpo(x);

            case EasingFunction.EASE_IN_CIRC:
                return EaseInCirc(x);
            case EasingFunction.EASE_OUT_CIRC:
                return EaseOutCirc(x);
            case EasingFunction.EASE_INOUT_CIRC:
                return EaseInOutCirc(x);

            case EasingFunction.EASE_IN_BACK:
                return EaseInBack(x);
            case EasingFunction.EASE_OUT_BACK:
                return EaseOutBack(x);
            case EasingFunction.EASE_INOUT_BACK:
                return EaseInOutBack(x);

            case EasingFunction.EASE_IN_ELASTIC:
                return EaseInElastic(x);
            case EasingFunction.EASE_OUT_ELASTIC:
                return EaseOutElastic(x);
            case EasingFunction.EASE_INOUT_ELASTIC:
                return EaseInOutElastic(x);

            case EasingFunction.EASE_IN_BOUNCE:
                return EaseInBounce(x);
            case EasingFunction.EASE_OUT_BOUNCE:
                return EaseOutBounce(x);
            case EasingFunction.EASE_INOUT_BOUNCE:
                return EaseInOutBounce(x);

            case EasingFunction.NO_EASE:
                return x;
            default:
                Debug.Log("Invalid Easing Function");
                return -1;
        }
    }
}
