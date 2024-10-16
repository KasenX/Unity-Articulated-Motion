using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryObject : MonoBehaviour
{
    // Used Vector3 so that it can be serialized in the inspector
    public Vector3 position;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshRenderer>().material = material;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 10, 0),
            new Vector3(1, 10, 0),
            new Vector3(1, 0, 0),
        };

        Color[] colors = new Color[]
        {
            Color.black,
            Color.black,
            Color.black,
            Color.black,
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
        };

        Matrix3x3 T = IGB283Transform.Translate(position);

        for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = T.MultiplyPoint(vertices[i]);
		}

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}