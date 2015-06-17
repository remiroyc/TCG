using UnityEngine;

public class TakeDamageModel
{
    private bool _canDodge = true;
    public bool CanDodge
    {
        get { return _canDodge; }
        set { _canDodge = value; }
    }

    public Transform Emitter { get; set; }
    public string SkillName { get; set; }

    public NBAttack NbAttack { get; set; }
    public AttackTypes AttackTypes { get; set; }
    public bool RightAttack { get; set; }
    public bool Default { get; set; }

    public float AttackMultiplicator { get; set; }

    public TakeDamageModel()
    {
        Default = true;
    }
}

