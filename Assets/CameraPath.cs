using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class WayPoint
{
    public Vector3 Position;
    public Vector3 Direction;
}

public class CameraPath : MonoBehaviour
{
    public WayPoint[] WayPoints;
    public float Smoothness;
    public Camera Camera;

    private WayPoint[] _controlPoints;
    private float[] _pathLengths;

    private void Start()
    {
        GenerateControlPoints();
        GeneratePathLengths();
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        while (Camera == null)
        {
            yield return null;
        }

        for (int i = 0; i < WayPoints.Length - 1; i++)
        {
            float t = 0.0f;

            while (t < 1.0f)
            {
                t += Time.deltaTime * 0.2f / _pathLengths[i];
                t = Mathf.Min(t, 1.0f);

                Vector3 pos = CalculateBezier(WayPoints[i].Position,
                                              _controlPoints[i * 2].Position,
                                              _controlPoints[i * 2 + 1].Position,
                                              WayPoints[i + 1].Position, t);

                Camera.transform.position = pos;

                Vector3 dir = CalculateBezier(WayPoints[i].Direction,
                                              _controlPoints[i * 2].Direction,
                                              _controlPoints[i * 2 + 1].Direction,
                                              WayPoints[i + 1].Direction, t);

                if (dir.magnitude > 0.0f)
                {
                    Camera.transform.rotation = Quaternion.LookRotation(dir);
                }

                yield return null;
            }
        }
    }

    private void GenerateControlPoints()
    {
        if (WayPoints == null || WayPoints.Length == 0)
        {
            return;
        }

        int count = (WayPoints.Length * 2) - 2;
        _controlPoints = new WayPoint[count];

        int iControl = 0;
        for (int iWay = 0; iWay < WayPoints.Length; iWay++)
        {
            if (iWay == 0)
            {
                _controlPoints[iControl] = new WayPoint();

                Vector3 dir = WayPoints[iWay + 1].Position - WayPoints[iWay].Position;
                _controlPoints[iControl].Position = WayPoints[iWay].Position + (dir * Smoothness);
                _controlPoints[iControl].Direction = WayPoints[iWay].Direction;
                iControl++;
            }
            else if (iWay == WayPoints.Length - 1)
            {
                _controlPoints[iControl] = new WayPoint();

                Vector3 dir = WayPoints[iWay].Position - WayPoints[iWay - 1].Position;
                _controlPoints[iControl].Position = WayPoints[iWay - 1].Position + (dir * Smoothness);
                _controlPoints[iControl].Direction = WayPoints[iWay].Direction;
                iControl++;
            }
            else
            {
                Vector3 posCtrl1, posCtrl2;
                Vector3 dirCtrl1, dirCtrl2;

                GetControlVector(WayPoints[iWay - 1].Position, WayPoints[iWay].Position, WayPoints[iWay + 1].Position, out posCtrl1, out posCtrl2);
                GetControlVector(WayPoints[iWay - 1].Direction, WayPoints[iWay].Direction, WayPoints[iWay + 1].Direction, out dirCtrl1, out dirCtrl2);

                _controlPoints[iControl] = new WayPoint();
                _controlPoints[iControl].Position = posCtrl1;
                _controlPoints[iControl].Direction = dirCtrl1;
                iControl++;

                _controlPoints[iControl] = new WayPoint();
                _controlPoints[iControl].Position = posCtrl2;
                _controlPoints[iControl].Direction = dirCtrl2;
                iControl++;
            }
        }
    }

    private void GetControlVector(Vector3 p1, Vector3 p2, Vector3 p3, out Vector3 ctrl1, out Vector3 ctrl2)
    {
        Vector3 parallel = p3 - p1;
        parallel.Normalize();

        float prevControlLength = (p2 - p1).magnitude * -Smoothness;
        float nextControlLength = (p2 - p3).magnitude * Smoothness;

        ctrl1 = p2 + (parallel * prevControlLength);
        ctrl2 = p2 + (parallel * nextControlLength);
    }

    private void GeneratePathLengths()
    {
        _pathLengths = new float[WayPoints.Length - 1];

        for (int i = 0; i < _pathLengths.Length; i++)
        {
            _pathLengths[i] = 0.0f;

            for (int j = 0; j < 100; j++)
            {
                Vector3 start = CalculateBezier(WayPoints[i].Position,
                                                _controlPoints[i * 2].Position,
                                                _controlPoints[i * 2 + 1].Position,
                                                WayPoints[i + 1].Position, j / 100.0f);

                Vector3 end = CalculateBezier(WayPoints[i].Position,
                                              _controlPoints[i * 2].Position,
                                              _controlPoints[i * 2 + 1].Position,
                                              WayPoints[i + 1].Position, (j + 1) / 100.0f);

                _pathLengths[i] += (end - start).magnitude;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (WayPoints == null)
        {
            return;
        }

        GenerateControlPoints();
        for (int i = 0; i < WayPoints.Length; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(WayPoints[i].Position, new Vector3(0.2f, 0.2f, 0.2f));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(WayPoints[i].Position, WayPoints[i].Position + WayPoints[i].Direction);


            if (i < WayPoints.Length - 1)
            {
                Gizmos.color = Color.white;

                for (int j = 0; j < 10; j++)
                {
                    Vector3 start = CalculateBezier(WayPoints[i].Position,
                                                    _controlPoints[i * 2].Position,
                                                    _controlPoints[i * 2 + 1].Position,
                                                    WayPoints[i + 1].Position, j / 10.0f);

                    Vector3 end = CalculateBezier(WayPoints[i].Position,
                                                  _controlPoints[i * 2].Position,
                                                  _controlPoints[i * 2 + 1].Position,
                                                  WayPoints[i + 1].Position, (j + 1) / 10.0f);

                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }

    private Vector3 CalculateBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float distance)
    {
        float invDistance = 1.0f - distance;
        Vector3 result = (p1 * (invDistance * invDistance * invDistance)) +
                         (p2 * (3.0f * invDistance * invDistance * distance)) +
                         (p3 * (3.0f * invDistance * distance * distance)) +
                         (p4 * (distance * distance * distance));

        return result;
    }
}

