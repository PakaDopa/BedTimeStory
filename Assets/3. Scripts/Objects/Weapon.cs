using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Manager;
using Cinemachine;



public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject rocketProjectilePrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] ParticleSystem reloadParticleSystem;

    [SerializeField] private SoundEventSO[] attackEventSOs;
    [SerializeField] private SoundEventSO reloadEventSo;
    bool isAiming = false;
    bool isShotting = false;
    bool isReloading = false;

    float projectileSpeed = 100f;
    float rocketProjectileSpeed = 50f;

    float currSkillCooltime = 0f;
    public float CurrSkillCooltime => currSkillCooltime;

    int attackIndex = 0;
    const int maxAmmo = 30;
    private const float delay = 0.125f;
    int currAmmo = maxAmmo;

    // Update is called once per frame
    private void Start()
    {
        reloadParticleSystem.Stop();
        StartCoroutine(Shotting());
        EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));
    }
    void Update()
    {
        if(GamePlayManager.isGamePlaying ==false)
        {
            return;
        }

        
        float deltaTime = Time.deltaTime;
        currSkillCooltime = Mathf.Max(currSkillCooltime - deltaTime, 0);

        Aim();
        IsShot();
        Reload();

        if (Input.GetKeyDown(KeyCode.Q) && currSkillCooltime == 0)
        {
           UseSkill();
        }
    }
    IEnumerator Shotting()
    {
        //������ �����鼭, ������ ���ϰ� ������, ������ �ϰ� �־����.
        // bool checker = isShotting == true && isReloading == false && isAiming == true;
        Debug.Log(isShotting + " " + isReloading + " " + isAiming);
        yield return new WaitUntil(() => isShotting == true && isReloading == false && isAiming == true );
        if (currAmmo > 0)
        {
            Shot();
            currAmmo--;
            EventManager.Instance.PostNotification(MEventType.OnShoot, this, new TransformEventArgs(transform,0.5f));
            EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));
            yield return new WaitForSeconds(delay);
        }
        else
            isShotting = false;

        StartCoroutine(Shotting());
    }
    private void Aim()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            isAiming = true;
        if (Input.GetKeyUp(KeyCode.Mouse1))
            isAiming = false;
    }
    private void IsShot() => isShotting = Input.GetKey(KeyCode.Mouse0);
    private void Shot()
    {
        attackEventSOs[attackIndex++].Raise();
        if (attackIndex >= attackEventSOs.Length)
            attackIndex = 0;
        Vector3 projectileDir = CalcDir();
        GameObject projectile = Instantiate(projectilePrefab,
            muzzle.position, Quaternion.Euler(projectileDir));
        projectile.GetComponent<Rigidbody>().AddForce(projectileDir * projectileSpeed, ForceMode.Impulse);
    }

    void UseSkill()
    {
        Vector3 roketPrjDir = CalcDir();

           GameObject rocketProj = Instantiate(rocketProjectilePrefab, muzzle.position, Quaternion.Euler(roketPrjDir));

           rocketProj.GetComponent<Rigidbody>().AddForce(roketPrjDir * rocketProjectileSpeed, ForceMode.Impulse);

        currSkillCooltime = PlayerStats.Instance.SkillCooltime;

        EventManager.Instance.PostNotification(MEventType.OnShoot, this, new TransformEventArgs(transform, 20f));
    }

    private void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(currAmmo < maxAmmo && !isReloading)
            {
                reloadEventSo.Raise();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        reloadParticleSystem.Play();
        float reloadDuration = PlayerStats.Instance.ReloadSpeed;
        EventManager.Instance.PostNotification(MEventType.ReloadingArmo, this, new TransformEventArgs(transform, true, reloadDuration));
        yield return new WaitForSeconds(reloadDuration);
        reloadParticleSystem.Stop();
        currAmmo = maxAmmo;
        isReloading = false;
        EventManager.Instance.PostNotification(MEventType.ReloadingArmo, this, new TransformEventArgs(transform, false, reloadDuration));
        EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));
    }

    private Vector3 CalcDir()
    {
        Vector3 startPos = muzzle.position;
        Vector3 endPos;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 10000))
        {
            endPos = hitInfo.point;
        }
        else
        {
            endPos = Camera.main.transform.position + Camera.main.transform.forward * 10000;
        }

        Vector3 projectileDir = Vector3.Normalize((endPos - startPos));
        return projectileDir;
    }
}
