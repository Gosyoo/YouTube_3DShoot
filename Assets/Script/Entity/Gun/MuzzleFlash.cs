using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {
    public GameObject flashObject; //闪烁对象
    public Sprite[] sprites; //精灵图数组
    public SpriteRenderer[] spriteRenderers; //精灵图渲染器

    [Header("单次闪烁持续时间")]
    public float flashTime;

    void Start() {
        Deactivate();
    }

    public void Activate() {
        flashObject.SetActive(true);

        int randomIndex = Random.Range(0, sprites.Length);
        for (int i = 0; i < spriteRenderers.Length; i++) {
            spriteRenderers[i].sprite = sprites[i];
        }

        Invoke("Deactivate", flashTime);
    }

    public void Deactivate() {
        flashObject.SetActive(false);
    }
}
