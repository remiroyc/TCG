using System.Collections;

public interface IArtificialIntelligenceAttack
{

    void RandomAction();

    void Refocus();

    IEnumerator Attack(float waitTimeBeforeAttack = 0);

    IEnumerator Idle(float time);

    void UpdateAttack();

    void MoveHandToHand();

    void RandomMove();

    void UpdateMoveHandToHand();

    void UpdateRandomMove();

	IEnumerator CastSpecialDistanceAttack();

}
