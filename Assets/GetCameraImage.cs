using UnityEngine;
using System.Collections;
using Vuforia;
using System.IO;
using System.Linq;

public class GetCameraImage : MonoBehaviour
{
    private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGB888;
    private ImageTargetBehaviour mImageTargetBehaviour = null;
    private bool m_RegisteredFormat = false;
    private bool m_LogInfo = true;
    public Material newMaterialRef;


    void Start()
    {
        mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

        if (mImageTargetBehaviour == null)
        {
            Debug.Log("ImageTargetBehaviour not found ");
        }
    }

    public void Update()
    {
        if (mImageTargetBehaviour == null)
        {
            Debug.Log("ImageTargetBehaviour not found");
            return;
        }
        if (!m_RegisteredFormat)
        {
            CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);
            m_RegisteredFormat = true;
        }
        CameraDevice cam = CameraDevice.Instance;
        Image image = cam.GetCameraImage(m_PixelFormat);
        if (image == null)
        {
            Debug.Log(m_PixelFormat + " image is not available yet");
        }
        else
        {
            string s = m_PixelFormat + " image: \n";
            s += "  size: " + image.Width + "x" + image.Height + "\n";
            s += "  bufferSize: " + image.BufferWidth + "x" + image.BufferHeight + "\n";
            s += "  stride: " + image.Stride;
            Debug.Log(s);
            m_LogInfo = false;
            Texture2D tex = new Texture2D(2, 2);

            // Get coordinates
            Vector2 targetSize = mImageTargetBehaviour.GetSize();
            float targetAspect = targetSize.x / targetSize.y;

            // We define a point in the target local reference 
            // we take the bottom-left corner of the target, 
            // just as an example
            // Note: the target reference plane in Unity is X-Z, 
            // while Y is the normal direction to the target plane
            Vector3 pointOnTarget = new Vector3(-0.5f, 0, -0.5f / targetAspect);

            // We convert the local point to world coordinates
            Vector3 targetPointInWorldRef = transform.TransformPoint(pointOnTarget);

            // We project the world coordinates to screen coords (pixels)
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPointInWorldRef);

            Debug.Log("target point in screen coords (bottom left): " + screenPoint.x + ", " + screenPoint.y);

            pointOnTarget = new Vector3(0.5f, 0, -0.5f / targetAspect);
            targetPointInWorldRef = transform.TransformPoint(pointOnTarget);
            Vector3 screenPoint2 = Camera.main.WorldToScreenPoint(targetPointInWorldRef);
            Debug.Log("target point in screen coords (bottom right): " + screenPoint2.x + ", " + screenPoint2.y);

            pointOnTarget = new Vector3(0.5f, 0, 0.5f / targetAspect);
            targetPointInWorldRef = transform.TransformPoint(pointOnTarget);
            Vector3 screenPoint3 = Camera.main.WorldToScreenPoint(targetPointInWorldRef);
            Debug.Log("target point in screen coords (top right): " + screenPoint3.x + ", " + screenPoint3.y);

            pointOnTarget = new Vector3(-0.5f, 0, 0.5f / targetAspect);
            targetPointInWorldRef = transform.TransformPoint(pointOnTarget);
            Vector3 screenPoint4 = Camera.main.WorldToScreenPoint(targetPointInWorldRef);
            Debug.Log("target point in screen coords (top left): " + screenPoint4.x + ", " + screenPoint4.y);


            float[] xValues = { screenPoint.x, screenPoint2.x, screenPoint3.x, screenPoint.x };
            float[] yValues = { screenPoint.y, screenPoint2.y, screenPoint3.y, screenPoint.y };

            image.CopyToTexture(tex);

            Color newBlack = new Color(0,0,0);

            Resolution res = Screen.currentResolution;

            /*int minY = (int)(yValues.Min() / res.height) * tex.height;
            int minX = (int)(xValues.Min() / res.width) * tex.width;
            int maxY = (int)(yValues.Max() / res.height) * tex.height;
            int maxX = (int)(xValues.Max() / res.width) * tex.width;*/

            // Will need to change each coordinate to match texture size
            screenPoint.x = (screenPoint.x / res.width) * tex.width;
            screenPoint.y = (screenPoint.y / res.height) * tex.height;
            screenPoint2.x = (screenPoint2.x / res.width) * tex.width;
            screenPoint2.y = (screenPoint2.y / res.height) * tex.height;
            screenPoint3.x = (screenPoint3.x / res.width) * tex.width;
            screenPoint3.y = (screenPoint3.y / res.height) * tex.height;
            screenPoint4.x = (screenPoint4.x / res.width) * tex.width;
            screenPoint4.y = (screenPoint4.y / res.height) * tex.height;

            // Calculate expression (gradient and offset)

            // for now just to block out the whole rectangle covering the image
            for (int i = minY; i < maxY; i++)
            {
                for (int j = minX; j < maxX; j++)
                {
                    tex.SetPixel(j, i, newBlack);
                }

            }

            Debug.Log("dimensions of texture: " + tex.height + ", " + tex.width);

            newMaterialRef.mainTexture = tex;
            tex.Apply();

            // Encode texture into PNG
            //byte[] bytes = tex.EncodeToPNG();

            // For testing purposes, also write to a file in the project folder
            //File.WriteAllBytes(Application.persistentDataPath + "/../SavedScreen.png", bytes);

        }
    }
}
