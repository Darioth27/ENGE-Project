using UnityEngine;
using System.Collections;

public class Enemy : Fighter {

	//private int slot;
	//private bool isSelected;
	//private bool hasAttacked;
	//private Animator ani;
	//public AudioClip[] attackSounds;
	//private AudioSource soundPlayer;
	private EnemyData stats;
	private Character currentTarget;

	private ParticleSystem breathAttack;

	//public GameObject damageText;

	public override void Awake()
	{
		//ani = GetComponent<Animator> ();
		//soundPlayer = GetComponent<AudioSource>();
		base.Awake ();
		Slot = 1;
		setSlot ();
		stats = new EnemyData();
		UpdateStats ();

		breathAttack = GameObject.FindGameObjectWithTag("BossFX").GetComponent<ParticleSystem>();
	}

	void Update()
	{
		ani.SetBool ("Selected", Selected);
	}

	public void setTarget(Character target)
	{
		currentTarget = target;
	}

	public Character Target
	{
		get {return currentTarget; }
	}

	public void dealDamage()
	{
		playSound (0);
		int damage = (int)((data().Attack.AdjustedValue () - Target.data ().Defense.AdjustedValue ()) 
		                   * Random.Range(0.8f, 1.2f));

		if (damage <= 0)
		{
			damage = 1;
		}
		Target.Flinch (damage);

	}
	
	public void recieveDamage(int damage)
	{
		data().takeDamage (damage);
		GameObject text = Instantiate (damageText, transform.position + new Vector3 (0.2f, 0, 0), 
		                               Quaternion.Euler (new Vector3 (0, 0, 0))) as GameObject;
		text.GetComponent<DamageDisplay>().setAmount (damage);

		if (stats.CurrentHealth <= 0)
		{
			setState (CharacterState.DOWN);
			ani.SetBool("isDead", true);
		}
	}

	public override void PerformAction()
	{	
		setState (CharacterState.ATTACK);
		ani.SetTrigger ("Attack");
		HasAttacked = true;
	}

	public override void Flinch(int damage)
	{
		ani.SetTrigger ("Flinch");
		recieveDamage (damage);
	}

	public override void setSlot()
	{
		if (Slot == 1)
		{
			transform.parent = GameObject.Find ("EnemySpawn1").transform;
			transform.localPosition = parentPosition;
		}
		if (Slot == 2)
		{
			transform.parent = GameObject.Find ("EnemySpawn2").transform;
			transform.localPosition = parentPosition;
		}
		if (Slot == 3)
		{
			transform.parent = GameObject.Find ("EnemySpawn3").transform;
			transform.localPosition = parentPosition;
		}
	}

	public EnemyData data()
	{
		return stats;
	}

	public override void UpdateStats()
	{
		data ().updateStats ();
	}

	//Temp methods for boss

	public void bossPerformAction()
	{
		setState (CharacterState.ATTACK);
		ani.SetTrigger ("Attack");
		HasAttacked = true;
		breathAttack.Play();
	}

}
