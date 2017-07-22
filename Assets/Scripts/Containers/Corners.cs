using UnityEngine;

namespace Assets.Scripts.Containers
{
    public class Corners
    {
        public Vector3 TopLeftPoint;
        public Vector3 TopRightPoint;
        public Vector3 BotLeftPoint;
        public Vector3 BotRightPoint;

        public Corners(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            TopLeftPoint = tl;
            TopRightPoint = tr;
            BotLeftPoint = bl;
            BotRightPoint = br;
        }

    }
}
