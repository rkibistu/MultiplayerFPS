using System.Runtime.CompilerServices;
using UnityEngine;

public static class VectorExtensions
{
    // Start is called before the first frame update
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this Vector2 vector) {
        return vector.x == 0.0f && vector.y == 0.0f;
    }
}
