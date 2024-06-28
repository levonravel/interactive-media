using System;
using System.Numerics;

public static class Vector2Extensions
{
    public static Vector2 Normalize(this Vector2 vector)
    {
        float length = vector.Length();
        return length > 0 ? vector / length : Vector2.Zero;
    }

    public static float Magnitude(this Vector2 vector)
    {
        return vector.Length();
    }

    public static float Angle(Vector2 from, Vector2 to)
    {
        float dotProduct = from.X * to.X + from.Y * to.Y;
        float magnitudes = from.Magnitude() * to.Magnitude();
        return (float)(Math.Acos(dotProduct / magnitudes) * (180.0 / Math.PI));
    }
}