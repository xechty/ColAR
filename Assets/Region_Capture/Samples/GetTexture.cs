using UnityEngine;
using System.Collections;

public class GetTexture : MonoBehaviour {

	private Region_Capture monobehaviour;
	private Camera RenderTextureCamera;
	[Space(20)]
	public GameObject Region_Capture;
	[Space(20)]
	public bool FreezeEnable = false;

	void Start () {
	
	RenderTextureCamera = Region_Capture.GetComponentInChildren<Camera>();

		monobehaviour = Region_Capture.GetComponent<Region_Capture> ();

		if (FreezeEnable) {
			monobehaviour.CheckMarkerPosition = true;
		}

	StartCoroutine(WaitForTexture());

	}

	private IEnumerator WaitForTexture() {

		yield return new WaitForEndOfFrame ();

		if (RenderTextureCamera.targetTexture) {
			GetComponent<Renderer> ().material.SetTexture ("_MainTex", RenderTextureCamera.targetTexture);
		}
		else StartCoroutine(WaitForTexture());

	}



	void LateUpdate () {

		if (FreezeEnable && monobehaviour.MarkerIsOUT)
			RenderTextureCamera.enabled = false;

		else RenderTextureCamera.enabled = true;
		}
}