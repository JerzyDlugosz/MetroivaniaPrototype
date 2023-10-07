using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class MathExtensions
{
    /// <summary>
    /// Gets the angle between transform and the target. 
    /// Angle is described as an angle from (in positive y) 180 to 0 and (in negative y) -180 to 0 (from left to right);
    /// </summary>
    /// <param name="me">This Position</param>
    /// <param name="target">Target Position</param>
    /// <returns></returns>
    public static float GetAngle(Vector2 me, Vector2 target)
    {
        return Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI);
    }


    /// <summary>
    /// Gets the angle magnitude as a Vector2.
    /// </summary>
    /// <param name="angle">Angle form GetAngle method.  (-180 -> 180 angle)</param>
    /// <param name="reversed">Is the resulting magnitude reversed? This will make the magnitude point towards the center of the angle</param>
    /// <returns></returns>
    public static Vector2 GetAngleMagnitude(float angle, bool reversed)
    {
        float magnitude = angle / 90;
        Vector2 vector;

        if (magnitude > -1 && magnitude < 1)
        {
            vector = new Vector2(1 - Mathf.Abs(magnitude), magnitude);
        }
        else if (magnitude < -1)
        {
            vector = new Vector2(1 - Mathf.Abs(magnitude), -2 + Mathf.Abs(magnitude));
        }
        else
        {
            vector = new Vector2(1 - Mathf.Abs(magnitude), 2 - magnitude);
        }

        if (reversed)
        {
            return -vector;
        }
        else
        {
            return vector;
        }
    }

    public static Vector2 GetReducedValue(Vector2 magnitude, float maximumValue, float value)
    {
        float temp = value / maximumValue;
        return magnitude / temp;
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
}
