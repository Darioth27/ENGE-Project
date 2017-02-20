using UnityEngine;
using System.Collections;

public class CharacterTwo : Fighter {

	//private bool isTurn;
	//private bool hasAttacked;
	//private int nextAttack;
	//private CharacterState state;
	private int timer;
	private int multiplier;
	
	//private Animator ani;
	public GameObject pentacle;
	//public AudioClip[] attackSounds;
	//private AudioSource soundPlayer;
	//private CameraControl camControl;
	
	private ParticleSystem fireEffect;
	private ParticleSystem healEffect;
	//public GameObject damageText;
	public Enemy currentTarget;
	
	//private Vector3 parentPosition;
	private bool goToTarget;

	public override void Awake()
	{
		base.Awake ();
		fireEffect = GameObject.FindGameObjectWithTag("ExplodeFX").GetComponent<ParticleSystem>();
		healEffect = GameObject.FindGameObjectWithTag("HealFX").GetComponent<ParticleSystem>();
		//ani = GetComponent<Animator> ();
		//soundPlayer = GetComponent<AudioSource> ();
		//camControl = GetComponent<CameraControl> ();
		//setSlot ();
		//parentPosition = new Vector3(-2.2f, -1.2f, 0);
		Slot = data ().Slot;
		setSpawnPosition ();
		//state = CharacterState.IDLE;
		//setHasAttacked (false);
		goToTarget = false;
		UpdateStats ();
	}
	
	void Update()
	{
		moveTowardsOffset ();
	}
	
	public override void UpdateStats()
	{
		data ().updateStats ();
	}

	public void setTarget(Enemy target)
	{
		currentTarget = target;
	}
	
	public Enemy Target
	{
		get {return currentTarget; }
	}
	
	public void dealDamage()
	{
		int damage = (int)((data ().Attack.AdjustedValue() 
		                    - Target.data().Defense.AdjustedValue ()) * Random.Range(0.8f, 1.2f));
		if (damage <= 0)
		{
			damage = 1;
		}
		Target.Flinch (damage);
	}
	
	public void dealMagicDamage()
	{
		int damage = (int)((data ().MagicAttack.AdjustedValue() 
		                    - Target.data ().MagicDefense.AdjustedValue ()) * Random.Range(1.2f, 1.6f));
		if (damage <= 0)
		{
			damage = 1;
		}
		Target.Flinch (damage);
	}
	
	public void recieveDamage(int damage)
	{
		data ().takeDamage (damage);
		GameObject text = Instantiate (damageText, transform.position + new Vector3 (0.1f, 0, 0), 
		                               Quaternion.Euler (new Vector3 (0, 0, 0))) as GameObject;
		text.GetComponent<DamageDisplay>().setAmount (damage);
		if (GameManager.Instance.P1Data.getCurrentHealth () == 0)
		{
			setState (CharacterState.DOWN);
			ani.SetBool ("isDown", true);
		}
	}
	
	public void recoverHealth()
	{
		int amount = (int)(data ().MagicAttack.AdjustedValue() * Random.Range(1.5f, 1.7f));
		GameManager.Instance.P1Data.restoreHealth (amount);
	}
	
	public override void PerformAction()
	{
		if (NextAttack == 1)
		{
			Attack ();
		}
		else if (NextAttack == 2)
		{
			CastFire ();
		}
		else if (NextAttack == 3)
		{
			Heal ();
		}
	}
	
	public void Attack()
	{
		setState (CharacterState.ATTACK);
		ani.SetTrigger ("Attack");
		HasAttacked = true;
	}
	
	public void CastFire()
	{
		setState (CharacterState.ATTACK);
		ani.SetTrigger ("CastFire");
		HasAttacked = true;
		data ().consumeMana (8);
	}
	
	public void Heal()
	{
		setState (CharacterState.ATTACK);
		//GameManager.Instance.P1Data.consumeMana (10);
		HasAttacked = true;
		ani.SetTrigger("UseItem");
	}
	
	public override void Flinch(int damageAmount)
	{
		ani.SetTrigger ("Flinch");
		recieveDamage (damageAmount);
	}
	
	public override void setSlot()
	{
		if (data ().Slot == 1)
		{
			transform.parent = GameObject.Find ("PlayerSpawn1").transform;
			transform.localPosition = parentPosition;
		}
		if (data ().Slot == 2)
		{
			transform.parent = GameObject.Find ("PlayerSpawn2").transform;
			transform.localPosition = parentPosition;
		}
	}
	
	public Vector3 getTargetOffset()
	{
		if (Target.Slot == 1)
		{
			return new Vector3 (0.1f, 0.2f, 0.1f);
		}
		else if (Target.Slot == 2)
		{
			return new Vector3 (0.7f, -0.2f, -0.9f);
		}
		else if (Target.Slot == 3)
		{
			return new Vector3 (0.5f, 0.7f, 1.1f);
		}
		else
		{
			return new Vector3 (0, 0, 0);
		}
	}
	
	public float getMoveSpeed(Vector3 offset, int frames)
	{
		return (Mathf.Sqrt((offset.x * offset.x) + (offset.y * offset.y))/frames);
	}
	
	public void moveTowardsOffset()
	{
		if (goToTarget && State == CharacterState.ATTACK)
		{
			transform.parent.position = Vector3.MoveTowards (
				transform.parent.position, parentPosition + getTargetOffset(), getMoveSpeed (getTargetOffset(), 20));
		}
		else if (!goToTarget && transform.parent.position != parentPosition)
		{
			transform.parent.position = Vector3.MoveTowards (
				transform.parent.position, parentPosition, getMoveSpeed (getTargetOffset(), 20));
		}
	}
	
	public void toggleGoToTarget()
	{
		goToTarget = !goToTarget;
	}
	
	public void createPentacle()
	{
		GameObject circle = Instantiate (pentacle, transform.position + new Vector3 (-0.1f, -0.35f, 0), Quaternion.Euler (new Vector3 (0, 0, 0))) as GameObject;
	}
	
	public void playFireEffect()
	{
		fireEffect.Play ();
		fireEffect.transform.position = Target.transform.position + new Vector3 (0, 0, -8);
		fireEffect.GetComponent<SetParticleSortingLayer>().playSoundFX ();
	}
	
	public void playHealEffect()
	{
		healEffect.Play ();
		healEffect.transform.position = transform.position + new Vector3 (-0.05f, -0.45f, -5);
	}
	
	public CharacterData data()
	{
		return GameManager.Instance.P2Data;
	}
	
	
	
	public void CameraZoom()
	{
		camControl.cameraZoom(1.8f, 25);
		camControl.moveTowardsPoint (new Vector3(Target.transform.position.x - 1, 
		                                         Target.transform.position.y + 0.5f, -10), 20);
	}
	
	public void CameraOut(int i)
	{
		camControl.resetCamera (i);
	}
	
	public void CameraZoomCustom(float f)
	{
		camControl.cameraZoom(f, 10);
		camControl.moveTowardsPoint (new Vector3(transform.position.x + 0.85f, 
		                                         transform.position.y + 0.6f, -10), 10);
	}
}
