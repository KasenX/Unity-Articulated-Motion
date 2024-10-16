using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Limb : MonoBehaviour
{
    public GameObject child;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public Vector3[] limbVertexLocations;
    public Mesh mesh;
    public Material material;
    public Color color;

    public Vector3 position;
    private float angle;
    private float lastAngle;
    private float altitude = 0;

    // This will run before Start
    void Awake() {
        DrawLimb();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (child != null) {
            child.GetComponent<Limb>().MoveByOffset(jointOffset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        lastAngle = angle;

        if (child != null) {
            child.GetComponent<Limb>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }

        mesh.RecalculateBounds();
        */
    }

    private void DrawLimb() {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        // Get the Mesh from the MeshFilter
        mesh = GetComponent<MeshFilter>().mesh;

        GetComponent<MeshRenderer>().material = material;

        // Clear all vertex and index data from the mesh
        mesh.Clear();

        mesh.vertices = limbVertexLocations;

        mesh.colors = new Color[] { color, color, color, color };

        // Set vertex indicies
        mesh.triangles = new int[] {0, 1, 2, 0, 2, 3};

        mesh.RecalculateBounds();
    }

    private void MoveByOffset(Vector3 offset) {
        Matrix3x3 T = IGB283Transform.Translate(offset);

        Vector3[] vertices = mesh.vertices;

        // Rotate each point in the mesh to its new position
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }

        mesh.vertices = vertices;
        
        jointLocation = T.MultiplyPoint(jointLocation);

        if (child != null) {
            child.GetComponent<Limb>().MoveByOffset(offset);
        }
    }

    private void RotateAroundPoint(Vector3 point, float angle, float lastAngle) {
        // Move the point to the origin
        Matrix3x3 T = IGB283Transform.Translate(-point);
        // Undo the last rotation
        Matrix3x3 R = IGB283Transform.Rotate(-lastAngle);
        // Move the point back to the original position
        Matrix3x3 T2 = IGB283Transform.Translate(point);
        // Perform the new rotation
        Matrix3x3 R2 = IGB283Transform.Rotate(angle);
        // The final transformation matrix
        Matrix3x3 M = T2 * R2 * R * T;

        Vector3[] vertices = mesh.vertices;

        // Rotate each point in the mesh to its new position
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }

        mesh.vertices = vertices;
        // Apply the transformation to the joint
        jointLocation = M.MultiplyPoint(jointLocation);
        // Apply the transformation to the child
        if (child != null) {
            child.GetComponent<Limb>().RotateAroundPoint(point, angle, lastAngle);
        }
    }

    public bool MoveLimb(float moveSpeed, float targetX) {
        Vector3 offset = new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        MoveByOffset(offset);
        // Update the position
        position += offset;
        // Check if the limb has reached the target distance
        return moveSpeed >= 0 ? position.x >= targetX : position.x <= targetX;
    }

    public bool Rotate(float speed, float targetAngle)
    {
        targetAngle *= Mathf.Deg2Rad;
        // Save the last angle
        lastAngle = angle;

        float increment = speed * Time.deltaTime;
        angle += increment;
        
        if (child != null) {
            child.GetComponent<Limb>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }
        // Check if the limb has reached the target angle
        return speed >= 0 ? angle >= targetAngle : angle <= targetAngle;
    }

    public void Rotate0() {
        lastAngle = angle;
        angle = 0;
        if (child != null) {
            child.GetComponent<Limb>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }
    }

    public void Rotate180() {
        lastAngle = angle;
        angle = Mathf.PI;
        if (child != null) {
            child.GetComponent<Limb>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }
    }

    public bool IncreaseAltitude(float speed, float targetY) {
        float increment = speed * Time.deltaTime;
        altitude += increment;
        MoveByOffset(new Vector3(0, increment, 0));
        return speed > 0 ? altitude >= targetY : altitude <= targetY;
    }
}
