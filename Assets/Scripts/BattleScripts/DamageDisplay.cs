using UnityEngine;
using System.Collections;

public class DamageDisplay : MonoBehaviour {
	
	private Vector3 offset;
	private int amount;
	private int originalAmount;
	private int digits;
	private int timer;

	private TextMesh display;

	void Awake() {
		display = GetComponent <TextMesh> ();
		offset = new Vector3 (0, 0.01f, 0);
		timer = 20;
	}
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 1.5f);
		display.text = "" + Random.Range(0, 10);
		display.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {

		if (timer > 5) {
						switch (digits) {
						case 1:
								display.text = "" + Random.Range (0, 10); 
								break;
						case 2:
								display.text = "" + Random.Range (10, 100); 
								break;
						case 3:
								display.text = "" + Random.Range (100, 1000); 
								break;
						case 4: 
								display.text = "" + Random.Range (1000, 10000); 
								break;
						default:
								break;
						}

						timer--;
				} else {
						display.text = "" + amount;
						transform.position = transform.position + offset;
						if (timer > 0) {timer--;}
						if (timer <= 0) {
							display.color = new Color(display.color.r, display.color.g - 0.02f, 
			                          display.color.b - 0.02f, display.color.a - 0.01f);
						}
				}

	}

	public void setAmount (int a)
	{
		amount = a;
		if (amount < 10) {
						digits = 1;
				} else if (amount < 100) {
						digits = 2;
				} else if (amount < 1000) {
						digits = 3;
				} else {
						digits = 4;
				}
	}

	public int getAmount()
	{
		return amount;
	}

	public void setTransform(Transform t)
	{
		transform.position = t.position;
	}

	public void setOffSet(Vector3 v)
	{
		offset = v;
	}
}
