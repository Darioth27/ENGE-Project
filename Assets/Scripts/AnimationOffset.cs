using UnityEngine;
using System.Collections;

public class AnimationOffset : MonoBehaviour {

	private Vector3 offset;

	void Awake()
	{
		offset = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

		//transform.position = transform.position + offset;
	}

	void LateUpdate() 
	{
		transform.localPosition += offset;
	}

	public void setOffset(float x, float y, float z)
	{
		offset = new Vector3 (x, y, z);
	}

	public void setParent(Transform parent)
	{
		transform.parent = parent;
	}

	public void setParent(GameObject obj)
	{
		transform.parent = obj.transform;
	}

}
