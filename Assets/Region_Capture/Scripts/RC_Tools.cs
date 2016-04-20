using UnityEngine;
using System.Collections;
using Vuforia;

public class RC_Tools : MonoBehaviour {

	[Space(20)]
	public bool AutofocusCamera = true;
	public bool HideAndroidToolbar = true;

	void Start () {
	
		#if UNITY_ANDROID
		if (HideAndroidToolbar) {
			DisableSystemUI.Run();
			DisableSystemUI.DisableNavUI();
		}
		#endif

		StartCoroutine(Autofocus());

	}
	
	private IEnumerator Autofocus()
	{
		yield return new WaitForSeconds(1.0f);
		if (AutofocusCamera) CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
	}


	void OnApplicationPause() {
		StartCoroutine(Autofocus());
	}
}
