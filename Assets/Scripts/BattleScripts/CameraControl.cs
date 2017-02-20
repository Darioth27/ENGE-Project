using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public float size = 2.4f;
	private Camera camera;
	private int timerZ;
	private int timerP;
	private float amount;
	private Vector3 point;
	private float speed;
	private int reset;

	void Awake()
	{
		camera = Camera.main;
		reset = -1;
		timerZ = 0;
		timerP = 0;
		amount = 0;
	}

	void Update()
	{
		if (timerZ > 0)
		{
			camera.orthographicSize -= amount;

			timerZ--;
		}
		if (timerP > 0)
		{
			camera.transform.position = Vector3.MoveTowards (camera.transform.position, point, speed);

			timerP--;
		}
		if (reset > 0)
		{
			reset --;
		}
		else if (reset == 0)
		{
			resetCamera ();
			reset = -1;
		}
	}

	public void cameraZoom(float newSize, int frames)
	{
		amount = (camera.orthographicSize - newSize) / frames;
		timerZ = frames;
	}

	public void moveTowardsPoint (Vector3 p, int frames)
	{
		point = p;
		speed = (p - camera.transform.position).magnitude / frames;
		timerP = frames;
	}

	public void resetCamera()
	{
		camera.orthographicSize = size;
		camera.transform.position = new Vector3(0,0,-10);
		amount = 0;
		speed = 0;
	}

	public void resetCamera(int frames)
	{
		amount = (camera.orthographicSize - size) / frames;
		timerZ = frames;
		point = new Vector3(0, 0, -10);
		speed = (point - camera.transform.position).magnitude / frames;
		timerP = frames;
		reset = frames;
	}
}
