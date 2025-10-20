using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RoadEdit : MonoBehaviour
{
    [SerializeField]
    private MeshFilter mr;
    [SerializeField]
    private MeshCollider mc;


    [Header("Size")]
    [SerializeField]
    private float stepSize;
    [SerializeField]
    private float width;
    [SerializeField]
    private int UVStepCount;
    private float UVStepSize;

    [Header("Barrier")]
    [SerializeField]
    private float barrierHeight;
    [SerializeField]
    private GameObject barrierPrefab;
    [SerializeField]
    private Transform barrierParent;
    [SerializeField]
    private bool lBarrier = false;
    [SerializeField]
    private bool rBarrier = false;

    [Header("Road Build")]
    [SerializeField]
    private bool editOn = false;

    [SerializeField]
    private List<Transform> knots;
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
    }

    // Update is called once per frame
    void Update()
    {
        if(editOn)
        {
            SetMesh();
            editOn = false;
        }
    }

    private void DrawMesh()
    {

        UVStepSize = 1f / UVStepCount;

        List<Vector3> verts = new();
        List<int> tris = new();
        List<Vector2> UVs = new();

        int offset = 0; Vector3 _p; float UVCounter = 0f;

        Vector3 _p0 = knots[0].position;
        _p0.y = 0f;
        Vector3 right = Vector3.Cross(knots[0].forward, Vector3.up).normalized;
        Vector3 rP = _p0 + right * width;
        Vector3 lP = _p0 - right * width;
        Vector3 _rP = rP;
        Vector3 _lP = lP;
        verts.AddRange(new List<Vector3> { rP, lP });
        UVs.AddRange(new List<Vector2> { new Vector2(UVCounter, 0f), new Vector2(UVCounter, 1f) });
        UVCounter += UVStepSize;
        if (UVCounter > 1f)
            UVCounter = 0f;

        for (int i = 1; i < knots.Count; ++i)
        {
            Debug.Log(i);
            Vector3[] p = new Vector3[]
            {
                knots[i - 1].position,
                knots[i].position
            };
            p[0].y = 0f;
            p[1].y = 0f;
            Vector3[] v = new Vector3[]
            {
                knots[i - 1].forward,
                knots[i].forward
            };
            Vector3 c = CornerOf(p, v);


            for (float t = stepSize; t < 1f; t += stepSize)
            {
                _p = WeightPoint(p[0], p[1], c, t);
                right = Vector3.Cross(_p - _p0, Vector3.up).normalized;
                _p0 = _p;

                rP = _p + right * width;
                lP = _p - right * width;

                verts.AddRange(new List<Vector3> {rP, lP});
                UVs.AddRange(new List<Vector2> { new Vector2(UVCounter, 0f), new Vector2(UVCounter, 1f) });
                tris.AddRange(new List<int> {offset + 2, offset + 3, offset, offset + 3, offset + 1, offset});
                offset += 2;

                if (lBarrier)
                {
                    Transform obj = Instantiate(barrierPrefab, barrierParent).transform;
                    obj.localPosition = (lP + _lP) * .5f + .5f * barrierHeight * Vector3.up;
                    obj.forward = Vector3.Cross(lP - _lP, Vector3.up);
                    obj.localScale = new(1f, 1f, -1f);

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    sr.size = new Vector2((lP - _lP).magnitude, barrierHeight);
                    obj.GetComponent<BoxCollider>().size = (Vector3)sr.size + Vector3.forward * .2f;
                }
                if (rBarrier)
                {
                    Transform obj = Instantiate(barrierPrefab, barrierParent).transform;
                    obj.localPosition = (rP + _rP) * .5f + .5f * barrierHeight * Vector3.up;
                    obj.forward = Vector3.Cross(rP - _rP, Vector3.up);

                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    sr.size = new Vector2((rP - _rP).magnitude, barrierHeight);
                    obj.GetComponent<BoxCollider>().size = (Vector3)sr.size + Vector3.forward * .2f;
                }
                _rP = rP;
                _lP = lP;

                UVCounter += UVStepSize;
                if (UVCounter > 1f)
                    UVCounter = 0f;
            }

            _p = p[1];
            right = Vector3.Cross(v[1], Vector3.up).normalized;
            _p0 = _p;

            rP = _p + right * width;
            lP = _p - right * width;

            verts.AddRange(new List<Vector3> { rP, lP });
            UVs.AddRange(new List<Vector2> { new Vector2(UVCounter, 0f), new Vector2(UVCounter, 1f) });
            tris.AddRange(new List<int> { offset + 2, offset + 3, offset, offset + 3, offset + 1, offset });
            offset += 2;

            if (lBarrier)
            {
                Transform obj = Instantiate(barrierPrefab, barrierParent).transform;
                obj.localPosition = (lP + _lP) * .5f + .5f * barrierHeight * Vector3.up;
                obj.forward = Vector3.Cross(lP - _lP, Vector3.up);
                obj.localScale = new(1f, 1f, -1f);

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.size = new Vector2((lP - _lP).magnitude, barrierHeight);
                obj.GetComponent<BoxCollider>().size = (Vector3)sr.size + Vector3.forward * .2f;
            }
            if (rBarrier)
            {
                Transform obj = Instantiate(barrierPrefab, barrierParent).transform;
                obj.localPosition = (rP + _rP) * .5f + .5f * barrierHeight * Vector3.up;
                obj.forward = Vector3.Cross(rP - _rP, Vector3.up);

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.size = new Vector2((rP - _rP).magnitude, barrierHeight);
                obj.GetComponent<BoxCollider>().size = (Vector3)sr.size + Vector3.forward * .2f;
            }
            _rP = rP;
            _lP = lP;

            UVCounter += UVStepSize;
            if (UVCounter > 1f || UVCounter < 0f)
            {
                UVStepSize *= -1f;
                UVCounter += UVStepSize;
            }

        }

        //_p0 = knots[^1].position;
        //right = Vector3.Cross(knots[^1].forward, Vector3.up).normalized;
        //rP = _p0 + right * width;
        //lP = _p0 - right * width;

        //verts.AddRange(new List<Vector3> { rP, lP });
        //tris.AddRange(new List<int> { offset, offset + 3, offset + 2, offset, offset + 3, offset + 1 });

        mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, UVs);
    }

    private void SetMesh()
    {
        DrawMesh();
        mr.mesh = mesh;
        mc.sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private Vector3 CornerOf(Vector3[] p, Vector3[] v)  
    {
        Vector3 c = new Vector3();
        float m0 = v[0].z / (v[0].x == 0 ? .0001f : v[0].x);
        float m1 = v[1].z / (v[1].x == 0 ? .0001f : v[1].x);
        float _m = m0 - m1 == 0 ? .0001f : m0 - m1;


        c.x = ((m0 * p[0].x) - (m1 * p[1].x) + p[1].z - p[0].z) / _m;
        c.z = (c.x - p[0].x) * m0 + p[0].z;
        return c;
    }

    private Vector3 WeightPoint(Vector3 p0, Vector3 p1, Vector3 c, float t)
    {
        Vector3 d0 = (1f - t) * p0 + t * c;
        Vector3 d1 = (1f - t) * c + t * p1;
        return (1f - t) * d0 + t * d1;
    }
}
