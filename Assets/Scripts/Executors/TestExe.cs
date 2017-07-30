
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

        public Material Mat;

        private MeshFilter _mf;
        private Vector3[] _originVertecies;
        public int width;
        public int height;

        public Transform DestLeft;
        public Transform DestRight;


        private Corners _originCorners;
        private Corners _futereCorners;

        private BezieControlPoints _r_cont_p; //right control points
        private BezieControlPoints _l_cont_p;//left control points



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

        private Tuple<Vector3[], Vector3[]> _armatureCurves;// main trajectory armature
        private Tuple<Vector3[], Vector3[]> _subArmetureCurve;// traectory if bottom original corners cross by Y with bezie curve
        private Tuple<Vector3[], Vector3[]> _moovingTrajectorySubCurve;



        private void CreateArmatureCurves()
        {

            var leftCurve = MyMath.GetBezieCurve(height,
                _l_cont_p.p0,
                _l_cont_p.p1,
                _l_cont_p.p2,
                _l_cont_p.p3);

            var rightCurve = MyMath.GetBezieCurve(height,
                _r_cont_p.p0,
                _r_cont_p.p1,
                _r_cont_p.p2,
                _r_cont_p.p3);

            _armatureCurves = new Tuple<Vector3[], Vector3[]>(leftCurve, rightCurve);
        }

        private void InitControlPoints()
        {
            _l_cont_p.p0 = _futereCorners.TopLeftPoint;
            _l_cont_p.p1 = new Vector3(_futereCorners.TopLeftPoint.x, _futereCorners.BotLeftPoint.y);
            _l_cont_p.p2 = new Vector3(_futereCorners.BotLeftPoint.x, _futereCorners.TopLeftPoint.y);
            _l_cont_p.p3 = _futereCorners.BotLeftPoint;

            _r_cont_p.p0 = _futereCorners.TopRightPoint;
            _r_cont_p.p1 = new Vector3(_futereCorners.TopRightPoint.x, _futereCorners.BotRightPoint.y);
            _r_cont_p.p2 = new Vector3(_futereCorners.BotRightPoint.x, _futereCorners.TopRightPoint.y);
            _r_cont_p.p3 = _futereCorners.BotRightPoint;
        }

        private void CreateSubArmatureCurves(int segmentsCount)
        {
            var leftCurve = MyMath.GetSubBezieCurve
                (
                    height,
                    segmentsCount,
                    _l_cont_p.p0,
                    _l_cont_p.p1,
                    _l_cont_p.p2,
                    _l_cont_p.p3
                );

            var rightCurve = MyMath.GetSubBezieCurve
                (
                    height,
                    segmentsCount,
                    _r_cont_p.p0,
                    _r_cont_p.p1,
                    _r_cont_p.p2,
                    _r_cont_p.p3
                );

            _subArmetureCurve = new Tuple<Vector3[], Vector3[]>(leftCurve, rightCurve);
        }

        private void CreateInverseSubArmatureCurves(int segmentsCount)
        {
            var leftCurve = MyMath.GetInverseSubBezieCurve
            (
                height,
                segmentsCount,
                _l_cont_p.p0,
                _l_cont_p.p1,
                _l_cont_p.p2,
                _l_cont_p.p3
            );

            var rightCurve = MyMath.GetInverseSubBezieCurve
            (
                height,
                segmentsCount,
                _r_cont_p.p0,
                _r_cont_p.p1,
                _r_cont_p.p2,
                _r_cont_p.p3
            );

            _moovingTrajectorySubCurve = new Tuple<Vector3[], Vector3[]>(leftCurve, rightCurve);
        }

        private int GetSubCurveBeziePointsCount()
        {
            float relativeSubCurveLength = _indexOfCrossPointInBezieCurve / (float)(_armatureCurves.Item1.Length - 1);
            int pointsCount = (int)(height / relativeSubCurveLength);
            return pointsCount;
        }

        private int GetInverseSubCurveBeziePointsCount()
        {
            float relativeSubCurveLength = ((_armatureCurves.Item1.Length - 1) - _indexOfCrossPointInBezieCurve) / (float)(_armatureCurves.Item1.Length - 1);
            int pointsCount = (int)(height / relativeSubCurveLength);
            return pointsCount;
        }

        private Vector3[] _futureSubPoints;

        private Vector3[] _futureInverseSubPoints;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            //Gizmos.color = Color.black;
            foreach (var p in _armatureCurves.Item1)
            {
                Gizmos.DrawSphere(p, 0.5f);
            }

            foreach (var p in _armatureCurves.Item2)
            {
                Gizmos.DrawSphere(p, 0.5f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_originCorners.BotLeftPoint, 1f);


            foreach (var p in _futureSubPoints)
            {
                Gizmos.DrawSphere(p, 0.1f);
            }

            Gizmos.color = Color.green;
            foreach (var p in _moovingTrajectorySubCurve.Item1)
            {
                Gizmos.DrawSphere(p, 0.1f);
            }

            foreach (var p in _moovingTrajectorySubCurve.Item2)
            {
                Gizmos.DrawSphere(p, 0.1f);
            }

            foreach (var p in _futureInverseSubPoints)
            {
                Gizmos.DrawSphere(p, 0.1f);
            }

            Gizmos.DrawSphere(_futureInverseSubPoints[0], 1f);


        }

        private void FillBetweenCurves(Vector3[] leftCurv, Vector3[] rightCurv, out Vector3[] futurePoints)
        {
            var rowIndex = 0;
            futurePoints = new Vector3[width * height];

            for (int i = 0; i < height; i++)
            {
                rowIndex = i * width;

                float distanceBtwLeftAndRight = Vector3.Distance(leftCurv[i], rightCurv[i]);
                float stepLength = distanceBtwLeftAndRight / (width - 1);

                for (int j = 0; j < width; j++)
                {
                    var step = j * stepLength;
                    futurePoints[rowIndex + j] = new Vector3(leftCurv[i].x + step, leftCurv[i].y, leftCurv[i].z);
                }
            }
        }


        private IEnumerator MoveToInversionPoints
            (
            float time,
            AnimationDirection direction,
            Vector3[] destinationPoints,
            Vector3[] exstraDestinationPoints = null,
            bool goIn = false,
            bool goOut = false
            )
        {
            var newVertArr = _mf.mesh.vertices;

            for (int j = 0; j < height; j++)
            {
                var originalVertecies = _mf.mesh.vertices;
                var cacheTime = time;
                while (cacheTime > 0)
                {
                    yield return null;
                    cacheTime -= Time.deltaTime;

                    var fullSize = width * height;

                    for (int i = 0; i < fullSize; i++)
                    {
                        int futureSubPosIndex = 0;
                        if (direction == AnimationDirection.Backward)
                        {
                            futureSubPosIndex = i - width * (j + 1);

                            if (futureSubPosIndex < 0)
                            {
                                if (goIn) continue;
                                newVertArr[i] = Vector3.Lerp(originalVertecies[i], exstraDestinationPoints[fullSize + futureSubPosIndex], 1 - cacheTime / time);
                                ApplyVertex(newVertArr);
                                continue;
                            }
                        }
                        else if (direction == AnimationDirection.Forward)
                        {
                            futureSubPosIndex = i + width * (j + 1);

                            if (futureSubPosIndex >= fullSize)
                            {
                                if (goIn) continue;
                                newVertArr[i] = Vector3.Lerp(originalVertecies[i], exstraDestinationPoints[futureSubPosIndex - fullSize], 1 - cacheTime / time);
                                ApplyVertex(newVertArr);
                                continue;
                            }
                        }

                        if (goOut) continue;
                        newVertArr[i] = Vector3.Lerp(originalVertecies[i], destinationPoints[futureSubPosIndex], 1 - cacheTime / time);
                        ApplyVertex(newVertArr);
                    }
                }
            }

        }



        IEnumerator MoveVertecies(float time, Vector3[] _futurePoints)
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

        private int _indexOfCrossPointInBezieCurve;
        private bool CheckCrossYTheCurve(float crossPointY)//Check if bottom left or bottom right original points cross the Y bezie curve and get point 
        {
            var _beziePoints = _armatureCurves.Item1;
            for (int i = 0; i < _beziePoints.Length; i++)//no metter what curve it exactly is 
            {
                if (i + 1 > _beziePoints.Length - 1) break;
                if ((_beziePoints[i].y >= crossPointY && _beziePoints[i + 1].y <= crossPointY) ||
                    (_beziePoints[i].y <= crossPointY && _beziePoints[i + 1].y >= crossPointY))
                {
                    _indexOfCrossPointInBezieCurve = i;
                    return true;
                }
            }
            return false;
        }

        IEnumerator StartAnimation(float time)
        {
            int i = 0;
            while (i == 0)
            {

                StartCoroutine(MoveVertecies(time, _futureSubPoints));
                yield return new WaitForSeconds(time);
                StartCoroutine(MoveToInversionPoints(0.01f, AnimationDirection.Forward, _futureSubPoints, _futureInverseSubPoints));
                yield return new WaitForSeconds(2);

                StartCoroutine(MoveToInversionPoints(0.01f, AnimationDirection.Forward, _futureInverseSubPoints, null, true, false));

                yield return new WaitForSeconds(2);
                StartCoroutine(MoveToInversionPoints(0.01f, AnimationDirection.Backward, null, _futureInverseSubPoints, false, true));

                yield return new WaitForSeconds(2);
                StartCoroutine(MoveToInversionPoints(0.01f, AnimationDirection.Backward, _futureInverseSubPoints, _futureSubPoints));

                yield return new WaitForSeconds(2);
                StartCoroutine(MoveVertecies(time, _originVertecies));
                yield return new WaitForSeconds(time);

            }
        }

        private void ApplyVertex(Vector3[] vertecies)
        {
            _mf.mesh.MarkDynamic();
            _mf.mesh.vertices = vertecies;
            // _mf.mesh.RecalculateBounds();
        }

        void Start()
        {
            _mf = this.gameObject.GetComponent<MeshFilter>();
            _mf.mesh = MeshGenerator.GenerateMeshSquare(width, height).CreateMesh();
            this.gameObject.GetComponent<MeshRenderer>().material = Mat;

            _originVertecies = _mf.mesh.vertices;
            InitCorners();
            InitDestenationPonts();
            InitFutureCorners();
            InitControlPoints();
            CreateArmatureCurves();

            if (CheckCrossYTheCurve(_originCorners.BotLeftPoint.y))
            {
                CreateSubArmatureCurves(GetSubCurveBeziePointsCount());
                FillBetweenCurves(_subArmetureCurve.Item1, _subArmetureCurve.Item2, out _futureSubPoints);
                CreateInverseSubArmatureCurves(GetInverseSubCurveBeziePointsCount());
                FillBetweenCurves(_moovingTrajectorySubCurve.Item1, _moovingTrajectorySubCurve.Item2, out _futureInverseSubPoints);
                StartCoroutine(StartAnimation(AnimationDuration));
            }

        }

    }

}

public enum AnimationDirection
{
    Forward,
    Backward
}

