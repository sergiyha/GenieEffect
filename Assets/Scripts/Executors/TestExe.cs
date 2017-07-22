
using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Containers;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Executors
{
    public class TestExe : MonoBehaviour
    {

        // Use this for initialization

        private MeshFilter _mf;
        private Vector3[] _originVertecies;
        public int width;
        public int height;

        public Transform DestLeft;
        public Transform DestRight;


        private Corners _originCorners;
        private Corners _futereCorners;
        private DestinationPoints _desPoints;

        public float AnimationDuration;

        private void InitDestenationPonts()
        {
            _desPoints = new DestinationPoints(DestLeft.position, DestRight.position);
        }

        private void InitCorners()
        {
            var vert = _mf.mesh.vertices;

            _originCorners = new Corners
            (
                vert.First(),
                vert[width - 1],
                vert[width * (height - 1)],
                vert.Last()
            );
        }

        private void InitFutureCorners()
        {
            var vert = _mf.mesh.vertices;
            _futereCorners = new Corners
            (
                vert.First(),
                vert[width - 1],
                _desPoints.Left,
                _desPoints.Right
            );
        }

        private Tuple<Vector3[], Vector3[]> _armatureCurves;

        private void CreateArmatureCurves()
        {
            var p0_l = _futereCorners.TopLeftPoint;
            var p1_l = new Vector3(_futereCorners.TopLeftPoint.x, _futereCorners.BotLeftPoint.y);
            var p2_l = new Vector3(_futereCorners.BotLeftPoint.x, _futereCorners.TopLeftPoint.y);
            var p3_l = _futereCorners.BotLeftPoint;

            var leftCurve = MyMath.GetBezieCurve(height, p0_l, p1_l, p2_l, p3_l);

            var p0_r = _futereCorners.TopRightPoint;
            var p1_r = new Vector3(_futereCorners.TopRightPoint.x, _futereCorners.BotRightPoint.y);
            var p2_r = new Vector3(_futereCorners.BotRightPoint.x, _futereCorners.TopRightPoint.y);
            var p3_r = _futereCorners.BotRightPoint;

            var rightCurve = MyMath.GetBezieCurve(height, p0_r, p1_r, p2_r, p3_r);

            _armatureCurves = new Tuple<Vector3[], Vector3[]>(leftCurve, rightCurve);
        }

        private Vector3[] _futurePoints;

        private void FillBetweenCurves()
        {
            var leftCurv = _armatureCurves.Item1;
            var rightCurv = _armatureCurves.Item2;

            var rowIndex = 0;
            _futurePoints = new Vector3[width * height];

            for (int i = 0; i < height; i++)
            {
                rowIndex = i * width;

                var distanceBtwLeftAndRight = Vector3.Distance(leftCurv[i], rightCurv[i]);
                float stepLength = distanceBtwLeftAndRight / width;

                for (int j = 0; j < width; j++)
                {
                    var step = j * stepLength;
                    _futurePoints[rowIndex + j] = new Vector3(leftCurv[i].x + step, leftCurv[i].y, leftCurv[i].z);
                }
            }
        }

        private float[] _distences;

        private void CalculateDiatanceBetveenVertecies()
        {
            var arrCount = width * height;
            _distences = new float[arrCount];
            var originalVrtcs = _mf.mesh.vertices;
            for (int i = 0; i < arrCount; i++)
            {
                _distences[i] = Vector3.Distance(_futurePoints[i], originalVrtcs[i]);
            }
        }

        IEnumerator MoveVertecies(float time)
        {
            var originalVertecies = _mf.mesh.vertices;
            var newVertArr = new Vector3[width * height];

            var cacheTime = time;
            while (cacheTime >= 0)
            {
                yield return null;
                cacheTime -= Time.deltaTime;

                for (int i = 0; i < width * height; i++)
                {
                    newVertArr[i] = Vector3.Lerp(originalVertecies[i], _futurePoints[i], 1 - cacheTime / time);
                }
                ApplyVertex(newVertArr);
            }
        }

        IEnumerator MoveVerteciesInverse(float time)
        {
            var originalVertecies = _mf.mesh.vertices;
            var newVertArr = new Vector3[width * height];

            var cacheTime = time;
            while (cacheTime >= 0)
            {
                yield return null;
                cacheTime -= Time.deltaTime;

                for (int i = 0; i < width * height; i++)
                {
                    newVertArr[i] = Vector3.Lerp(originalVertecies[i], _originVertecies[i], 1 - cacheTime / time);
                }
                ApplyVertex(newVertArr);
            }
        }

        IEnumerator StartAnimation(float time)
        {
            while (true)
            {
                StartCoroutine(MoveVertecies(time));
                yield return new WaitForSeconds(time);

                StartCoroutine(MoveVerteciesInverse(time));
                yield return new WaitForSeconds(time);
            }
        }

        private void ApplyVertex(Vector3[] vertecies)
        {
            _mf.mesh.vertices = vertecies;
            _mf.mesh.RecalculateBounds();
        }

        void Start()
        {
            _mf = this.gameObject.GetComponent<MeshFilter>();
            _mf.mesh = MeshGenerator.GenerateMeshSquare(width, height).CreateMesh();
            _originVertecies = _mf.mesh.vertices;
            InitCorners();
            InitDestenationPonts();
            InitFutureCorners();
            CreateArmatureCurves();
            FillBetweenCurves();
            StartCoroutine(StartAnimation(AnimationDuration));
        }


    }
}

