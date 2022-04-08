using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(GunControl))]
public class Player : LivingEntity
{

    public float Speed = 5;
    PlayerControl playerControl;
    Camera myCamera;
    GunControl gunControl;

    protected override void Start()
    {
        base.Start();
        playerControl = GetComponent<PlayerControl>();
        myCamera = Camera.main; //��ȡ�����
        gunControl = GetComponent<GunControl>();
    }

    void Update()
    {
        //�ƶ�����
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * Speed;
        playerControl.Move(moveVelocity);

        //ע������
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);  //�����λ�������λ�÷���һ������
        Plane ground = new Plane(Vector3.up, Vector3.zero);  //ͨ�����ߺ�һ���㴴��һ����׼ƽ��
        float rayDistance;
        if (ground.Raycast(ray, out rayDistance))  //���ƽ���Ƿ��������в�����һ��float���͵Ĳ�����
        {
            Vector3 point = ray.GetPoint(rayDistance); //��ȡ���е������
            playerControl.lookAt(point); //��ɫע�Ӹ�λ�÷���
            //Debug.DrawLine(ray.origin, point, Color.red);
        }

        //�������
        if (Input.GetMouseButton(0))
        {
            gunControl.Shoot();
        }
    }
}
