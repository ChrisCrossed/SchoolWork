using UnityEngine;
using System.Collections;

public class ReferenceFloat
{
    private float value;

    ReferenceFloat(float _value)
    {
        value = _value;
    }

    public static implicit operator ReferenceFloat(float a)
    {
        return new ReferenceFloat(a);
    }

    public static float operator /(ReferenceFloat a, float b)
    {
        return a.value / b;
    }
    public static float operator *(ReferenceFloat a, float b)
    {
        return a.value * b;
    }
    public static float operator -(ReferenceFloat a, float b)
    {
        return a.value - b;
    }
    public static float operator +(ReferenceFloat a, float b)
    {
        return a.value + b;
    }
    public static bool operator >(ReferenceFloat a, float b)
    {
        return a.value > b;
    }
    public static bool operator <(ReferenceFloat a, float b)
    {
        return a.value < b;
    }
    public static bool operator >=(ReferenceFloat a, float b)
    {
        return a.value >= b;
    }
    public static bool operator <=(ReferenceFloat a, float b)
    {
        return a.value <= b;
    }

    public static float operator /(float a, ReferenceFloat b)
    {
        return a / b.value;
    }
    public static float operator *(float a, ReferenceFloat b)
    {
        return a * b.value;
    }
    public static float operator -(float a, ReferenceFloat b)
    {
        return a - b.value;
    }
    public static float operator +(float a, ReferenceFloat b)
    {
        return a + b.value;
    }
    public static bool operator >(float a, ReferenceFloat b)
    {
        return a > b.value;
    }
    public static bool operator <(float a, ReferenceFloat b)
    {
        return a < b.value;
    }
    public static bool operator >=(float a, ReferenceFloat b)
    {
        return a >= b.value;
    }
    public static bool operator <=(float a, ReferenceFloat b)
    {
        return a <= b.value;
    }
}
