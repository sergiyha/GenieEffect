using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshJeen : MonoBehaviour
{
    private MeshFilter _mf;
    public int Row_n;

    private bool[] isVertexGreenArr;

    private void Start()
    {
        _mf = this.gameObject.GetComponent<MeshFilter>();
        CacheVertecies();
        isVertexGreenArr = new bool[_mf.mesh.vertices.Length];
        StartCoroutine(startChangeColor());
        //ManipulateMesh();
        // ChangeColor();
    }


    private Vector3[] cachedVertecies;

    private void CacheVertecies()
    {
        cachedVertecies = _mf.mesh.vertices;
    }

    private void ManipulateMesh()
    {
        var vertecies = _mf.mesh.vertices;
        int choosenVrtx = Random.Range(0, vertecies.Length);
        vertecies[choosenVrtx] = new Vector3(100, 100, 100);

        _mf.mesh.vertices = vertecies;
        _mf.mesh.RecalculateBounds();
    }

    private List<Vector3> _neededVertecies;



    IEnumerator startChangeColor()
    {
        while (true)
        {
            ChangeColor();
            StartCoroutine(MoveVertexUp(0.5f, 10));
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(MoveVertexDown(0.5f, 10));
            yield return new WaitForSeconds(0.5f);
            //ApplyVertex(MoveVertex());
        }
    }


    public static Color ConvertColor(int r, int g, int b, int a)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }


    private bool CheckColor(Color color)
    {
        Color32 col32 = new Color(color.r, color.g, color.b, color.a);

        if ((col32.r >= 0 && col32.r <= 32) &&
            (col32.g >= 188 && col32.g <= 255) &&
            (col32.b >= 0 && col32.b <= 66)
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void ChangeColor()
    {
        _neededVertecies = new List<Vector3>();

        var vertecies = _mf.mesh.vertices;

        var cols = new Color[_mf.mesh.vertices.Length];
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f));
            if (CheckColor(cols[i]))
            {
                isVertexGreenArr[i] = true;
            }
            else
            {
                isVertexGreenArr[i] = false;
            }
        }

        _mf.mesh.colors = cols;
        //  Debug.Log(_mf.mesh.colors.Length);
    }


    private Vector3[] MoveVertex()
    {
        var newVertArr = _mf.mesh.vertices;

        for (int i = 0; i < isVertexGreenArr.Length; i++)
        {
            if (isVertexGreenArr[i])
            {
                newVertArr[i] = new Vector3(cachedVertecies[i].x, 2, cachedVertecies[i].z);
            }
            else
            {
                newVertArr[i] = cachedVertecies[i];
            }
        }
        return newVertArr;
    }


    private IEnumerator MoveVertexUp(float time, int distance)
    {
        var cachedTime = time;
        while (time >= 0)
        {
            yield return null;
            time -= Time.deltaTime;
            var newVertArr = _mf.mesh.vertices;

            for (int i = 0; i < isVertexGreenArr.Length; i++)
            {
                if (isVertexGreenArr[i])
                {
                    newVertArr[i] = new Vector3(cachedVertecies[i].x, distance * (1 - time / cachedTime),
                        cachedVertecies[i].z);
                }
                else
                {
                    newVertArr[i] = cachedVertecies[i];
                }

                ApplyVertex(newVertArr);
            }
        }

    }


    private IEnumerator MoveVertexDown(float time, int distance)
    {
        var cachedTime = time;
        while (time >= 0)
        {
            yield return null;
            time -= Time.deltaTime;
            var newVertArr = _mf.mesh.vertices;

            for (int i = 0; i < isVertexGreenArr.Length; i++)
            {
                if (isVertexGreenArr[i])
                {
                    newVertArr[i] = new Vector3(cachedVertecies[i].x, distance * time / cachedTime,
                        cachedVertecies[i].z);
                }
                else
                {
                    newVertArr[i] = cachedVertecies[i];
                }

                ApplyVertex(newVertArr);
            }
        }

    }


    private void ApplyVertex(Vector3[] vertecies)
    {
        _mf.mesh.vertices = vertecies;
        _mf.mesh.RecalculateBounds();
    }

    private void OnDrawGizmos()
    {
        if (_neededVertecies == null) return;
        foreach (var vertex in _neededVertecies)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertex, 0.3f);
        }
    }
}