using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {
    //Unity�༭���࣬�������ʱֱ�ӿ��ӻ�  �ڲ����е�����¿���ֱ�ӵ��ýű�

    //����Gui
    public override void OnInspectorGUI() {

        MapGenerator map = target as MapGenerator;

        //������ֵ�ı�ʱˢ��
        if (DrawDefaultInspector()) {
            map.MapGenerate();
        }

        //�ֶ������ťˢ��
        if (GUILayout.Button("Generate Map")) {
            map.MapGenerate();
        }
    }

}
