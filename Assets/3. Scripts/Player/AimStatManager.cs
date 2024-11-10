using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AimStatManager : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;

    public AxisState xAxis, yAxis;

    [Header("�÷��̾� ����ٴϴ� ī�޶�, �ó׸ӽ� ī�޶󿡼� ������")]
    [SerializeField] Transform camFollowPos;

    [Header("�� ������ ��ǥ ����")]
    [SerializeField] Transform camFollowPosTarget;
    [SerializeField] Transform camFollowPosRunTarget;
    [SerializeField] Transform camFollowPosAimTarget;

    [SerializeField] float mouseSense;

    [Header("���� �̹��� UI")]
    [SerializeField] RectTransform crosshairImg;

    private void Start()
    {
        //setting default position
        camFollowPos.position = camFollowPosTarget.position;
    }
    // Update is called once per frame
    void Update()
    {
        //State
        if(GamePlayManager.isGamePlaying ==false)
        {
            return;
        }


        AimMode();

        xAxis.Value += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis.Value -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis.Value = Mathf.Clamp(yAxis.Value, yAxis.m_MinValue, yAxis.m_MaxValue);
    }
    private void LateUpdate()
    {
        camFollowPos.localEulerAngles = new Vector3(yAxis.Value, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis.Value, transform.eulerAngles.z);
    }
    private void AimMode()
    {
        // ���� ��� Ȱ��ȭ
        if (Input.GetKey(KeyCode.Mouse1))
        {
            camFollowPos.DOLocalMove(camFollowPosAimTarget.localPosition, 0.25f, false);
            crosshairImg.gameObject.SetActive(true);
            return;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
            camFollowPos.DOLocalMove(camFollowPosRunTarget.localPosition, 0.25f, false);
        else
            camFollowPos.DOLocalMove(camFollowPosTarget.localPosition, 0.25f, false).SetEase(Ease.OutCubic);
        crosshairImg.gameObject.SetActive(false);
    }
}
