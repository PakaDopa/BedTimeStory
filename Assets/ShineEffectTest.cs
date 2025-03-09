using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShineEffectTest : MonoBehaviour
{
    Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Image>().material;
    }

    public void EnableEffect()
    {
        material.SetFloat("_ShineRoate", 0.0f);
        //±½±â
        material.SetFloat("_ShineWidth", 0.18f);
        material.SetFloat("_ShineGlow", 20.0f);
        material.SetFloat("_ShineLocation", 0f);

        Sequence seq = DOTween.Sequence();
        seq
            .SetAutoKill(true)
            .SetUpdate(true)
            .Append(material.DOFloat(1.0f, "_ShineLocation", 0.5f).SetEase(Ease.InOutQuart))
            .Join(material.DOFloat(0.025f, "_ShineWidth", 0.5f).SetEase(Ease.InOutQuart));
    }
    // Update is called once per frame
    void Update()
    {
         
    }
}
