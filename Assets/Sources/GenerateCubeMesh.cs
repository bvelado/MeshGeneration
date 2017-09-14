using System.Collections.Generic;
using UnityEngine;

public class GenerateCubeMesh : MonoBehaviour {
    
    public struct Ring
    {
        public int bottomLeftVertexIndex;
        public int bottomRightVertexIndex;
        public int topLeftVertexIndex;
        public int topRightVertexIndex;
        public Vector3 bottomLeftVertex;
        public Vector3 bottomRightVertex;
        public Vector3 topLeftVertex;
        public Vector3 topRightVertex;
    }

    public struct CubeMesh
    {
        public Ring backFace;
        public Ring frontFace;
    }

    private MeshFilter meshFilter;
    private Mesh mesh;

    [SerializeField]
    private int numberOfRings = 2;
    [SerializeField]
    private Material cubeMaterial;
    [SerializeField]
    private float cubeSize = 1f;

    private List<CubeMesh> cubes;
    private List<Ring> rings;

	private void Start () {
        mesh = new Mesh();
        mesh.name = "Generated Mesh";
        cubes = new List<CubeMesh>();
        rings = new List<Ring>();

        var generatedGameObject = new GameObject("GeneratedMesh");
        meshFilter = generatedGameObject.AddComponent<MeshFilter>();
        var meshRenderer = generatedGameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = cubeMaterial;

        int currentIndex = 0;
        for(int i = 0; i < numberOfRings; i++)
        {
            GenerateNewRing(ref currentIndex);
        }
        UpdateGeometry();
    }

    /// <summary>
    /// Generate a new ring given the current index
    /// </summary>
    /// <param name="currentIndex">The current vertex index</param>
    /// <returns>The updated index</returns>
    private void GenerateNewRing(ref int currentIndex)
    {
        float forwardOffset = rings.Count * cubeSize;

        Ring ring = new Ring();

        ring.bottomLeftVertexIndex = currentIndex++;
        ring.bottomLeftVertex = new Vector3(0, 0, 0 + forwardOffset);
        ring.bottomRightVertexIndex = currentIndex++;
        ring.bottomRightVertex = new Vector3(cubeSize, 0, 0 + forwardOffset);
        ring.topRightVertexIndex = currentIndex++;
        ring.topRightVertex = new Vector3(cubeSize, cubeSize, 0 + forwardOffset);
        ring.topLeftVertexIndex = currentIndex++;
        ring.topLeftVertex = new Vector3(0, cubeSize, 0 + forwardOffset);

        rings.Add(ring);
    }

    private void UpdateGeometry()
    {
        if(rings.Count > 0)
        {
            var vertices = new Vector3[rings.Count * 4];
            var triangles = new int[(4 + (8 * (rings.Count - 1))) * 3];

            int tIndex = -1;

            foreach (var ring in rings)
            {
                vertices[ring.bottomLeftVertexIndex] = ring.bottomLeftVertex;
                vertices[ring.bottomRightVertexIndex] = ring.bottomRightVertex;
                vertices[ring.topRightVertexIndex] = ring.topRightVertex;
                vertices[ring.topLeftVertexIndex] = ring.topLeftVertex;
            }

            // First ring triangles
            triangles[++tIndex] = rings[0].bottomLeftVertexIndex;
            triangles[++tIndex] = rings[0].topRightVertexIndex;
            triangles[++tIndex] = rings[0].bottomRightVertexIndex;
            triangles[++tIndex] = rings[0].bottomLeftVertexIndex;
            triangles[++tIndex] = rings[0].topLeftVertexIndex;
            triangles[++tIndex] = rings[0].topRightVertexIndex;

            if(rings.Count > 1)
            {
                Ring prevRing = rings[0];
                List<Ring> innerAndLastRings = rings.GetRange(1, rings.Count - 1);
                // From 1 to n
                foreach (var ring in innerAndLastRings)
                {
                    GenerateTrianglesBetweenTwoRings(ref tIndex, ref triangles, prevRing, ring);
                    prevRing = ring;
                }
            }            

            mesh = new Mesh();
            mesh.name = "Generated Mesh";
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            meshFilter.mesh = mesh;
        }
    }

    private void GenerateTrianglesBetweenTwoRings(ref int tIndex, ref int[] triangles, Ring prevRing, Ring currentRing)
    {
        // Right face
        triangles[++tIndex] = prevRing.bottomRightVertexIndex;
        triangles[++tIndex] = currentRing.topRightVertexIndex;
        triangles[++tIndex] = currentRing.bottomRightVertexIndex;
        triangles[++tIndex] = prevRing.bottomRightVertexIndex;
        triangles[++tIndex] = prevRing.topRightVertexIndex;
        triangles[++tIndex] = currentRing.topRightVertexIndex;

        // Left face
        triangles[++tIndex] = currentRing.bottomLeftVertexIndex;
        triangles[++tIndex] = prevRing.topLeftVertexIndex;
        triangles[++tIndex] = prevRing.bottomLeftVertexIndex;
        triangles[++tIndex] = currentRing.bottomLeftVertexIndex;
        triangles[++tIndex] = currentRing.topLeftVertexIndex;
        triangles[++tIndex] = prevRing.topLeftVertexIndex;

        // Top face
        triangles[++tIndex] = prevRing.topLeftVertexIndex;
        triangles[++tIndex] = currentRing.topRightVertexIndex;
        triangles[++tIndex] = prevRing.topRightVertexIndex;
        triangles[++tIndex] = prevRing.topLeftVertexIndex;
        triangles[++tIndex] = currentRing.topLeftVertexIndex;
        triangles[++tIndex] = currentRing.topRightVertexIndex;

        // Bottom face
        triangles[++tIndex] = currentRing.bottomLeftVertexIndex;
        triangles[++tIndex] = prevRing.bottomRightVertexIndex;
        triangles[++tIndex] = currentRing.bottomRightVertexIndex;
        triangles[++tIndex] = currentRing.bottomLeftVertexIndex;
        triangles[++tIndex] = prevRing.bottomLeftVertexIndex;
        triangles[++tIndex] = prevRing.bottomRightVertexIndex;
    }
}
