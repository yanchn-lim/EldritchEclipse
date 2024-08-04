using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Static class that contains easing functions for animating or whatever
/// </summary>
public static class EasingFunctions
{
    #region Sine Easing Function
    public static float EaseInSine(float x)
    {
        return 1 - Mathf.Cos((x * Mathf.PI) / 2);
    }

    public static float EaseOutSine(float x)
    {
        return Mathf.Sin((x * Mathf.PI) / 2);
    }

    public static float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
    }
    #endregion

    #region Quadratic Easing Function
    public static float EaseInQuad(float x)
    {
        return x * x;
    }

    public static float EaseOutQuad(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }

    public static float EaseInOutQuad(float x)
    {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }
    #endregion

    #region Cubic Easing Function
    public static float EaseInCubic(float x)
    {
        return x * x * x;
    }

    public static float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    public static float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
    #endregion

    #region Quartic Easing Function
    public static float EaseInQuart(float x)
    {
        return x * x * x * x;
    }

    public static float EaseOutQuart(float x)
    {
        return 1 - Mathf.Pow(1 - x, 4);
    }

    public static float EaseInOutQuart(float x)
    {
        return x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;
    }
    #endregion

    #region Quintic Easing Function
    public static float EaseInQuint(float x)
    {
        return x * x * x * x * x;
    }

    public static float EaseOutQuint(float x)
    {
        return 1 - Mathf.Pow(1 - x, 5);
    }

    public static float EaseInOutQuint(float x)
    {
        return x < 0.5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
    }
    #endregion

    #region Exponential Easing Function
    public static float EaseInExpo(float x)
    {
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }
    public static float EaseOutExpo(float x)
    {
        return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }

    public static float EaseInOutExpo(float x)
    {
        return x == 0
                ? 0
                : x == 1
                ? 1
                : x < 0.5 ? Mathf.Pow(2, 20 * x - 10) / 2
                : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
    }

    #endregion

    #region Circular Easing Function
    public static float EaseInCirc(float x) 
    {
        return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
    }

    public static float EaseOutCirc(float x)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
    }

    public static float EaseInOutCirc(float x)
    {
        return x < 0.5
                ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
                : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;
    }
    #endregion

    #region Back Easing Function
    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return c3 * x * x * x - c1 * x * x;
    }
    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    public static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
          : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
    #endregion

    #region Elastic Easing Function
    public static float EaseInElastic(float x)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10f - 10.75f) * c4);
    }

    public static float EaseOutElastic(float x)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1;
    }

    public static float EaseInOutElastic(float x)
    {
        float c5 = (2 * Mathf.PI) / 4.5f;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : x < 0.5
          ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
          : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
    }
    #endregion

    #region Bounce Easing Function
    public static float EaseInBounce(float x)
    {
        return 1 - EaseOutBounce(1 - x);
    }

    public static float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2 / d1)
        {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }
        else
        {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }

    public static float EaseInOutBounce(float x)
    {
        return x < 0.5
                ? (1 - EaseOutBounce(1 - 2 * x)) / 2
                : (1 + EaseOutBounce(2 * x - 1)) / 2;
    }
    #endregion

    public static float Ease(EasingFunction EasingFunc,float x)
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

    public enum EasingFunction
    {
        NO_EASE,
        EASE_IN_SINE,
        EASE_OUT_SINE,
        EASE_INOUT_SINE,
        EASE_IN_QUAD,
        EASE_OUT_QUAD,
        EASE_INOUT_QUAD,
        EASE_IN_CUBIC,
        EASE_OUT_CUBIC,
        EASE_INOUT_CUBIC,
        EASE_IN_QUARTIC,
        EASE_OUT_QUARTIC,
        EASE_INOUT_QUARTIC,
        EASE_IN_QUINTIC,
        EASE_OUT_QUINTIC,
        EASE_INOUT_QUINTIC,
        EASE_IN_EXPO,
        EASE_OUT_EXPO,
        EASE_INOUT_EXPO,
        EASE_IN_CIRC,
        EASE_OUT_CIRC,
        EASE_INOUT_CIRC,
        EASE_IN_BACK,
        EASE_OUT_BACK,
        EASE_INOUT_BACK,
        EASE_IN_ELASTIC,
        EASE_OUT_ELASTIC,
        EASE_INOUT_ELASTIC,
        EASE_IN_BOUNCE,
        EASE_OUT_BOUNCE,
        EASE_INOUT_BOUNCE,
    }
}

