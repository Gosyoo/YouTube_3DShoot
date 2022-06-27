using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <summary>
    /// ������飨��һ��������Ԫ��������ң�
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    /// <param name="array">��������</param>
    /// <param name="seed">���������(�ı����ʹ����仯��ͬ)</param>
    /// <returns></returns>
    public static T[] ShuffleArray<T>(T[] array , int seed = 10) {
        //�����
        System.Random random = new System.Random(seed);
        for(int i = 0; i< array.Length - 1; i++) {
            int randomIndex = random.Next(i, array.Length); //���ѡ��һ������
            T randomItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = randomItem;  //���� i �� randomIndex Ԫ�ص�ֵ
        }

        return array;
    }

}
