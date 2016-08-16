using UnityEngine;
using System.Collections;

public static class TileUtility {

    public static Mesh CreateMesh(Vector2[] poly, float zStart, float zEnd) {
        // convert polygon to triangles
        Triangulator triangulator = new Triangulator(poly);
        int[] tris = triangulator.Triangulate();
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[poly.Length * 2];

        for (int i = 0; i < poly.Length; i++) {
            vertices[i].x = poly[i].x;
            vertices[i].y = poly[i].y;
            vertices[i].z = zEnd; // front vertex
            vertices[i + poly.Length].x = poly[i].x;
            vertices[i + poly.Length].y = poly[i].y;
            vertices[i + poly.Length].z = zStart;  // back vertex    
        }
        int[] triangles = new int[tris.Length * 2 + poly.Length * 6];
        int count_tris = 0;
        for (int i = 0; i < tris.Length; i += 3) {
            triangles[i] = tris[i];
            triangles[i + 1] = tris[i + 1];
            triangles[i + 2] = tris[i + 2];
        } // front vertices
        count_tris += tris.Length;
        for (int i = 0; i < tris.Length; i += 3) {
            triangles[count_tris + i] = tris[i + 2] + poly.Length;
            triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
            triangles[count_tris + i + 2] = tris[i] + poly.Length;
        } // back vertices
        count_tris += tris.Length;
        for (int i = 0; i < poly.Length; i++) {
            // triangles around the perimeter of the object
            int n = (i + 1) % poly.Length;
            triangles[count_tris] = i;
            triangles[count_tris + 1] = i + poly.Length;
            triangles[count_tris + 2] = n;
            triangles[count_tris + 3] = n;
            triangles[count_tris + 4] = n + poly.Length;
            triangles[count_tris + 5] = i + poly.Length;
            count_tris += 6;
        }
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.Optimize();
        return m;
    }
}
