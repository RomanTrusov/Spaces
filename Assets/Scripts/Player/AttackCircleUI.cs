using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCircleUI : MonoBehaviour
{
    //get UI sample to instantiation
    [SerializeField]
    private DamageIndicator myAttackIndicator;


    public void CreateUIArrow(Transform objtransform)
    {
        //get enemy position once
        myAttackIndicator.damageLocation = objtransform.position;
        myAttackIndicator.enemy = objtransform.gameObject;
        //create UI arrow
        GameObject attackUI = Instantiate(myAttackIndicator.gameObject, myAttackIndicator.transform.position, myAttackIndicator.transform.rotation, myAttackIndicator.transform.parent);
        // activate it
        attackUI.SetActive(true);
    }
}
