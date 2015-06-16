using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyTransformation : MonoBehaviour {

	public Enemy Enemy;
	public EnemyTransformationState[] States;

	// public Mesh NormalMesh1, NormalMesh2, NormalMesh3, PerfectMesh1, PerfectMesh2;
	// public Material NormalMaterial, NormalMaterial2, NormalMaterial3, PerfectMaterial1, PerfectMaterial2;

	public void Awake()
	{
		if(Enemy != null)
		{
			Enemy = GetComponent<Enemy>();
		}
	}

	private IEnumerator TransformToCellPerfect()
	{
		Enemy.Attacking = true;
		Enemy.CurrentSkill = SkillType.Transformation;
		Enemy.CanAttack = false;
		Enemy.CanMove = false;
		
		Enemy.CharAnimator.SetBool("change_start", true);
		GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/Cell/trans"));
		Camera.main.transform.DOShakeRotation(3f, 10, 10);
		
		yield return new WaitForSeconds(1f);
		var shieldEffect = Resources.Load<GameObject>("Prefabs/ShieldEffectSubtleGold");
		var shieldEffectObj = Instantiate(shieldEffect, this.transform.position + Vector3.down, Quaternion.identity) as GameObject;
		
		yield return new WaitForSeconds(2f);
		if(Enemy.AuraCell != null)
		{
			Enemy.AuraCell.SetActive(true);
		}else
		{
			Debug.Log ("Impossible de récupérer l'objet AuraCell.");
		}
		GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/aura"));
		
		yield return new WaitForSeconds(3f);
		shieldEffect = Resources.Load<GameObject>("Prefabs/CellTransformShield");
		var shieldEffectObj2 = Instantiate(shieldEffect, this.transform.position, Quaternion.identity);
		//audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/pulseLoop5"));
		
		yield return new WaitForSeconds(4f);
		
		Enemy.CharAnimator.SetBool("change_start", false);
		GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/Cell/scream"));

		Destroy(shieldEffectObj);
		Destroy(shieldEffectObj2);

		if(Enemy.AuraCell != null)
		{
			Enemy.AuraCell.SetActive(false);
		}else
		{
			Debug.Log ("Impossible de récupérer l'objet AuraCell.");
		}
		
		//HeadMeshRenderer.sharedMesh = SsjHeadMesh;
		//HeadMeshRenderer.material = SS1Material;

		Enemy.Attacking = false;
		Enemy.RandomAction();
	}

}
