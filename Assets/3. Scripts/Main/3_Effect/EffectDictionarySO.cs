using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum EffectEvent
{
    Text
}







[CreateAssetMenu(fileName = "EffectDictionary", menuName = "SO/Dictionary/Effect", order = int.MaxValue) ]
public class EffectDictionarySO : ScriptableObject 
{
    [SerializeField] List<GameObject> list = new(); 


    public SerializableDictionary<EffectEvent,PoolData> totalPoolData = new();



    
    // 유니티 에디터에서 값이 변경될 때마다 호출되는 메서드
    private void OnValidate()
    {
        // 딕셔너리와 리스트를 동기화
        Sync();
    }

    // 딕셔너리를 리스트와 동기화하는 메서드
    private void Sync()
    {
        // 리스트에서 null인 값이 없을 때, 
        if (list.Any(x=>x==null ))
        {
            return;
        }


        list = list.OrderBy(x=> x.GetComponent<Effect>().effectEvent ).ToList();    // id로 오름차순
        
        // totalData.Clear();
        totalPoolData.Clear();

        // 사전에 리스트의 데이터 등록 
        foreach (GameObject effectObject in list)
        {
            Debug.Log("등록쓰");
            Effect effect = effectObject.GetComponent<Effect>();
            EffectEvent ee = effect.effectEvent;

            // 풀데이터 
            if(!totalPoolData.ContainsKey(ee ))
            {
                PoolData epd = new();
                epd._name = $"{ee}";
                epd._component = effect;
                
                totalPoolData[ee] = epd;
            }
        }
    }
}
