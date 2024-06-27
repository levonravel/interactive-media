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
}