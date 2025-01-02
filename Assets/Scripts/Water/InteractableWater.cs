using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer),
    typeof(EdgeCollider2D))]
[RequireComponent(typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour
{
    [FormerlySerializedAs("_springConstant")]
    [Header("Springs")]
    [SerializeField]
    private float springConstant = 1.4f;

    [SerializeField] private float damping = 1.1f;
    [SerializeField] private float spread = 6.5f;
    [SerializeField, Range(0f, 20f)] private float speedMult = 10f;
    [SerializeField, Range(1, 10)] private int wavePropagationIterations = 8;

    [Header("Force")] public float forceMultiplier = 0.2f;
    [Range(1f, 50f)] public float MaxForce = 5f;

    [Header("Collision")] [SerializeField, Range(1f, 10f)]
    private float playerCollisionRadiusMult = 4.15f;

    [FormerlySerializedAs("numOfVertices")]
    [FormerlySerializedAs("NumOfVertices")]
    [Header("Mash Generation")]
    [Range(2, 500)]
    public int numOfXVertices = 70;

    [SerializeField] public float height = 10f;
    [SerializeField] public float width = 4f;

    public Material waterMaterial;
    private const int NumOfYVertices = 2;

    [Header("Gizmo")] public Color gizmoColor = Color.white;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Vector3[] _vertices;
    private int[] _topVerticesIndex;

    private EdgeCollider2D _edgeCollider;

    private class WaterPoint
    {
        public float Velocity, Pos, TargetHeight;
    }
    private List<WaterPoint> _waterPoints = new List<WaterPoint>();

    private void Start()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        GenerateMash();
        InitWaterPoints();
    }

    private void InitWaterPoints()
    {
        _waterPoints.Clear();
        for (int i = 0; i < _topVerticesIndex.Length; i++)
        {
            _waterPoints.Add(new WaterPoint
            {
                Pos = _vertices[_topVerticesIndex[i]].y,
                TargetHeight = _vertices[_topVerticesIndex[i]].y
            });
        }
        
    }

    private void FixedUpdate()
    {
        // Update all springs positions
        for (int i = 1; i < _waterPoints.Count - 1; i++)
        {
            WaterPoint point = _waterPoints[i];
            float x = point.Pos - point.TargetHeight;
            float acceleration =
                -springConstant * (x - point.Velocity) * damping;
            point.Pos += point.Velocity * speedMult * Time.fixedDeltaTime;
            _vertices[_topVerticesIndex[i]].y = point.Pos;
            point.Velocity += acceleration * Time.fixedDeltaTime * speedMult;
        }

        // wave propagation
        for (int j = 0; j < wavePropagationIterations; j++)
        {
            for (int i = 1; i < _waterPoints.Count - 1; i++)
            {

                float leftDelta = spread *
                                  (_waterPoints[i].Pos -
                                   _waterPoints[i - 1].Pos) * speedMult * Time.fixedDeltaTime;
                _waterPoints[i - 1].Velocity +=
                    leftDelta * Time.fixedDeltaTime;

                float rightDelta = spread *
                                   (_waterPoints[i].Pos -
                                    _waterPoints[i + 1].Pos);
                _waterPoints[i + 1].Velocity +=
                    rightDelta * Time.fixedDeltaTime;
            }
        }
        // Update the mesh
        _mesh.vertices = _vertices;
    }

    public void Splash(Collider2D collision, float force)
    {
        float radius = collision.bounds.extents.x * playerCollisionRadiusMult;
        Vector2 center = collision.transform.position;
        
        for (int i = 0; i < _waterPoints.Count; i++)
        {
            Vector2 vertexWorldPos = transform.TransformPoint(_vertices[_topVerticesIndex[i]]);
            if (IsPointInsideCircle(vertexWorldPos, center, radius))
            {
                _waterPoints[i].Velocity = force;
            }
        }
    }
    
    private bool IsPointInsideCircle(Vector2 point, Vector2 circleCenter, float radius)
    {
        float distanceSquared = (point - circleCenter).sqrMagnitude;
        return distanceSquared <= radius * radius;
    }
    
    private void Reset()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _edgeCollider.isTrigger = true;
    }

    public void ResetEdgeCollider()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] newPoints = new Vector2[2];

        Vector2 firstPoint = new Vector2(_vertices[_topVerticesIndex[0]].x,
            _vertices[_topVerticesIndex[0]].y);
        Vector2 secondPoint =
            new Vector2(
                _vertices[_topVerticesIndex[_topVerticesIndex.Length - 1]].x,
                _vertices[_topVerticesIndex[_topVerticesIndex.Length - 1]].y);
        newPoints[0] = firstPoint;
        newPoints[1] = secondPoint;

        _edgeCollider.offset = Vector2.zero;

        _edgeCollider.points = newPoints;
    }

    public void GenerateMash()
    {
        _mesh = new Mesh();

        // add vertices
        _vertices = new Vector3[numOfXVertices * NumOfYVertices];
        _topVerticesIndex = new int[numOfXVertices];
        for (int y = 0; y < NumOfYVertices; y++)
        {
            for (int x = 0; x < numOfXVertices; x++)
            {
                float xPos = (x / (float)(numOfXVertices - 1)) * width -
                             width / 2;
                float yPos = (y / (float)(NumOfYVertices - 1)) * height -
                             height / 2;
                _vertices[x + y * numOfXVertices] = new Vector3(xPos, yPos, 0);

                if (y == NumOfYVertices - 1)
                {
                    _topVerticesIndex[x] = y * numOfXVertices + x;
                }
            }
        }

        // construct triangles
        int[] triangles = new int[(numOfXVertices - 1) * 6];
        int index = 0;
        for (int y = 0; y < NumOfYVertices - 1; y++)
        {
            for (int x = 0; x < numOfXVertices - 1; x++)
            {
                int bottomLeft = x + y * numOfXVertices;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + numOfXVertices;
                int topRight = topLeft + 1;

                // first triangle
                triangles[index++] = bottomLeft;
                triangles[index++] = topLeft;
                triangles[index++] = bottomRight;

                // second triangle
                triangles[index++] = bottomRight;
                triangles[index++] = topLeft;
                triangles[index++] = topRight;
            }
        }

        // UVs
        Vector2[] uvs = new Vector2[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i++)
        {
            uvs[i] = new Vector2((_vertices[i].x + width / 2) / width,
                (_vertices[i].y + height / 2) / height);
        }

        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        _meshRenderer.material = waterMaterial;
        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _meshFilter.mesh = _mesh;
    }
}

[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor
{
    private InteractableWater _water;

    private void OnEnable()
    {
        _water = (InteractableWater)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);
        root.Add(new VisualElement { style = { height = 10 } });

        Button generateMeshButton = new Button(() => _water.GenerateMash())
        {
            text = "Generate Mesh"
        };
        root.Add(generateMeshButton);

        Button placeEdgeColliderButton =
            new Button(() => _water.ResetEdgeCollider())
            {
                text = "Place Edge Collider"
            };
        root.Add(placeEdgeColliderButton);

        return root;
    }

    private void ChangeDimensions(ref float width, ref float height,
        float calculatedWidthMax, float calculatedHeightMax)
    {
        width = Mathf.Max(0.1f, calculatedWidthMax);
        height = Mathf.Max(0.1f, calculatedHeightMax);
    }

    private void OnSceneGUI()
    {
        // Draw the wireframe box
        Handles.color = _water.gizmoColor;
        Vector3 center = _water.transform.position;
        Vector3 size = new Vector3(_water.width, _water.height, 0.1f);
        Handles.DrawWireCube(center, size);

        // Handles for width and height
        float handleSize = HandleUtility.GetHandleSize(center) * 0.1f;
        Vector3 snap = Vector3.one * 0.1f;

        // Corner handles
        Vector3[] corners = new Vector3[4];
        corners[0] = center +
                     new Vector3(-_water.width / 2, -_water.height / 2,
                         0); // Bottom-left
        corners[1] =
            center +
            new Vector3(_water.width / 2, -_water.height / 2,
                0); // Bottom-right
        corners[2] =
            center +
            new Vector3(-_water.width / 2, _water.height / 2, 0); // Top-left
        corners[3] =
            center +
            new Vector3(_water.width / 2, _water.height / 2, 0); // Top-right

        // Handle for each corner
        EditorGUI.BeginChangeCheck();
        Vector3 newBottomLeft = Handles.FreeMoveHandle(corners[0], handleSize,
            snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref _water.width, ref _water.height,
                corners[1].x - newBottomLeft.x,
                corners[3].y - newBottomLeft.y);
            _water.transform.position += new Vector3(
                (newBottomLeft.x - corners[0].x) / 2,
                (newBottomLeft.y - corners[0].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newBottomRight = Handles.FreeMoveHandle(corners[1], handleSize,
            snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref _water.width, ref _water.height,
                newBottomRight.x - corners[0].x,
                corners[3].y - newBottomRight.y);
            _water.transform.position += new Vector3(
                (newBottomRight.x - corners[1].x) / 2,
                (newBottomRight.y - corners[1].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newTopLeft = Handles.FreeMoveHandle(corners[2], handleSize,
            snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref _water.width, ref _water.height,
                corners[3].x - newTopLeft.x, newTopLeft.y - corners[0].y);
            _water.transform.position += new Vector3(
                (newTopLeft.x - corners[2].x) / 2,
                (newTopLeft.y - corners[2].y) / 2, 0);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newTopRight = Handles.FreeMoveHandle(corners[3], handleSize,
            snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref _water.width, ref _water.height,
                newTopRight.x - corners[1].x, newTopRight.y - corners[1].y);
            _water.transform.position += new Vector3(
                (newTopRight.x - corners[3].x) / 2,
                (newTopRight.y - corners[3].y) / 2, 0);
        }

        // Update the mesh if the handles are moved
        if (GUI.changed)
        {
            _water.GenerateMash();
        }
    }
}