using UnityEngine;

public static class IGB283Transform {

    public static Matrix3x3 Rotate (float angle) {
        // Create a new matrix
        Matrix3x3 matrix = new Matrix3x3();
        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle), 0.0f));
        matrix.SetRow(1, new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0.0f));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));
        // Return the matrix
        return matrix;
    }

    public static Matrix3x3 Translate (Vector3 offset) {
        // Create a new matrix
        Matrix3x3 matrix = new Matrix3x3();
        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(1.0f, 0.0f, offset.x));
        matrix.SetRow(1, new Vector3(0.0f, 1.0f, offset.y));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));
        // Return the matrix
        return matrix;
    }
}