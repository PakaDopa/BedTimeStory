using DG.Tweening;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusCheckUI : MonoBehaviour
{
    // [SerializeField] private TMP_Text text;
    // [SerializeField] private TMP_Text reloadingText;
    // [SerializeField] Slider ammoSlide;
    [SerializeField] CrossHair crossHair;
    [SerializeField] private Image crosshair;
    [SerializeField] private Image crosshair2;

    // [SerializeField] private TMP_Text hpText;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(MEventType.ChangeArmo, ChangeArmoText);
        EventManager.Instance.AddListener(MEventType.ReloadingArmo, ReloadingText);
        EventManager.Instance.AddListener(MEventType.EnemyHitted, HittedEffect);
    }

    private void ChangeArmoText(MEventType MEventType, Component Sender, EventArgs args = null)
    {
        TransformEventArgs tArgs = args as TransformEventArgs;
        int ammoRemain= (int)tArgs.value[0];
        int ammoMax = (int)tArgs.value[1];
        crossHair.UpdateAmmoRemain( ammoRemain,ammoMax);
    }
    private void ReloadingText(MEventType MEventType, Component Sender, EventArgs args = null)
    {
        TransformEventArgs tArgs = args as TransformEventArgs;
        bool isReload = bool.Parse(tArgs.value[0].ToString());
        float duration = (float) tArgs.value[1];
        crossHair.OnReload( isReload, duration );
    }

    private void HittedEffect(MEventType MEventType, Component Sender, EventArgs args = null)
    {
        TransformEventArgs tArgs = args as TransformEventArgs;
        var seq = DOTween.Sequence();
        seq
            .SetAutoKill(true)
            .OnStart(() =>
            {
                crosshair2.gameObject.SetActive(true);
                crosshair.transform.localScale = new Vector3(1,1,1);
                crosshair2.transform.localScale = new Vector3(1,1,1);
            })
            .Join(crosshair.transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0f), 0.125f))
            .Join(crosshair.DOColor(new Color(1,0,0), 0.125f))
            .Join(crosshair2.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 0f), 0.125f))
            .Join(crosshair2.DOColor(new Color(1,0,0), 0.125f))
            .OnComplete(()=>
            {
                crosshair2.gameObject.SetActive(false);
                crosshair.color = new Color(1, 1, 1);
                crosshair2.color = new Color(1, 1, 1);
            });
    }



}
