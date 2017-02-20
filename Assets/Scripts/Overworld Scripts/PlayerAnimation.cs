using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {

	public Animator anim;
	public float speed;
	private float diagMultiplier = 0.707106f;
	private Direction direction;		//current direction the character is moving in
	private bool left, right, front, back; //booleans that determine if a directional key is being held back or not
	private bool directionChanged;		//boolean that represents if direction of movement has changed
	private Direction previousDirection;	//represents the direction the character was traveling during the last frontdate

	//Enums that represent the direction the character is currently traveling in
	public enum Direction {
		Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft, Idle
	}

	void Awake () 
	{
		anim = GetComponent<Animator> ();
		direction = Direction.Idle;		//Initializes direction as Idle
		previousDirection = direction;	//sets the previousDirection equal to the direction because there is none yet.
		directionChanged = false;		//Initializes directionChanged
		left = false;					//Initialize directional booleans
		right = false;					//Their purpose is to effectively call "getKey" without actually calling
		front = false;					//"GetKey".  Primarily here for code readability.
		back = false;
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) 
		{
			left = true;					//Sets 'left' to true and left stays true until the key is lifted.
			anim.SetBool ("Idle", false);	//Sets "Idle" to false in the anim to unlock movement.
		} 
		if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.LeftArrow))
		{
			left = false;					//Sets 'left' to false.  Basically says the key is no longer being pressed.
		}
		if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) 
		{
			anim.SetBool ("Idle", false);
			right = true;
		}
		if (Input.GetKeyUp (KeyCode.D)  || Input.GetKeyUp (KeyCode.RightArrow))
		{
			right = false;
		}
		if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			anim.SetBool ("Idle", false);
			front = true;
		}
		if (Input.GetKeyUp (KeyCode.W)|| Input.GetKeyUp (KeyCode.UpArrow)) 
		{
			front = false;
		}
		if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow))
		{
			anim.SetBool ("Idle", false);
			back = true;
		}
		if (Input.GetKeyUp (KeyCode.S) || Input.GetKeyUp (KeyCode.DownArrow)) 
		{
			back = false;
		}

		previousDirection = direction;	//This is used to record the direction of movement last frame.  Useful later.

		//This giant chain of if-statements determines the new current direction.
		if (front)
		{
			if(left)
			{
				direction = Direction.FrontLeft;
			}
			else if(right)
			{
				direction = Direction.FrontRight;
			}
			else if (back)							//This deals with pesky opposite directional presses.
			{
				return;
			}
			else
			{
				direction = Direction.Front;
			}
		}
		else if (right)
		{
			if(front)
			{
				direction = Direction.FrontRight;
			}
			else if (back)
			{
				direction = Direction.BackRight;
			}
			else if (left)
			{
				return;
			}
			else
			{
				direction = Direction.Right;
			}
		}
		else if (left)
		{
			if(front)
			{
				direction = Direction.FrontLeft;
			}
			else if (back)
			{
				direction = Direction.BackLeft;
			}
			else if (right)
			{
				return;
			}
			else
			{
				direction = Direction.Left;
			}
		}
		else if (back)
		{
			if(left)
			{
				direction = Direction.BackLeft;
			}
			else if (right)
			{
				direction = Direction.BackRight;
			}
			else if (front)
			{
				return;
			}
			else
			{
				direction = Direction.Back;
			}
		}
		else
		{
			direction = Direction.Idle;			//Sets direction to "Idle" if no keys are being pressed
		}

		if (previousDirection != direction)		//Determines if the direction has changed since the last update.
		{
			directionChanged = true;
		}
		else
		{
			directionChanged = false;
		}

		//Where Everything is done.
		switch (direction)
		{
		case Direction.Front:								//If (direction == Front)...
			transform.Translate (Vector2.up * speed);		//Move up.
			if (directionChanged)							//If the direction has changed since last update,
			{
				anim.SetTrigger ("Front");					//Trigger the animation once.
			}												//This prevents repeated calling of the animation trigger
			break;
		case Direction.FrontLeft:
			transform.Translate (Vector2.up * speed * diagMultiplier);
			transform.Translate (-Vector2.right * speed * diagMultiplier);
			if (directionChanged)
			{
				anim.SetTrigger ("FrontLeft");					//For the diagonal directions, create new "Triggers" in
			}												//the animator.  Temporarily using front and back.
			break;
		case Direction.Left:
			transform.Translate (-Vector2.right * speed);
			if (directionChanged)
			{
				anim.SetTrigger ("Left");
			}
			break;
		case Direction.BackLeft:
			transform.Translate (-Vector2.up * speed * diagMultiplier);
			transform.Translate (-Vector2.right * speed * diagMultiplier);
			if (directionChanged)
			{
				anim.SetTrigger ("BackLeft");
			}
			break;
		case Direction.Back:
			transform.Translate (-Vector2.up * speed);
			if (directionChanged)
			{
				anim.SetTrigger ("Back");
			}
			break;
		case Direction.BackRight:
			transform.Translate (-Vector2.up * speed * diagMultiplier);
			transform.Translate (Vector2.right * speed * diagMultiplier);
			if (directionChanged)
			{
				anim.SetTrigger ("BackRight");
			}
			break;
		case Direction.Right:
			transform.Translate (Vector2.right * speed);
			if (directionChanged)
			{
				anim.SetTrigger ("Right");
			}
			break;
		case Direction.FrontRight:
			transform.Translate (Vector2.up * speed * diagMultiplier);
			transform.Translate (Vector2.right * speed * diagMultiplier);
			if (directionChanged)
			{
				anim.SetTrigger ("FrontRight");
			}
			break;
		default:
			anim.SetBool ("Idle", true);						//If the character isn't moving, then set Idle to true.
			break;												//Character will stop moving and face the direction
		}														//he stopped in.
	}

	public void stopMovement()
	{
		left = false;
		right = false;
		front = false;	
		back = false;
		anim.SetBool ("Idle", true);
	}
}
