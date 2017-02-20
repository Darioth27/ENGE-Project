using UnityEngine;
using System.Collections;

public class Character: Fighter {

	public int CharacterID;
	
	public GameObject pentacle;
	public GameObject lightning;

	private ParticleSystem fireEffect;
	private ParticleSystem healEffect;

	public Enemy currentTarget;
	private bool goToTarget;

	public override void Awake()
	{
		base.Awake ();
		fireEffect = GameObject.FindGameObjectWithTag("ExplodeFX").GetComponent<ParticleSystem>();
		healEffect = GameObject.FindGameObjectWithTag("HealFX").GetComponent<ParticleSystem>();

		Slot = data ().Slot;
		setSpawnPosition();

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
		if (CharacterID == 1)
		{
			int damage = (int)((data ().Attack.AdjustedValue() 
			                    - Target.data().Defense.AdjustedValue ()) * Random.Range(0.8f, 1.2f));
			if (damage <= 0)
			{
				damage = 1;
			}
			Target.Flinch (damage);
		}
		else
		{
			int damage = (int)((data ().MagicAttack.AdjustedValue() 
			                    - Target.data().Defense.AdjustedValue ()) * Random.Range(0.6f, 1.0f));
			if (damage <= 0)
			{
				damage = 1;
			}
			Target.Flinch (damage);
		}
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
		if (data ().getCurrentHealth () == 0)
		{
			setState (CharacterState.DOWN);
			ani.SetBool ("isDown", true);
		}
	}

	public void recoverHealth()
	{
		int amount = (int)(data ().MagicAttack.AdjustedValue() * Random.Range(1.5f, 1.7f));
		data ().restoreHealth (amount);
	}

	public override void PerformAction()
	{
		if (NextAttack == 1 && !Target.isDown ())
		{
			Attack ();
		}
		else if (NextAttack == 2 && !Target.isDown ())
		{
			CastFire ();
		}
		else if (NextAttack == 3)
		{
			Heal ();
		}
		else
		{
			HasAttacked = true;
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
		Instantiate (pentacle, transform.position + new Vector3 (-0.1f, -0.35f, 0), 
		                                 Quaternion.Euler (new Vector3 (0, 0, 0)));
	}

	public void createLightning()
	{
		Instantiate (lightning, Target.transform.position 
		                                 + new Vector3 (0, -Target.transform.GetComponent<Renderer>().bounds.size.y / 6, 0), 
		                                 Quaternion.Euler (new Vector3 (0, 0, 0)));
	}

	public void playFireEffect()
	{
		fireEffect.Play ();
		fireEffect.transform.position = Target.transform.position + new Vector3 (0, 0, -5);
		fireEffect.GetComponent<SetParticleSortingLayer>().playSoundFX ();
	}

	public void playHealEffect()
	{
		healEffect.Play ();
		healEffect.transform.position = transform.position + new Vector3 (-0.05f, -0.45f, -5);
	}

	public CharacterData data()
	{
		if (CharacterID == 1)
		{
			return GameManager.Instance.P1Data;
		}
		else
		{
			return GameManager.Instance.P2Data;
		}
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

	public void CameraZoomCustomSpeed(int s)
	{
		camControl.cameraZoom(2.2f, s);
		camControl.moveTowardsPoint (new Vector3(transform.position.x + 1.55f, 
		                                         transform.position.y + 0.6f, -10), 10);
	}

}
