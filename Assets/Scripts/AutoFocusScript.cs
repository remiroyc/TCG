using System;
using System.Linq;
using UnityEngine;

public class AutoFocusScript : MonoBehaviour
{

    public MyCharacterController CharController;
    public float MaxFocusDistance = 5f;
    public LayerMask EnemyLayer;
    public AutoFocusCamera AutoFocusCamera;
    public float CloseCombatDistance = 1.5f;

    private bool _autoLock = true;

    private Transform[] _enemiesAroundMe;

    private bool _autoFocus = false;
    public bool AutoFocus
    {
        get { return _autoFocus; }
        set
        {
            ManageOutline(CharController.FocusEnemy, value);
            CharacterSight.Instance.SetVisible(!value);
            if (value)
            {
                CharController.OrbitCameraScript.enabled = false;
                AutoFocusCamera.enabled = true;
            }
            else
            {
                AutoFocusCamera.enabled = false;
                CharController.OrbitCameraScript.InitCamera();
                CharController.OrbitCameraScript.enabled = true;
                CharController.FocusEnemy = null;
            }
            _autoFocus = value;

        }
    }

    private void Update()
    {
        CharController.CharAnimator.SetBool("focus", (CharController.FocusEnemy != null));

        if (CharController.FocusEnemy == null)
        {
            if (AutoFocus)
            {
                AutoFocus = false;
            }
            else if (_autoLock)
            {
                // La première fois on détecte l'ennemi automatique
                DetectEnemies();
                if (CharController.FocusEnemy != null)
                {
                    AutoFocus = true;
                }
            }
            else
            {
                // Sinon on sélectionne l'ennemi en cliquant dessus
                TestAutoFocusActivationWithClick();
            }
        }
        else
        {
            if (AutoFocus && AutoFocusCamera.enabled)
            {
                TestAutoFocusDesactivationWithClick();
            }
            ManageAutoFocus();
        }
    }

    private void TestAutoFocusActivationWithClick()
    {
        if (!AutoFocus)
        {

            bool enemyClicked = false;
            var hit = new RaycastHit();

#if UNITY_EDITOR || UNITY_EDITOR_WIN
            if (Input.GetMouseButtonUp(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                enemyClicked = Physics.Raycast(ray, out hit, MaxFocusDistance, EnemyLayer);
            }
#else
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                    enemyClicked = Physics.Raycast(ray, out hit, MaxFocusDistance, EnemyLayer);
                }
            }
#endif

            if (enemyClicked)
            {
                CharController.FocusEnemy = hit.transform;
                AutoFocus = true;
            }
        }
    }

    private void TestAutoFocusDesactivationWithClick()
    {
        bool enemyClicked = false;
        var hit = new RaycastHit();

#if UNITY_EDITOR || UNITY_EDITOR_WIN
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            enemyClicked = Physics.Raycast(ray, out hit, MaxFocusDistance, EnemyLayer);
        }
#else
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(0);
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                    enemyClicked = Physics.Raycast(ray, out hit, MaxFocusDistance, EnemyLayer);
                }
#endif
        if (enemyClicked && hit.transform == CharController.FocusEnemy)
        {
            AutoFocus = false;
            _autoLock = false;
        }
    }

    private void ManageAutoFocus()
    {
        if (AutoFocus)
        {
            var distance = Vector3.Distance(transform.position, CharController.FocusEnemy.transform.position);
            CharController.CloseCombat = (distance <= CloseCombatDistance);

            if (distance > (MaxFocusDistance + 1) && !CharController.Attacking && CharController.CanMove)
            {
                // Si l'enemie est trop loin et que l'on est pas en train d'attaquer on annule le ciblage automatique.
                AutoFocus = false;
                CharController.FocusEnemy = null;
            }
            else
            {
                // Si on se trouve à cette distance on peut utiliser les techniques corp à corp
                var relativePos = CharController.FocusEnemy.transform.position - transform.position;
                var lookRot = Quaternion.LookRotation(relativePos);

                if (CharController.Grounded)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRot.eulerAngles.y, transform.rotation.eulerAngles.z);
                }
                else
                {
                    transform.rotation = transform.rotation = lookRot;
                }


                float rotation = CharController.InputMovement.x * CharController.RotationSpeed;
                if (Math.Abs(rotation) > float.Epsilon && CharController.CanMove && !CharController.Attacking)
                {
                    float dist = rotation * Time.deltaTime * CharController.RotationSpeed;
                    // distance qu'on souhaite parcourir.
                    float angle = -1 * Mathf.Atan(dist / (distance));
                    transform.RotateAround(CharController.FocusEnemy.transform.position, Vector3.up, angle);
                }
            }
        }
    }

    public void ManageOutline(Transform obj, bool activated)
    {
        if (obj != null)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var childRenderer in renderers)
            {
                if (activated)
                {
                    childRenderer.material.SetColor("_OutlineColor", Color.red);
                    //childRenderer.material.SetFloat("_Outline", 0.01f);
                }
                else
                {
                    childRenderer.material.SetColor("_OutlineColor", Color.black);
                    //childRenderer.material.SetFloat("_Outline", 0.005f);
                }
            }
        }
    }

    private void DetectEnemies()
    {
        //if (!CharController.TutorialMode)
        //{
        var enemies = Physics.OverlapSphere(transform.position, MaxFocusDistance - 1, EnemyLayer);
        _enemiesAroundMe = enemies.Select(e => e.transform).ToArray();
        if (_enemiesAroundMe.Length > 0)
        {

            // _enemiesAroundMe[0].transform;
            var newEnemy = (from enemy in _enemiesAroundMe
                            let dist = (transform.position - enemy.position).sqrMagnitude
                            let enemyScript = enemy.GetComponent<Enemy>()
                            where !enemyScript.IsDied && !enemyScript.KoManager.Ko
                            orderby dist
                            select enemy).FirstOrDefault();

            if (CharController.FocusEnemy != newEnemy)
            {
                if (CharController.FocusEnemy != null)
                {
                    ManageOutline(CharController.FocusEnemy, false);
                }

                ManageOutline(newEnemy, true);
                CharController.FocusEnemy = newEnemy;
            }
        }
        else
        {
            if (CharController.FocusEnemy != null)
            {
                ManageOutline(CharController.FocusEnemy, false);
                CharController.FocusEnemy = null;
            }
        }

        //}

    }
}