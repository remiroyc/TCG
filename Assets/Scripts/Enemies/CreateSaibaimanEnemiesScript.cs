using UnityEngine;

public class CreateSaibaimanEnemiesScript : MonoBehaviour
{

    public GameObject SaibaimanPrefab;
    private Transform _myPlayer;

    void Start()
    {
        _myPlayer = GameObject.Find("MyPlayer").transform;
        if (_myPlayer != null)
        {
            InvokeRepeating("GenerateEnemies", 3, 1);
        }
    }


    public void GenerateEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            var position = new Vector3(Random.Range(168, 225), 0, Random.Range(150, 180));
            var enemy = Instantiate(SaibaimanPrefab, position, Quaternion.identity) as GameObject;
            if (enemy != null)
            {
                enemy.GetComponent<SaibaimanScript>().Target = _myPlayer;
            }
        }
    }

}
