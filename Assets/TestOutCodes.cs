using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutCodes : MonoBehaviour
{
    static Texture2D screen;
    static int resWidth = Screen.width;
    static int resHeight = Screen.height;
    public Vector3[] cube;
    private float angle;
    Renderer ourScreen;
    Color defaultColour;
    public float rotationRate = 1;
    public Vector3 rotationAxis = new Vector3(0f, 0f, 0f);
    public Light light;
    // Start is called before the first frame update
    void Start()
    {
        ourScreen = FindObjectOfType<Renderer>();

        screen = new Texture2D(resWidth, resHeight, TextureFormat.RGBA32, false);
        defaultColour = new Color(screen.GetPixel(1, 1).r, screen.GetPixel(1, 1).g, screen.GetPixel(1, 1).b, screen.GetPixel(1, 1).a);
        ourScreen.material.mainTexture = screen;

        print("Screen Resolution - " + resWidth + " * " + resHeight);

        cube = new Vector3[8];
        cube[0] = new Vector3(1f, 1f, 1);
        cube[1] = new Vector3(-1f, 1f, 1);
        cube[2] = new Vector3(-1f, -1f, 1);
        cube[3] = new Vector3(1f, -1f, 1);
        cube[4] = new Vector3(1f, 1f, -1);
        cube[5] = new Vector3(-1f, 1f, -1);
        cube[6] = new Vector3(-1f, -1f, -1);
        cube[7] = new Vector3(1f, -1f, -1);

        cube = applyTranslateMatrix(cube, 2f, 2f, 0);
    }

    private void drawLine(Vector2 v1, Vector2 v2, Texture2D screen)
    {
        Vector2 start = v1, end = v2;

        if (lineClip(ref start, ref end))
        {
            List<Vector2Int> breshList01 = Breshenham(convertToScreenPoint(start), convertToScreenPoint(end));
            displayBresh(screen, breshList01);
        }
        else
        {
            print("rejected");
        }
    }

    public static bool lineClip(ref Vector2 v, ref Vector2 u)
    {
        Outcode v_outcode = new Outcode(v);
        Outcode u_outcode = new Outcode(u);
        Outcode inViewport = new Outcode();

        if ((u_outcode + v_outcode) == inViewport)
        {
            return true;
        }

        if ((u_outcode * v_outcode) != inViewport)
        {
            print("TR");
            return false;
        }

        if(v_outcode == inViewport)
        {
            return lineClip(ref u, ref v);
        }

        if(v_outcode.up)
        {
            v = intercept(u, v, 0);

            Outcode v2_outcode = new Outcode(v);

            if(v2_outcode == inViewport)
            {
                return lineClip(ref u, ref v);
            }
        }

        if (v_outcode.down)
        {
            v = intercept(u, v, 1);

            Outcode v2_outcode = new Outcode(v);

            if (v2_outcode == inViewport)
            {
                return lineClip(ref u, ref v);
            }
        }

        if (v_outcode.left)
        {
            v = intercept(u, v, 2);

            Outcode v2_outcode = new Outcode(v);

            if (v2_outcode == inViewport)
            {
                return lineClip(ref u, ref v);
            }
        }

        if (v_outcode.right)
        {
            v = intercept(u, v, 3);

            Outcode v2_outcode = new Outcode(v);

            if (v2_outcode == inViewport)
            {
                return lineClip(ref u, ref v);
            }
        }

        return false;

    }

    private static Vector2 intercept(Vector3 u, Vector3 v, int edge)
    {
        float m = (v.y - u.y)/(v.x - u.x);

        if(edge == 0)
        {
            return new Vector2(u.x + (1 / m) * (1 - u.y), 1);
        }

        if (edge == 1)
        {
            return new Vector2(u.x + (1 / m) * (-1 - u.y), -1);
        }

        if (edge == 2)
        {
            return new Vector2(-1, u.y + m * (-1 - u.x));
        }

        return new Vector2(1, u.y + m * (1 - u.x));
    }

    public static List<Vector2Int> Breshenham(Vector2Int start, Vector2Int finish)
    {
        List<Vector2Int> breshList = new List<Vector2Int>();

        int dy = finish.y - start.y;
        int dx = finish.x - start.x;
        int a = 2*dy;
        int b = 2 * (dy - dx);
        int p = a - dx;

        if(dx < 0)
        {
            return Breshenham(finish, start);
        }

        if(dy < 0)
        {
            return negativeY(Breshenham(negativeY(start), negativeY(finish)));
        }

        if(dy > dx)
        {
            return swapXY(Breshenham(swapXY(start), swapXY(finish)));
        }

        int y = start.y;
        for(int x = start.x; x <= finish.x; x++)
        {
            breshList.Add(new Vector2Int(x, y));

            if(p > 0)
            {
                y++;
                p += b;
            }

            else
            {
                p += a;
            }
        }

        return breshList;
    }

    public static List<Vector2Int> negativeY(List<Vector2Int> points)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach (Vector2Int v in points)
        {
            outputList.Add(negativeY(v));

        }

        return outputList;
    }

    public static Vector2Int negativeY(Vector2Int points)
    {
        return new Vector2Int(points.x, points.y *= -1);
    }

    public static List<Vector2Int> swapXY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach(Vector2Int v in list)
        {
            outputList.Add(swapXY(v));
        }

        return outputList;
    }

    public static Vector2Int swapXY(Vector2Int point)
    {
        return new Vector2Int(point.y, point.x);
    }

    private Matrix4x4 viewingMatrix(Vector3 posOfCam, Vector3 taregt, Vector3 up)
    {
        return Matrix4x4.TRS(-posOfCam, Quaternion.LookRotation(taregt - posOfCam, up.normalized), Vector3.one);
    }

    public static Vector3[] applyViewingMatrix(Vector3[] cube)
    {
        Matrix4x4 viewingMatrix = Matrix4x4.TRS(new Vector3(0, 0, 10), Quaternion.LookRotation((new Vector3(0, 0, 0) - new Vector3(0, 0, 10)), new Vector3(0, 1, 0).normalized), Vector3.one);

        Vector3[] imageAfterViewingMatrix =MatrixTransform(cube, viewingMatrix);

        return imageAfterViewingMatrix;
    }

    public static Vector3[] applyProjectionMatrix(Vector3[] cube)
    {
        Matrix4x4 projectionMatrix = Matrix4x4.Perspective(45f, 1.6f, 1f, 1000f);

        Vector3[] imageAfterProjectionMatrix = MatrixTransform(cube, projectionMatrix);

        return imageAfterProjectionMatrix;
    }

    private Matrix4x4 rotationMatrix(Vector3 axis, float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis.normalized);
        return Matrix4x4.TRS(new Vector3(0, 0, 0), rotation, Vector3.one);
    }

    public static Vector3[] applyRotationMatrix(Vector3[] cube)
    {
        Vector3 startingAxis = new Vector3(15, 2, 2);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(1, startingAxis);
        Matrix4x4 rotationMatrix =  Matrix4x4.TRS(new Vector3(0, 0, 0), rotation, Vector3.one);

        Vector3[] imageAfterRotation = MatrixTransform(cube, rotationMatrix);

        return imageAfterRotation;
    }

    public static Vector3[] applyTranslateMatrix(Vector3[] cube, float x, float y, float z)
    {
        Matrix4x4 translationMatrix = Matrix4x4.TRS(new Vector3(x, y, z), Quaternion.identity, Vector3.one);

        Vector3[] imageAfterTranslation = MatrixTransform(cube, translationMatrix);

        return imageAfterTranslation;
    }

    private Matrix4x4 translateMatrix(Vector3 v)
    {
        return Matrix4x4.TRS(v, Quaternion.identity, Vector3.one);
    }

    private static Vector3[] MatrixTransform(
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

    public static Vector2Int convertToScreenPoint(Vector2 v)
    {
        Vector2Int s = new Vector2Int((int)Math.Round((v.x + 1) / 2 * (resWidth - 1), 0), (int)Math.Round((1 - v.y) / 2 * (resHeight - 1), 0));
        return s;
    }

    public static void displayBresh(Texture2D screen, List<Vector2Int> breshList)
    {
        foreach (Vector2Int point in breshList)
        {
            screen.SetPixel(point.x, point.y, Color.red);
        }
    }
    

    private Vector3[] divideByZ(Vector3[] cube)
    {
        List<Vector3> output = new List<Vector3>();

        foreach(Vector3 v in cube)
        {
            output.Add(new Vector2(v.x / v.z, v.y / v.z));
        }

        return output.ToArray();
    }

    private void drawCube(Vector3[] cube)
    {
        //Front
        //t1
        drawFace(cube[0], cube[1], cube[2]);

        //t2
        drawFace(cube[0], cube[2], cube[3]);

        //Right
        //t1
        drawFace(cube[4], cube[0], cube[3]);
        //t2
        drawFace(cube[4], cube[3], cube[7]);


        //Top
        //t1
        drawFace(cube[4], cube[5], cube[1]);

        //t2
        drawFace(cube[4], cube[1], cube[0]);

        //Back
        //t1
        drawFace(cube[5], cube[4], cube[7]);

        //t2
        drawFace(cube[5], cube[7], cube[6]);

        //Left
        //t1
        drawFace(cube[1], cube[5], cube[6]);

        //t2
        drawFace(cube[1], cube[6], cube[2]);

        //Bottom
        //t1
        drawFace(cube[6], cube[7], cube[3]);

        //t2
        drawFace(cube[6], cube[3], cube[2]);
    }

    public void drawFace(Vector2 i, Vector2 j, Vector2 k)
    {
        float z = (j.x - i.x)*(k.y - j.y) - (j.y - i.y)*(k.x - j.x);

        if (z >= 0)
        {
            drawLine(i, j, screen);
            drawLine(j, k, screen);
            drawLine(k, i, screen);

            Vector2 Center = getCenter(i, j, k);
            Center = convertToScreenPoint(Center);
            Vector3 normal = getNormal(i, j, k);
            Vector3 lightDir = getLightDir(Center);
            float dotProduct = Vector3.Dot(lightDir, normal);
            Color reflection = new Color(dotProduct * Color.green.r * light.intensity, dotProduct * Color.green.g * light.intensity, dotProduct * Color.green.b * light.intensity, 1);
            print(reflection);
            floodFillStack((int)Center.x, (int)Center.y, reflection, defaultColour);
        }
    }

    public Vector2 getCenter(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return new Vector2((p1.x + p2.x + p3.x) / 3, (p1.y + p2.y + p3.y) / 3);
    }

    public Vector3 getNormal(Vector2 a, Vector2 b, Vector2 c)
    {
        return Vector3.Normalize(Vector3.Cross(b - a, c - a));
    }

    public Vector3 getLightDir(Vector3 center)
    {
        return Vector3.Normalize((center - light.transform.position));
    }

    public void floodFillStack(int x, int y, Color newColour, Color oldColour)
    {
        Stack<Vector2> pixels = new Stack<Vector2>();
        pixels.Push(new Vector2(x, y));

        while(pixels.Count > 0)
        {
            Vector2 p = pixels.Pop();
            if(checkBounds(p))
            {
                if(screen.GetPixel((int)p.x, (int)p.y) == oldColour)
                {
                    screen.SetPixel((int)p.x, (int)p.y, newColour);
                    pixels.Push(new Vector2(p.x + 1, p.y));
                    pixels.Push(new Vector2(p.x - 1, p.y));
                    pixels.Push(new Vector2(p.x, p.y + 1));
                    pixels.Push(new Vector2(p.x, p.y - 1));
                }
            }
        }
    }

    public bool checkBounds(Vector2 pixel)
    {
        if ((pixel.x < 0) || (pixel.x >= resWidth - 1))
        {
            print("pixel out of bounds");
            return false;
        }

        if ((pixel.y < 0) || (pixel.y >= resHeight - 1))
        {
            print("pixel out of bounds");
            return false;
        }

        return true;
    }
    
    // Update is called once per frame
    void Update()
    {
        Destroy(screen);
        screen = new Texture2D(resWidth, resHeight);
        ourScreen.material.mainTexture = screen;


        angle += rotationRate;
        Matrix4x4 persp = Matrix4x4.Perspective(45, 1.6f, 1, 1000);
        Matrix4x4 view = viewingMatrix(new Vector3(0,0,10), new Vector3(0,0,0), new Vector3(0,1,0));
        Matrix4x4 world = rotationMatrix(rotationAxis, angle);
        Matrix4x4 overall = persp * view * world;

        drawCube(divideByZ( MatrixTransform(cube, overall)));

        screen.Apply();
    }
}
