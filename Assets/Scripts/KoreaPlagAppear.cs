using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoreaPlagAppear : Domino
{
    public Transform flat;
    public Transform koreaPlag;
    public Transform lastDominos;
    public GameObject numFour;

    bool stop = false;
    protected override void FixedUpdate()
    {
        if (GetTop().y <= 0.2 && isUsedSkill == false && stop == false && numFour == null) // 넘어졌다라고 치면 사운드 출력
        {
            SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, this.transform.position);

            flat.gameObject.SetActive(true);
            koreaPlag.gameObject.SetActive(true);
            lastDominos.gameObject.SetActive(true);

            stop = true;
        }
    }
}
