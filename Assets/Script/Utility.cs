using UnityEngine;

public class Utility : MonoBehaviour
{
    public static float GetRotationFromTwoVectors(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}