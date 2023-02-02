using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalColliderForce : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Domino"))
        {
            SoundMG.Instance.PlaySFX(eSoundType.ForceOut, transform.position, DominoManager.Instance.forceOutVolume);
            collision.rigidbody.AddForce(new Vector3(Random.Range(-1, 2), 1, Random.Range(-1, 2)) * 2, ForceMode.Impulse);
        }
    }
}
