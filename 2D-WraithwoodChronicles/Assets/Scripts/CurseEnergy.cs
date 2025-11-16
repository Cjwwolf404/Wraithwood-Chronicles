using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseEnergy : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.TriggerModule triggerModule;

    private GameObject player;
    private Collider2D playerCollider2D;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        triggerModule = ps.trigger;
        
        SetupTriggerModule();
    }

    public void SetupTriggerModule()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerCollider2D = player.GetComponent<Collider2D>();
        } 

        triggerModule.enabled = true;

        triggerModule.SetCollider(0, playerCollider2D);
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> entered = new List<ParticleSystem.Particle>();

        int numEntered = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered);

        for(int i = 0; i < numEntered; i++)
        {
            ParticleSystem.Particle p = entered[i];
            p.remainingLifetime = 0f;
            entered[i] = p;
            GameManager.Instance.currentCurseEnergyAmount += 1;
            UIManager.Instance.UpdateCurseEnergyAmount();
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, entered);
    }
}
