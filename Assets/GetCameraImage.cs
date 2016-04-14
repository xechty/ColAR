using UnityEngine;
using System.Collections;
using Vuforia;
using System.IO;

public class GetCameraImage : MonoBehaviour
{
    private Image.PIXEL_FORMAT m_PixelFormat = Image.PIXEL_FORMAT.RGB888;
    private bool m_RegisteredFormat = false;
    private bool m_LogInfo = true;
    public Material newMaterialRef;
    void Start()
    {
        VuforiaBehaviour vuforiaBehaviour = (VuforiaBehaviour)FindObjectOfType(typeof(VuforiaBehaviour));
        if (vuforiaBehaviour)
        {
            vuforiaBehaviour.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        }
    }
    public void OnTrackablesUpdated()
    {
        if (!m_RegisteredFormat)
        {
            CameraDevice.Instance.SetFrameFormat(m_PixelFormat, true);
            m_RegisteredFormat = true;
        }
        if (m_LogInfo)
        {
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
                //tex.LoadImage(image.Pixels);
                //tex.Apply();
                image.CopyToTexture(tex);
                newMaterialRef.mainTexture = tex;
                tex.Apply();

                // Encode texture into PNG
                byte[] bytes = tex.EncodeToPNG();

                // For testing purposes, also write to a file in the project folder
                File.WriteAllBytes(Application.persistentDataPath + "/../SavedScreen.png", bytes);

            }

        }
    }
}
