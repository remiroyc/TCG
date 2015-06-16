using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
		float speed;
		float cameraRotX = 0f;
		public float cameraPitchMax = 45f;
		public Transform target;
		public float targetHeight = 1.7f;
		public float distance = 5.0f;
		public float offsetFromWall = 0.1f;
		public float maxDistance = 20;
		public float minDistance = .6f;
		public float xSpeed = 200.0f;
		public float ySpeed = 200.0f;
		public float targetSpeed = 5.0f;
		public float decalage = 15f; // On place la caméra coté épaule
		public LayerMask mask;
		public Transform aimTarget;
		public int yMinLimit = -80;
		public int yMaxLimit = 80;
		public int zoomRate = 40;
		public float rotationDampening = 3.0f;
		public float zoomDampening = 5.0f;
		public LayerMask collisionLayers = -1;
		public Texture reticle;
		private float xDeg = 0.0f;
		private float yDeg = 0.0f;
		private float currentDistance;
		private float desiredDistance;
		private float correctedDistance;
		private Vector3 originPosition;
		private Quaternion originRotation;
		public float shake_decay;
		public float shake_intensity;
		private bool _init = true;

		void OnGUI ()
		{
				if (Time.time != 0 && Time.timeScale != 0)
						GUI.DrawTexture (new Rect (Screen.width / 2 - (reticle.width * 0.5f), Screen.height / 2 - (reticle.height * 0.5f), reticle.width, reticle.height), reticle);
		}

		void Start ()
		{
				var angles = transform.eulerAngles;
				xDeg = angles.x;
				yDeg = angles.y;

				currentDistance = distance;
				desiredDistance = distance;
				correctedDistance = distance;

				mask = 1 << target.gameObject.layer;
				mask |= 1 << LayerMask.NameToLayer ("Ignore Raycast");
				mask = ~mask;

		}

		void Update ()
		{

				if (shake_intensity > 0) {
						transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
						transform.rotation = new Quaternion (
			            originRotation.x + Random.Range (-shake_intensity, shake_intensity) * .2f,
			            originRotation.y + Random.Range (-shake_intensity, shake_intensity) * .2f,
			            originRotation.z + Random.Range (-shake_intensity, shake_intensity) * .2f,
			            originRotation.w + Random.Range (-shake_intensity, shake_intensity) * .2f);
						shake_intensity -= shake_decay;
				}

		}

		void LateUpdate ()
		{
				// Si il n'y a pas de joueurs définis on ne fait rien
				if (!target) {
						return;
				}

				float targetRotationAngle = target.eulerAngles.y;
				float currentRotationAngle = transform.eulerAngles.y;

				// xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
				xDeg = target.eulerAngles.y;
				yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);

				// On applique la rotation à la caméra
				Quaternion rotation = Quaternion.Euler (yDeg, xDeg - decalage, 0);

				// On calcule la distance désirée (via le scroll)
				desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance);
				desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
				correctedDistance = desiredDistance;

				// On calcule la position de la caméra désirée
				Vector3 vTargetOffset = new Vector3 (0, -targetHeight, 0);
				Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

				// check for collision using the true target's desired registration point as set by user using height
				RaycastHit collisionHit;
				var trueTargetPosition = new Vector3 (target.position.x, target.position.y + targetHeight, target.position.z);

				// Si il y a eu une collision, on corrige la position de la caméra et on calcule la distance corrigée
				bool isCorrected = false;

				if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers.value)) {
						// On calcule la distance entre la position initiale estimée et la position de la collision.
						// Pour cela, on soustrait une distance de sécurité "offset" de l'objet collisionné.
						// Le décalage de sécurité aidera à garder la position de la caméra juste au-dessus de la surface heurtée et ainsi éviter que la caméra ne s'efonce dans l'objet.

						correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
						isCorrected = true;
				}

				// Pour le smoothing, on utilise la fonction Lerp seulement si une distance n'est pas correcte ou si correctedDistance > currentDistance
				currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

				// La fonction Clamp permet de maintenir la valeur dans une certaine limite
				currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);

				// On recalcule la position en se basant sur la nouvelle currentDistance
				position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);


				/*if (_init)
        {*/
				// Create a vector from the camera towards the player.
				Vector3 relPlayerPosition = target.position - transform.position;

				// Create a rotation based on the relative position of the player being the forward vector.
				Quaternion lookAtRotation = Quaternion.LookRotation (relPlayerPosition, Vector3.up);

        
				// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
				// transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, 1f * Time.deltaTime);

				transform.rotation = Quaternion.Euler (target.transform.eulerAngles.x, xDeg - decalage, 0);

				if (shake_intensity > 0) {
						transform.position = position + Random.insideUnitSphere * shake_intensity;
				} else {
						transform.position = position;
				}
				//_init = false;
				// }
        
		}

		private static float ClampAngle (float angle, float min, float max)
		{
				if (angle < -360) {
						angle += 360;
				}
				if (angle > 360) {
						angle -= 360;
				}
				return Mathf.Clamp (angle, min, max);
		}

		public void ShakeGenki ()
		{
				originPosition = transform.position;
				originRotation = transform.rotation;
				shake_intensity = .3f;
				shake_decay = 0.002f;
		}

		public void ShakeFatHit ()
		{
				originPosition = transform.position;
				originRotation = transform.rotation;
				shake_intensity = .3f;
				shake_decay = 0.002f;
		}

		public void ShakeNormalHit ()
		{
				originPosition = transform.position;
				originRotation = transform.rotation;
				shake_intensity = .1f;
				shake_decay = 0.5f;
		}
}
