using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

	public float startingHealth;
	protected float health;
	protected bool dead;

	protected virtual void Start(){
		health = startingHealth;
	} 

	public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
		TakeDamage(damage);
	}

	public virtual void TakeDamage(float damage){
		health -= damage;

		if (health <= 0 && !dead){
			Die();
		}
	}

	public virtual void Die(){
		dead = true;
	}
}
