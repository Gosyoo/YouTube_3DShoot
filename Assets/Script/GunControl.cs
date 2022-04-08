using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    public Gun startingEquip; //���е�����
    public Transform weaponHold; //����λ��
    Gun equipGun; //װ��������

    void Start()
    {
        if (startingEquip != null)
        {
            EquipToGun(startingEquip);
        }
    }


    public void EquipToGun(Gun gun) //װ��ǹе
    {
        if (equipGun != null)
        {
            Destroy(equipGun.gameObject);
        }
        equipGun = Instantiate<Gun>(gun, weaponHold.position, weaponHold.rotation);
        equipGun.transform.parent = weaponHold;
    }

    //���
    public void Shoot()
    {   //�����ǹ�������
        if(equipGun != null)
        {
            equipGun.Shoot();
        }
    }
}
