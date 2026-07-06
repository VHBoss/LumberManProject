using UnityEngine;

public static class Utils
{
    private static Vector3 extends = Vector3.one * 1.5f;
    public static Collider[] results = new Collider[10];

    public static int GetClosestTrees(Vector3 pos, out Collider[] foundTrees)
    {
        int count = Physics.OverlapBoxNonAlloc(pos, extends, results, Quaternion.identity, 1 << 7);
        //Debug.Log(count);
        //DrawDebugBox(pos, extends, Color.green, 4f);
        if (count > 0)
        {
            foundTrees = results;
            return count;
        }

        foundTrees = null;
        return 0;
    }

    private static void DrawDebugBox(Vector3 center, Vector3 halfExtents, Color color, float duration = 0f)
    {
        // Вычисляем углы куба
        Vector3 p1 = center + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        Vector3 p2 = center + new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        Vector3 p3 = center + new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        Vector3 p4 = center + new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        Vector3 p5 = center + new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        Vector3 p6 = center + new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        Vector3 p7 = center + new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        Vector3 p8 = center + new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);

        // Нижняя грань
        Debug.DrawLine(p1, p2, color, duration);
        Debug.DrawLine(p2, p3, color, duration);
        Debug.DrawLine(p3, p4, color, duration);
        Debug.DrawLine(p4, p1, color, duration);

        // Верхняя грань
        Debug.DrawLine(p5, p6, color, duration);
        Debug.DrawLine(p6, p7, color, duration);
        Debug.DrawLine(p7, p8, color, duration);
        Debug.DrawLine(p8, p5, color, duration);

        // Вертикальные ребра
        Debug.DrawLine(p1, p5, color, duration);
        Debug.DrawLine(p2, p6, color, duration);
        Debug.DrawLine(p3, p7, color, duration);
        Debug.DrawLine(p4, p8, color, duration);
    }
}
