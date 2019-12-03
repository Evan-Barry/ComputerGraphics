using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Transformations : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        //rotation
        Vector3 startingAxis = new Vector3(15,-2,-2);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(26, startingAxis);
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(new Vector3(0,0,0),
                            rotation,
                            Vector3.one);
        printMatrix(rotationMatrix, "Rotation Matrix");

        Vector3[] imageAfterRotation =
            MatrixTransform(cube, rotationMatrix);
        printVerts(imageAfterRotation, "rotation");

        //scale
        Matrix4x4 scaleMatrix =
            Matrix4x4.TRS(new Vector3(0, 0, 0),
                            Quaternion.identity,
                            new Vector3(15, 2, 2));
        printMatrix(scaleMatrix, "Scale Matrix");

        Vector3[] imageAfterScale =
            MatrixTransform(imageAfterRotation, scaleMatrix);
        printVerts(imageAfterScale, "scale");

        //translation
        Matrix4x4 translationMatrix =
            Matrix4x4.TRS(new Vector3(1, 2, -1),
                            Quaternion.identity,
                            Vector3.one);
        printMatrix(translationMatrix, "Translation Matrix");

        Vector3[] imageAfterTranslation =
            MatrixTransform(imageAfterScale, translationMatrix);
        printVerts(imageAfterTranslation, "translation");

        //super matrix
        Matrix4x4 superMatrix = translationMatrix * scaleMatrix * rotationMatrix;
        printMatrix(superMatrix, "Super Matrix");

        Vector3[] imageAfterSuperMatrix =
            MatrixTransform(cube, superMatrix);
        printVerts(imageAfterSuperMatrix, "Super Matrix");

        //viewing matrix
        Matrix4x4 viewingMatrix =
            Matrix4x4.TRS(new Vector3(-17, -1, -48),
                            Quaternion.LookRotation((new Vector3(-2, 15, 2) - new Vector3(17, 1, 48)), new Vector3(-1, -2, 15).normalized),
                            Vector3.one);
        printMatrix(viewingMatrix, "Viewing Matrix");

        Vector3[] imageAfterViewingMatrix =
            MatrixTransform(imageAfterTranslation, viewingMatrix);
        printVerts(imageAfterViewingMatrix, "Viewing Matrix");

        //projection matrix

        Matrix4x4 projectionMatrix = Matrix4x4.Perspective(45f, 1.6f, 1f, 1000f);
        printMatrix(projectionMatrix, "Projection Matrix");

        Vector3[] imageAfterProjectionMatrix =
            MatrixTransform(imageAfterViewingMatrix, projectionMatrix);
        printVerts(imageAfterProjectionMatrix, "Projection Matrix");

        //super mega matrix
        Matrix4x4 superMegaMatrix = projectionMatrix * viewingMatrix * superMatrix;
        printMatrix(superMegaMatrix, "Super Mega Matrix");

        Vector3[] imageAfterSuperMegaMatrix =
            MatrixTransform(cube, superMegaMatrix);
        printVerts(imageAfterSuperMegaMatrix, "Super Mega Matrix");

	
	}

    private void printVerts(Vector3[] newImage, String transformation)
    {      
        for (int i = 0; i < newImage.Length; i++)
        {
            print(transformation + "[" + i + "]\n" +
                newImage[i].x + " , " +
                newImage[i].y + " , " +
                newImage[i].z + "\n");
        }
    }

    private Vector3[] MatrixTransform(
        Vector3[] meshVertices, 
        Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix * 
                new Vector4( 
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;
    }

    private void printMatrix(Matrix4x4 matrix, string matrixName)
    {
        print(matrixName);
        for (int i = 0; i < 4; i++)
            print(matrix.GetRow(i).ToString());
    }



    // Update is called once per frame
    void Update () {
	
	}
}
