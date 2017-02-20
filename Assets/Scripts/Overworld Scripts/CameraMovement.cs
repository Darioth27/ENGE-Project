using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	void Update () 
	{
		transform.position = GameObject.Find ("Character").transform.position + new Vector3 (0, 0, -1);
	}
}
