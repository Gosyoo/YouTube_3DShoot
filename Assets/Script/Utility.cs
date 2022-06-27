using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <summary>
    /// 随机数组（将一个数组中元素随机打乱）
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="array">泛型数组</param>
    /// <param name="seed">随机数种子(改变可以使随机变化不同)</param>
    /// <returns></returns>
    public static T[] ShuffleArray<T>(T[] array , int seed = 10) {
        //随机器
        System.Random random = new System.Random(seed);
        for(int i = 0; i< array.Length - 1; i++) {
            int randomIndex = random.Next(i, array.Length); //随机选择一个索引
            T randomItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = randomItem;  //交换 i 与 randomIndex 元素的值
        }

        return array;
    }

}
