using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;
    public int maxAmmoCount = 6;
    public float autoRefillDelay = 10f;
    public bool autoRefill = false;

    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;
    private float nextFireTime;

    private int shotsFired = 0;
    private int ammoCount;
    private bool refillingCycle = false;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        shotsFired = 0;
        ammoCount = maxAmmoCount;
        refillingCycle = false;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired && ammoCount > 0)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire(m_CurrentLaunchForce, 1);
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired && ammoCount > 0)
        {
            Fire(m_CurrentLaunchForce, 1);
        }
        if (autoRefill && !refillingCycle)
        {
            StartCoroutine(AiRefill());
        }
    }


    public void Fire(float launchForce, float fireRate)
    {
        if (Time.time <= nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        m_Fired = true;

        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        shotsFired++;
        ammoCount--;

        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    public bool CheckPacifist()
    {
        return shotsFired == 0;
    }

    public void refillAmmo()
    {
        ammoCount = maxAmmoCount;
    }
    
    IEnumerator AiRefill()
    {
        refillingCycle = true;
        while(gameObject.activeInHierarchy)
        {
            
            yield return new WaitForSeconds(autoRefillDelay);
            refillAmmo();
        }
    }
}