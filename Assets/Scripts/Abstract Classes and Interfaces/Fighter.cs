using UnityEngine;
using System.Collections;

public abstract class Fighter: MonoBehaviour {

	//Fields
	private int slot;
	//private Fighter currentTarget;
	private bool isSelected;
	private bool hasAttacked;
	private int nextAttack;
	private CharacterState state;
	protected Vector3 parentPosition;	//The initial spawning point position

	//Unity Scene Components
	public AudioClip[] sounds;
	protected AudioSource soundPlayer;
	protected Animator ani;
	protected CameraControl camControl;
	public GameObject damageText;
	

	//Methods
	public virtual void Awake()
	{
		soundPlayer = gameObject.GetComponent<AudioSource>();
		ani = gameObject.GetComponent<Animator>();
		camControl = GetComponent<CameraControl> ();

		setState (CharacterState.IDLE);
		hasAttacked = false;
		isSelected = false;
	}

	//Getters and setters for State
	public void setState(CharacterState s)
	{
		state = s;
	}
	
	public CharacterState State
	{
		get { return state; }
	}

	/*public void setTarget(Fighter target)
	{
		currentTarget = target;
	}

	public Fighter Target
	{
		get {return currentTarget; }
	}*/

	public void setNextAction(int next)
	{
		nextAttack = next;
	}

	public int NextAttack
	{
		get { return nextAttack; }
	}

	public bool HasAttacked
	{
		get { return hasAttacked; }
		set { hasAttacked = value;}
	}

	public abstract void PerformAction();
	public abstract void Flinch(int damage);
	public abstract void setSlot();
	public abstract void UpdateStats();
	
	public int Slot
	{
		get { return slot; }
		set { slot = value; }
	}
	
	public bool Selected
	{
		get { return isSelected; }
		set { isSelected = value; }
	}

	public bool isDown()
	{
		return state == CharacterState.DOWN;
	}

	public void playSound(int index)
	{
		soundPlayer.clip = sounds[index];
		soundPlayer.Play();
	}

	public void setSpawnPosition()
	{
		setSlot ();
		parentPosition = new Vector3 (transform.parent.position.x, 
		                              transform.parent.position.y, 
		                              transform.parent.position.z);
	}
}

public enum CharacterState {
	IDLE,
	TAKEDAMAGE,
	ATTACK, 
	DOWN
}