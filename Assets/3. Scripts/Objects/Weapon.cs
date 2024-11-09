using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Manager;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject rocketProjectilePrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] ParticleSystem reloadParticleSystem;

    [SerializeField] private SoundEventSO[] attackEventSOs;
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
    }
    void Update()
    {
        float deltaTime = Time.deltaTime;
        currSkillCooltime = Mathf.Max(currSkillCooltime - deltaTime, 0);

        Aim();
        IsShot();
        Reload();

        if (Input.GetKeyDown(KeyCode.Q) && currSkillCooltime == 0)
        {
           Vector3 roketPrjDir = CalcDir();

           GameObject rocketProj = Instantiate(rocketProjectilePrefab,
               muzzle.position, Quaternion.Euler(roketPrjDir));

           rocketProj.GetComponent<Rigidbody>().AddForce(roketPrjDir * rocketProjectileSpeed, ForceMode.Impulse);

            currSkillCooltime = PlayerStats.Instance.SkillCooltime;
        }
    }
    IEnumerator Shotting()
    {
        //������ �����鼭, ������ ���ϰ� ������, ������ �ϰ� �־����.
        bool checker = isShotting == true && isReloading == false && isAiming == true;
        Debug.Log(isShotting + " " + isReloading + " " + isAiming);
        yield return new WaitUntil(() => isShotting == true && isReloading == false && isAiming == true );
        if (currAmmo > 0)
        {
            Shot();
            currAmmo--;
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

    private void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(currAmmo < maxAmmo && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        reloadParticleSystem.Play();
        EventManager.Instance.PostNotification(MEventType.ReloadingArmo, this, new TransformEventArgs(transform, true));
        yield return new WaitForSeconds(PlayerStats.Instance.ReloadSpeed);
        reloadParticleSystem.Stop();
        EventManager.Instance.PostNotification(MEventType.ReloadingArmo, this, new TransformEventArgs(transform, false));
        EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));
        currAmmo = maxAmmo;
        isReloading = false;
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
