using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rogue.Tool.MeshCreator
{
    public class MeshCreator : OdinEditorWindow
    {
        const float FixedYPos = 0.1f;
        const float LeastHeight = 0.01f;
        public enum ShapeType
        {
            Circular,
            Box,
        }

        [MenuItem("Tools/Rogue/Util/Mesh Creator")]
        public static void ShowWindow()
        {
            GetWindow<MeshCreator>().Show();
        }

        [SerializeField] ShapeType shapeType;

        [SerializeField] string meshName;

        [Tooltip("가로")]
        [ShowIf("shapeType", ShapeType.Box)]
        [SerializeField] float width;

        [Tooltip("세로")]
        [ShowIf("shapeType", ShapeType.Box)]
        [SerializeField] float length;

        [Tooltip("반지름")]
        [ShowIf("shapeType", ShapeType.Circular)]
        [SerializeField] float radius;

        [Tooltip("꼭짓점 갯수")]
        [ShowIf("shapeType", ShapeType.Circular)]
        [SerializeField] int splitCount;

        [Tooltip("완성한 오브젝트의 높이를 추가")]
        [SerializeField] float height;

        [Tooltip("호가 표현되는 최대 각도")]
        [ShowIf("shapeType", ShapeType.Circular)]
        [SerializeField] float maxDegree = 360;

        [Tooltip("호의 중심이 위를 바라볼 수 있도록 변경")]
        [SerializeField] bool isCenter;

        [Tooltip("내원 추가")]
        [SerializeField] bool innerShape = false;

        [Tooltip("내원 반지름(외원의 반지름보다 크면 안됨.)")]
        [ShowIfGroup("innerShape")]
        [ShowIf("shapeType", ShapeType.Circular)]
        [BoxGroup("innerShape/Shape")]
        [SerializeField] float innerRadius;

        [ShowIf("innerShape")]
        [ShowIf("shapeType", ShapeType.Box)]
        [BoxGroup("innerShape/Box")]
        [SerializeField] float innerWidth;

        [ShowIf("innerShape")]
        [ShowIf("shapeType", ShapeType.Box)]
        [BoxGroup("innerShape/Box")]
        [SerializeField] float innerLength;

        [ShowIf("innerShape")]
        [ShowIf("shapeType", ShapeType.Box)]
        [BoxGroup("innerShape/Box")]
        [SerializeField] float startX;

        [ShowIf("innerShape")]
        [ShowIf("shapeType", ShapeType.Box)]
        [BoxGroup("innerShape/Box")]
        [SerializeField] float startY;

        [SerializeField] bool createMeshAsset = true;
        [ShowIf("createMeshAsset")]
        [FolderPath]
        [SerializeField] string meshPath = "Assets/Mesh";
        [SerializeField] bool createGameObject = false;
        [PreviewField]
        [ShowIf("createGameObject")]
        [SerializeField] Material material;
        private string defaultMeshName = "Mesh_{0}";

        [Button]
        public void GenerateMesh()
        {
            GenerateMesh(createMeshAsset, createGameObject);
        }

        public void GenerateMesh(bool createMeshAsset, bool createGameObject)
        {
            // 이후 여러번 사용할 오브젝트의 이름을 미리 변수로 만든다.
            string objectName = string.Format(defaultMeshName, meshName);

            // ---Mesh파일 생성
            Mesh mesh = new Mesh { name = objectName };
            CreateShape(out var vertices, out var triangles);
            if (height < LeastHeight)
                height = LeastHeight;
            ToPillar(ref vertices, ref triangles, height);
            CombineMesh(mesh, vertices, triangles);

            // ---GameObject로 생성
            if (createGameObject)
            {
                GameObject TestObj = new GameObject(objectName);
                TestObj.AddComponent<MeshFilter>().mesh = mesh;
                var meshRenderer = TestObj.AddComponent<MeshRenderer>();
                meshRenderer.material = material ?? null;
            }

            // ---에셋으로 저장
            if (createMeshAsset)
                CreateMesh2Asset(mesh, objectName);
        }

        public Mesh CombineMesh(Mesh mesh, List<Vector3> planeVerticlesList, List<int> planeTrianglesList)
        {
            mesh.vertices = planeVerticlesList.ToArray();
            mesh.triangles = planeTrianglesList.ToArray();
            return mesh;
        }

        public void CreateMesh2Asset(Object meshObject, string obectName)
        {
            string meshPathWithExtension = Rogue.Tool.EditorHelper.GetUniqueFileDirectory($"{meshPath}/{obectName}", "mesh");
            AssetDatabase.CreateAsset(meshObject, meshPathWithExtension);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CreateShape(out List<Vector3> vertices, out List<int> triangles)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            switch (shapeType)
            {
                case ShapeType.Circular:
                    CreateCircular(ref vertices, ref triangles);
                    break;
                case ShapeType.Box:
                    CreateBox(ref vertices, ref triangles);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreateCircular(ref List<Vector3> vertices, ref List<int> triangles)
        {
            vertices.AddRange(GenerateVerticesList(radius, false));
            if (innerShape)
                vertices.AddRange(GenerateVerticesList(innerRadius, true));
            else
                vertices.Add(new Vector3(0, FixedYPos, 0));
            triangles = GenerateTriangleList(splitCount);
        }

        private void CreateBox(ref List<Vector3> vertices, ref List<int> triangles)
        {
            vertices.AddRange(GenerateBoxVerticesList(width, length));
            if (innerShape)
                vertices.AddRange(GenerateBoxVerticesList(innerWidth, innerLength, startX, startY, true));
            else
                vertices.Add(new Vector3(0, FixedYPos, 0));
            triangles = GenerateTriangleList(4);
        }

        List<int> GenerateTriangleList(int vertexCount)
        {
            var triangleList = new List<int>();
            if (innerShape)
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    triangleList.AddRange(Vertex2SquarePlane(vertexCount * 2 + 1 - i, vertexCount * 2 - i, i, i + 1));
                }
            }
            else
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    triangleList.AddRange(Vertex2TrianglePlane(i, i + 1, vertexCount + 1));
                }
            }
            return triangleList;
        }


        IEnumerable<int> Vertex2SquarePlane(int BottomLeft, int BottomRight, int TopLeft, int TopRight)
        {
            yield return BottomLeft;
            yield return TopLeft;
            yield return BottomRight;

            yield return BottomRight;
            yield return TopLeft;
            yield return TopRight;
        }

        IEnumerable<int> Vertex2TrianglePlane(int p1, int p2, int p3)
        {
            yield return p1;
            yield return p2;
            yield return p3;
        }

        public void ToPillar(ref List<Vector3> vertices, ref List<int> triangles, float height)
        {
            var origTriCount = triangles.Count;
            var origVertexCount = vertices.Count;
            var temp = new List<Vector3>(vertices);
            vertices.Clear();
            vertices.AddRange(temp.Select(x => x + Vector3.up * height));
            vertices.AddRange(temp);

            for (var i = 0; i < origTriCount; i += 3)
            {
                triangles.Add(triangles[i] + origVertexCount);
                triangles.Add(triangles[i + 2] + origVertexCount);
                triangles.Add(triangles[i + 1] + origVertexCount);
            }

            for (var i = 0; i < origVertexCount; i++)
            {
                var next = (i + 1) % origVertexCount;
                triangles.AddRange(Vertex2SquarePlane(origVertexCount + next, origVertexCount + i, next, i));
            }
        }


        List<Vector3> GenerateVerticesList(float radius, bool counterclockwise)
        {
            var vertices = new List<Vector3>();
            for (var i = 0; i <= splitCount; i++)
            {
                var roundValue = ((maxDegree / splitCount) * i - (isCenter ? maxDegree / 2 : 0)) * Mathf.Deg2Rad;
                if (counterclockwise)
                    vertices.Insert(0, new Vector3(Mathf.Sin(roundValue) * radius, FixedYPos, Mathf.Cos(roundValue) * radius));
                else
                    vertices.Add(new Vector3(Mathf.Sin(roundValue) * radius, FixedYPos, Mathf.Cos(roundValue) * radius));
            }
            return vertices;
        }

        List<Vector3> GenerateBoxVerticesList(float width, float length, float startX = 0, float startY = 0, bool counterclockwise = false)
        {
            List<Vector3> verticleList = new List<Vector3>();
            // 좌상위치에서 시계방향으로 돌아가며 vertex를 찍는다.
            float middleWidth = width / 2;
            float middleLength = length / 2;
            if (counterclockwise)
            {
                verticleList.Insert(0, new Vector3((isCenter ? -middleWidth : 0) + startX, FixedYPos, (isCenter ? middleLength : length) + startY));
                verticleList.Insert(0, new Vector3((isCenter ? middleWidth : width) + startX, FixedYPos, (isCenter ? middleLength : length) + startY));
                verticleList.Insert(0, new Vector3((isCenter ? middleWidth : width) + startX, FixedYPos, (isCenter ? -middleLength : 0) + startY));
                verticleList.Insert(0, new Vector3((isCenter ? -middleWidth : 0) + startX, FixedYPos, (isCenter ? -middleLength : 0) + startY));
                verticleList.Insert(0, new Vector3((isCenter ? -middleWidth : 0) + startX, FixedYPos, (isCenter ? middleLength : length) + startY));
            }
            else
            {
                verticleList.Add(new Vector3((isCenter ? -middleWidth : 0) + startX, FixedYPos, (isCenter ? middleLength : length) + startY));
                verticleList.Add(new Vector3((isCenter ? middleWidth : width) + startX, FixedYPos, (isCenter ? middleLength : length) + startY));
                verticleList.Add(new Vector3((isCenter ? middleWidth : width) + startX, FixedYPos, (isCenter ? -middleLength : 0) + startY));
                verticleList.Add(new Vector3((isCenter ? -middleWidth : 0) + startX, FixedYPos, (isCenter ? -middleLength : 0) + startY));
                verticleList.Add(new Vector3((isCenter ? -middleWidth : 0) + startX, FixedYPos, (isCenter ? middleLength : length) + startY));
            }
            return verticleList;
        }
    }
}
