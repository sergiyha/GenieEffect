using UnityEngine;

namespace Assets.Scripts.Tools
{
    public static class MyMath
    {
        public static Vector3[] GetBezieCurve(int segmentCount, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var curve = new Vector3[segmentCount];

            for (int i = 0; i < curve.Length; i++)
            {
                float t = i / (float)(segmentCount - 1);
                curve[i] = CalculateBezierPoint(t, p0, p1, p2, p3);
            }
            return curve;
        }

        public static Vector3[] GetSubBezieCurve(int endPoint, int segmentCount, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var curve = new Vector3[endPoint];

            for (int i = 0; i < endPoint; i++)
            {
                float t = i / (float)(segmentCount - 1);
                curve[i] = CalculateBezierPoint(t, p0, p1, p2, p3);
            }
            return curve;
        }

        public static Vector3[] GetInverseSubBezieCurve(int neededcount, int segmentCount, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var curve = new Vector3[neededcount];

            for (int i = 0; i < neededcount; i++)
            {
                int p = segmentCount - neededcount + i;
                float t = p / (float)(segmentCount - 1);
                curve[i] = CalculateBezierPoint(t, p0, p1, p2, p3);
            }
            return curve;
        }


        public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;    //first term
            p += 3 * uu * t * p1;    //second term
            p += 3 * u * tt * p2;    //third term
            p += ttt * p3;           //fourth term

            return p;
        }

    }


}
