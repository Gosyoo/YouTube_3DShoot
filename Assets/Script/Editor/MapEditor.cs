using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {
    //Unity�༭���࣬�������ʱֱ�ӿ��ӻ�  �ڲ����е�����¿���ֱ�ӵ��ýű�

    //����Gui
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;
        map.MapGenerate();
    }

}
