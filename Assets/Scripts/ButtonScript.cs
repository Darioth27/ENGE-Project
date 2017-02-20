using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	private bool selected;
	private bool isTurn;
	private int timer;
	private Transform button;
	private Vector2 scale;
	private SpriteRenderer image;
	private Color defaultColor;

	void Awake () {

		button = gameObject.transform;
		scale = new Vector2(button.localScale.x + 0.06f, button.localScale.y + 0.06f);
		image = gameObject.GetComponent <SpriteRenderer>();
		defaultColor = new Color(image.color.r, image.color.g, image.color.b);
		selected = false;
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (selected)
		{
			if (timer >= 0 && timer < 25)
			{
				timer++;
				image.color = new Color(image.color.r - 0.06f, image.color.g - 0.03f, image.color.b);
			}
			else if (timer < 50)
			{
				timer++;
				image.color = new Color(image.color.r + 0.06f, image.color.g + 0.03f, image.color.b);
			}
			else
			{
				timer = 0;
				image.color = defaultColor;
			}
		}
	}

	public void setSelected (bool b)
	{
		if (b == false)
		{
			button.localScale = new Vector2 (scale.x - 0.06f, scale.y - 0.06f);
			image.color = defaultColor;
			timer = 0;
		}
		else
		{
			button.localScale = scale;
		}
		selected = b;
	}

	public bool Selected
	{
		get { return selected; }
	}

	public void setIsTurn(bool b)
	{
		if (b == false)
		{
			image.enabled = false;
		}
		else
		{
			image.enabled = true;
		}
	}

	public bool IsTurn
	{
		get { return isTurn; }
	}
}
