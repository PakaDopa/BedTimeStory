using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_EnemyHp : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] Enemy enemy;


    Slider slider_hp;
    GameObject go;

    Transform t;

    //=====================================================================
    void Start()
    {
        mainCamera = Camera.main;
        go= gameObject;
        t=transform;
    }
    void Update()
    {
        // 적의 월드 위치 계산
        Vector3 worldPos = enemy.t.position + Vector3.up * 3f;

        // 월드 위치를 스크린 좌표로 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        // Vector3 cameraToEnemy = enemy.t.position - mainCamera.transform.position;
        // float forwardDistance = Vector3.Dot(mainCamera.transform.forward, cameraToEnemy);

        // // 최소 거리 설정 (예: 5미터)
        // float maxForwardDistance = 5f;

        // 카메라 앞에 있고, 화면 안에 있는지 확인
        if (screenPos.z > 0 && IsInViewport(screenPos) )
        {
            go.SetActive(true);

            // HP바의 위치를 스크린 좌표로 설정
            transform.position = screenPos;
        }
        else
        {
            // 카메라 뒤에 있거나 화면 밖에 있으면 HP바를 숨김
            go.SetActive(false);
        }
    }

    private bool IsInViewport(Vector3 screenPos)
    {
        return true;
        // return screenPos.x >= 0 && screenPos.x <= Screen.width &&
        //     screenPos.y >= 0 && screenPos.y <= Screen.height;
    }

    public void Init(Enemy enemy)
    {
        this.enemy = enemy;

        slider_hp = GetComponent<Slider>();
        slider_hp.maxValue = enemy.maxHp;
        slider_hp.value = enemy.currHp;
    }

    public void OnUpdateEnemyHp()
    {
        slider_hp.value = enemy.currHp;
    }


    public void OnEnemyDie()
    {
        Destroy(gameObject);
    }
}
