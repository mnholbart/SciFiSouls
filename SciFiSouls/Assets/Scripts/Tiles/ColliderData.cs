using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColliderData  {

    public enum ColliderTypes {
        None = 0,
        Box = 1,
        Circle = 2,
        Mesh = 3
    }

    public Data data = new Data();
    public Data moveData = new Data();

    [System.Serializable]
    public class Data {
        public ColliderTypes CurrentColliderType = ColliderTypes.None;

        public bool isValidPolygon() {
            return points.Length > 2;
        }

        public bool isValidBox() {
            if (boxBoundsX == Vector2.zero && boxBoundsY == Vector2.zero)
                return false;

            return true;
        }

        public bool isValidCylinder() {
            if (radius == 0)
                return false;

            return true;
        }

        public int SpriteWidth;
        public int SpriteHeight;

        public Vector2[] points;

        public Vector2 boxBoundsX = Vector2.zero;
        public Vector2 boxBoundsY = Vector2.zero;

        public Vector3 cylinderCenter = Vector3.zero;
        public float radius;

        public Mesh MeshColliderMesh;
    }
}
