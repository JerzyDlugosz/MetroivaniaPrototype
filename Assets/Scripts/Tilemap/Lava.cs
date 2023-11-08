using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BaseEntity comp))
        {
            if(comp.onFire)
            {
                return;
            }
            comp.inLava = true;
            comp.Burning(0.5f, 3);
            return;
        }
        else if(collision.transform.parent.TryGetComponent(out BaseEntity parentComp))
        {
            if(parentComp.TryGetComponent(out Player playerComp))
            {
                if(playerComp.playerData.FireSpirit)
                {
                    return;
                }
            }
            if (parentComp.onFire)
            {
                return;
            }
            parentComp.inLava = true;
            parentComp.Burning(0.5f, 3);
            return;
        }
        collision.GetComponentInParent<BaseEntity>().inLava = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BaseEntity comp))
        {
            comp.inLava = false;
            return;
        }
        else if (collision.transform.parent.TryGetComponent(out BaseEntity parentComp))
        {
            parentComp.inLava = false;
            return;
        }
        collision.GetComponentInParent<BaseEntity>().inLava = false;
    }
}
