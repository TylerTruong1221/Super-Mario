using UnityEngine;

// Extension methods must be defined in a static class
public static class Extesnsions
{
    // Casting for collision on the default layer (Plumber is on the Player layer)
    private static LayerMask layermask = LayerMask.GetMask("Default");
    public static bool Raycast(this Rigidbody2D rb, Vector2 direction)
    {
        // If physics engine is controlling the object
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            return false;
        }

        float radius = 0.25f;
        float distance = 0.375f;

        // Circle cast because Plubmber's collider is circular.
        RaycastHit2D hit = Physics2D.CircleCast(rb.position, radius, direction.normalized, distance, layermask);
        return hit.collider != null && hit.rigidbody != rb;
    }

    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection)
    {
        // Direction pointing from the other object to Plumber
        Vector2 direction = other.position - transform.position;

        // Returns a float: 1 - parallel same-direction vectors, 0 - perpendicular vectors, -1 - opposite vectors
        return Vector2.Dot(direction.normalized, testDirection) > 0.2f;
    }
}
