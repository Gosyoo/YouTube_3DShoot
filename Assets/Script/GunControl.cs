using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    public Gun startingEquip; //持有的武器
    public Transform weaponHold; //武器位置
    Gun equipGun; //装备的武器

    void Start()
    {
        if (startingEquip != null)
        {
            EquipToGun(startingEquip);
        }
    }


    public void EquipToGun(Gun gun) //装备枪械
    {
        if (equipGun != null)
        {
            Destroy(equipGun.gameObject);
        }
        equipGun = Instantiate<Gun>(gun, weaponHold.position, weaponHold.rotation);
        equipGun.transform.parent = weaponHold;
    }

    //射击
    public void Shoot()
    {   //如果有枪，就射击
        if(equipGun != null)
        {
            equipGun.Shoot();
        }
    }
}
