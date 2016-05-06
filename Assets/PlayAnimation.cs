using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {

    Animator animator;

    // Use this for initialization
    void Start () {

        animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.touchCount > 0)
        {
            animator.SetTrigger("TouchTrigger");
        }
	
	}
}
