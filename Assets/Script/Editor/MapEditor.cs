using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {
    //Unity编辑器类，方便调试时直接可视化  在不运行的情况下可以直接调用脚本

    //重新Gui
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;
        map.MapGenerate();
    }

}
