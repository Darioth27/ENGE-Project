using UnityEngine;
using System.Collections;

public class LightningScript : MonoBehaviour {

	private ParticleSystem lightningEffect;

	void Awake() {
		lightningEffect = GameObject.FindGameObjectWithTag("LightningFX").GetComponent<ParticleSystem>();
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void destroySelf()
	{
		Destroy(gameObject);
	}

	public void playParticles()
	{
		lightningEffect.Play ();
		lightningEffect.transform.position = transform.position;
	}
}
