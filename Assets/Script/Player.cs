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
        myCamera = Camera.main; //获取主相机
        gunControl = GetComponent<GunControl>();
    }

    void Update()
    {
        //移动输入
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * Speed;
        playerControl.Move(moveVelocity);

        //注视输入
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);  //从相机位置向鼠标位置发射一条射线
        Plane ground = new Plane(Vector3.up, Vector3.zero);  //通过法线和一个点创建一个标准平面
        float rayDistance;
        if (ground.Raycast(ray, out rayDistance))  //检查平面是否被射线命中并返回一个float类型的参数；
        {
            Vector3 point = ray.GetPoint(rayDistance); //获取击中点的坐标
            playerControl.lookAt(point); //角色注视该位置方向
            //Debug.DrawLine(ray.origin, point, Color.red);
        }

        //射击输入
        if (Input.GetMouseButton(0))
        {
            gunControl.Shoot();
        }
    }
}
