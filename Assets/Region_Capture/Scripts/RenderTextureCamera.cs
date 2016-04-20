using UnityEngine;
using System.Collections;
using Vuforia;

#if UNITY_EDITOR
#pragma warning disable 0219
#endif

public class RenderTextureCamera : MonoBehaviour
{
	[RC_Extension.ShowAsLayerAttribute]
    public int CameraLayer = 20;

	[Space(20)]
	public int TextureResolution = 512;
	[Space(10)]
	public Texture SetMarkerBackground;
	public bool MarkerBackgroundAlpha = false;
    private string screensPath;
    private int TextureResolutionX;
	private int TextureResolutionY;
	[Space(20)]
	public bool ShowTexture = false;
	public static Camera ARCamera_Camera;
	private Camera Render_Texture_Camera;
	private GameObject DebugGUITexture;
	private RenderTexture CameraOutputTexture;

    public RenderTexture GetRenderTexture()
    {
        return CameraOutputTexture;
    }

	void Start() {

		StartCoroutine(StartRenderingToTexture());

	}

		IEnumerator StartRenderingToTexture() {

		if (SetMarkerBackground) GetComponent<Renderer>().material.SetTexture ("_BackTex", SetMarkerBackground);
		if (MarkerBackgroundAlpha) GetComponent<Renderer>().material.SetFloat ("_AlphaBack", 0);
		else GetComponent<Renderer>().material.SetFloat ("_AlphaBack", 1);
		yield return new WaitForSeconds(0.5f);

		Render_Texture_Camera = GetComponentInChildren<Camera>();
		Render_Texture_Camera.GetComponent<Camera>().orthographicSize = transform.localScale.z * 5;

		if (GetComponentInParent<ImageTargetBehaviour> ())
			Render_Texture_Camera.GetComponent<Camera> ().orthographicSize *= 20.0f;


		if (transform.localScale.x >= transform.localScale.z) {

			TextureResolutionX = TextureResolution;
			TextureResolutionY = (int)(TextureResolution * transform.localScale.z / transform.localScale.x);
		}

		if (transform.localScale.x < transform.localScale.z) {

			TextureResolutionX =  (int)(TextureResolution * transform.localScale.x / transform.localScale.z);
			TextureResolutionY = TextureResolution;
		}

		CameraOutputTexture = new RenderTexture(TextureResolutionX, TextureResolutionY, 0);
		CameraOutputTexture.Create();
		Render_Texture_Camera.GetComponent<Camera>().targetTexture = CameraOutputTexture;

		gameObject.layer = CameraLayer;
		Render_Texture_Camera.cullingMask = 1 << CameraLayer;

		StartCoroutine(ShowTextureOnGUI());
	}


    IEnumerator ShowTextureOnGUI() {

		if (ShowTexture) {

			if (!DebugGUITexture) {
				DebugGUITexture = new GameObject ("Debug GUI Texture");
				GUITexture DebugTexture = DebugGUITexture.AddComponent<GUITexture> ();
				DebugTexture.color = Color.gray;
				DebugTexture.texture = CameraOutputTexture;

				ARCamera_Camera = VuforiaManager.Instance.ARCameraTransform.GetComponentInChildren<Camera>();
				float GuiTextureAspect = TextureResolutionX / (TextureResolutionY * ARCamera_Camera.aspect);

				DebugGUITexture.transform.localScale = new Vector3 (0.3f * GuiTextureAspect, 0.3f, 0.3f);				
				DebugGUITexture.transform.position = new Vector3 ((1.0f - (0.3f * GuiTextureAspect / 2)) - 0.1f * (1 / ARCamera_Camera.aspect), 0.25f, 0.0f);
			}
			else {
				
				float GuiTextureAspect = TextureResolutionX / (TextureResolutionY * ARCamera_Camera.aspect);

				DebugGUITexture.transform.localScale = new Vector3 (0.3f * GuiTextureAspect, 0.3f, 0.3f);				
				DebugGUITexture.transform.position = new Vector3 ((1.0f - (0.3f * GuiTextureAspect / 2)) - 0.1f * (1 / ARCamera_Camera.aspect), 0.25f, 0.0f);
			}

		} 

		else {
			if (DebugGUITexture) Destroy (DebugGUITexture);
		}
		yield return null;
	}


	public void RecalculateTextureSize() {
		StartCoroutine(RecalculateRenderTexture());
	}

	private IEnumerator RecalculateRenderTexture() {

		yield return new WaitForEndOfFrame();

		Render_Texture_Camera.GetComponent<Camera> ().targetTexture = null;
		CameraOutputTexture.Release();
		CameraOutputTexture = null;
		if (DebugGUITexture) Destroy (DebugGUITexture);

		StartCoroutine(StartRenderingToTexture());

	}


	void OnValidate(){
		
		#if UNITY_EDITOR
		if (Application.isPlaying) {
			if 	(GetComponent<RenderTextureCamera>().isActiveAndEnabled) StartCoroutine(ShowTextureOnGUI());
		}
		#endif
		
	}
	

    public void MakeScreen() {

        if (screensPath == null) {

		#if UNITY_ANDROID && !UNITY_EDITOR
			screensPath = "/sdcard/DCIM/RegionCapture";

		#elif UNITY_IPHONE && !UNITY_EDITOR
			screensPath = Application.persistentDataPath;

		#else
            screensPath = Application.dataPath + "/Screens";

		#endif
            System.IO.Directory.CreateDirectory(screensPath);
        }

        StartCoroutine(TakeScreen());
    }

    private IEnumerator TakeScreen() {

        yield return new WaitForEndOfFrame();

        Texture2D FrameTexture = new Texture2D(CameraOutputTexture.width, CameraOutputTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = CameraOutputTexture;
        FrameTexture.ReadPixels(new Rect(0, 0, CameraOutputTexture.width, CameraOutputTexture.height), 0, 0);
        RenderTexture.active = null;

        FrameTexture.Apply();
        saveImgToGallery(FrameTexture.EncodeToPNG());

    }

	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern void ImgToAlbum(string str);
	#endif

    private void saveImgToGallery(byte[] img)
    {
    	string fileName = saveImg(img);

		#if UNITY_IPHONE && !UNITY_EDITOR 
		ImgToAlbum(fileName);
		#endif
    }

    private string saveImg(byte[] imgPng)
    {
        string fileName = screensPath + "/screen_" + System.DateTime.Now.ToString("dd_MM_HH_mm_ss") + ".png";

        Debug.Log("write to " + fileName);

        System.IO.File.WriteAllBytes(fileName, imgPng);
        return fileName;
    }
}