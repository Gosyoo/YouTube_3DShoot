using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {
    //Unity编辑器类，方便调试时直接可视化  在不运行的情况下可以直接调用脚本

    //重新Gui
    public override void OnInspectorGUI() {

        MapGenerator map = target as MapGenerator;

        //当界面值改变时刷新
        if (DrawDefaultInspector()) {
            map.MapGenerate();
        }

        //手动点击按钮刷新
        if (GUILayout.Button("Generate Map")) {
            map.MapGenerate();
        }
    }

}
