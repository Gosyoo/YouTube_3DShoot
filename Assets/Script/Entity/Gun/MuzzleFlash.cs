using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {
    public GameObject flashObject; //��˸����
    public Sprite[] sprites; //����ͼ����
    public SpriteRenderer[] spriteRenderers; //����ͼ��Ⱦ��

    [Header("������˸����ʱ��")]
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
