using UnityEngine;

public class Saiyan : Player
{
    public bool DisplayKiBar = false;
    public float MaxKi = 100;
    private float _ki;

    public float Ki
    {
        get { return _ki; }
        set
        {
            if (value <= 0)
            {
                _ki = 0;
            }
            else if (value >= MaxKi)
            {
                _ki = MaxKi;
            }
            else
            {
                _ki = value;
            }
        }
    }

	protected override void Awake()
    {
		base.Awake();
        Ki = MaxKi / 2;
        Life = MaxLife;
    }

}