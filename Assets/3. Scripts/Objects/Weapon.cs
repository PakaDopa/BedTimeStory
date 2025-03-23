using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Manager;
using Cinemachine;
using System.Threading;



public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject rocketProjectilePrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] ParticleSystem reloadParticleSystem;



    
    // bool isAiming = false;
    // bool isShotting = false;



    bool isReloading = false;

    float projectileSpeed = 100f;
    

    // float currSkillCooltime = 0f;
    public float CurrSkillCooltime =>lastUseSkillTime + PlayerStats.Instance.SkillCooltime - Time.time;

    int attackIndex = 0;
    const int maxAmmo = 30;
    private const float shootDelay = 0.125f;
    float lastShotTime ;
    float lastUseSkillTime;

    int currAmmo = maxAmmo;

    // Update is called once per frame
    private void Start()
    {
        reloadParticleSystem.Stop();
        EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));

        lastUseSkillTime = -PlayerStats.Instance.SkillCooltime + 2;     //초기값
    }

    public bool TryShoot(bool isAiming,bool toShoot)
    { 
        if( isAiming && toShoot &&
             isReloading == false && currAmmo > 0 &&
             Time.time >= lastShotTime + shootDelay)
        {   
            Shoot();
            return true;
        }
        return false;
    }

    public bool TryUseSkill(bool isAiming, bool toUseSkill)
    {
        if(isAiming && toUseSkill && Time.time >= lastUseSkillTime + PlayerStats.Instance.SkillCooltime)
        {
            UseSkill();
            return true;
        }
        return false;
    }


    public bool TryReload(bool isAiming, bool toReload, bool toShoot)
    {
        if(    (toReload ||  ( toShoot &&  currAmmo <=0)) &&        // 입력감지
                currAmmo < maxAmmo && isReloading == false )        // 조건
        {
            Reload();
            return true;
        }
        return false;
    }

    //=====================================================
    void Shoot()
    {
        lastShotTime  = Time.time;

        //
        Shot();
        currAmmo--;
        EventManager.Instance.PostNotification(MEventType.OnShoot, this, new TransformEventArgs(transform,0.5f));
        EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));
    }


    // IEnumerator Shotting()
    // {
    //     //������ �����鼭, ������ ���ϰ� ������, ������ �ϰ� �־����.
    //     // bool checker = isShotting == true && isReloading == false && isAiming == true;
    //     Debug.Log(isShotting + " " + isReloading + " " + isAiming);
    //     yield return new WaitUntil(() => isShotting == true && isReloading == false && isAiming == true );
    //     if (currAmmo > 0)
    //     {
    //         Shot();
    //         currAmmo--;
    //         EventManager.Instance.PostNotification(MEventType.OnShoot, this, new TransformEventArgs(transform,0.5f));
    //         EventManager.Instance.PostNotification(MEventType.ChangeArmo, this, new TransformEventArgs(transform, currAmmo, maxAmmo));
    //         yield return new WaitForSeconds(shootDelay);
    //     }
    //     else
    //         isShotting = false;

    //     StartCoroutine(Shotting());
    // }
    // private void Aim()
    // {
    //     if (Input.GetKeyDown(KeyCode.Mouse1))
    //         isAiming = true;
    //     if (Input.GetKeyUp(KeyCode.Mouse1))
    //         isAiming = false;
    // }
    // private void IsShot() => isShotting = Input.GetKey(KeyCode.Mouse0);
    
    
    private void Shot()
    {
        // attackEventSOs[attackIndex++].Raise();


        Vector3 projectileDir = CalcDir();
        GameObject projectile = Instantiate(projectilePrefab,
            muzzle.position, Quaternion.Euler(projectileDir));
        projectile.GetComponent<Rigidbody>().AddForce(projectileDir * projectileSpeed, ForceMode.Impulse);
    }

    void UseSkill()
    {
        lastUseSkillTime = Time.time;
        
        
        Vector3 roketPrjDir = CalcDir();

        var rocketProj = Instantiate(rocketProjectilePrefab).GetComponent<RocketProjectile>();
        rocketProj.Init( muzzle.position, roketPrjDir); 

        // currSkillCooltime = PlayerStats.Instance.SkillCooltime;

        EventManager.Instance.PostNotification(MEventType.OnShoot, this, new TransformEventArgs(transform, 20f));
        
    }

    private void Reload()
    {
        StartCoroutine(ReloadCoroutine());
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
