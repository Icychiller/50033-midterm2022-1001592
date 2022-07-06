using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRefill : MonoBehaviour
{
    public float minRespawnDelay = 5f;
    public float maxRespawnDelay = 10f;
    public float hoverOffset = 0.5f;
    public float hoverSpeed = 6f;

    private CapsuleCollider capsuleCollider;
    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private int hoverUp = 1;
    private Vector3 origin;
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Hover();
    }

    void Hover()
    {
        transform.position += Vector3.up * hoverSpeed * hoverUp * Time.deltaTime;
        if (Mathf.Abs((transform.position - origin).y) >= hoverOffset)
        {
            hoverUp *= -1;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Trigger Entered");
        if(col.gameObject.CompareTag("Player"))
        {
            TogglePresence(false);
            StartCoroutine(Respawn());
            TankShooting tank = col.gameObject.GetComponent<TankShooting>();
            if (tank != null)
            {
                tank.refillAmmo();
            }
        }
    }
    
    void TogglePresence(bool value)
    {
        capsuleCollider.enabled = value;
        meshRenderer.enabled = value;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(Random.Range(minRespawnDelay,maxRespawnDelay));
        TogglePresence(true);
    }
}
