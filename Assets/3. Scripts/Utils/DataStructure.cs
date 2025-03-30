using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameUtil
{
        
    /// <summary>
    /// 삽입, 삭제, 랜덤접근이 아주 빠르다. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class FastList<T>
    {
        public int Count => list.Count;

        [SerializeField] private Dictionary<T, int> dic = new (); // 원소와 리스트의 인덱스 매치
        [SerializeField] private List<T> list = new (); // 빠른 랜덤 접근을 위한 리스트
        [SerializeField] private System.Random random = new ();

        //=======================================================================================
        public void Add(T item)
        {
            dic[item] = list.Count;
            list.Add(item);
        }

        /// <summary>
        /// 삭제할 원소를 리스트의 마지막 원소와 swap하여 삭제. (O(1) 보장)
        /// </summary>
        public bool Remove(T  item)
        {
            // 리스트에 없는 경우,
            if (dic.ContainsKey(item) == false)
            {
                return false;
            }
            
            // swap 대상 찾기
            int index = dic[item]; 
            int lastIndex = list.Count - 1;
            var lastItem = list[lastIndex]; 

            // swap
            list[index] = lastItem;
            dic[lastItem] = index;

            // remove
            list.RemoveAt(lastIndex);
            dic.Remove(item);

            return true;
        }

        public T GetRandom()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("원소가 없는데 호출됨!");

            return list[random.Next(list.Count)];
        }

        public List<T> GetTotalItems()
        {
            return list;
        }



    }

}
