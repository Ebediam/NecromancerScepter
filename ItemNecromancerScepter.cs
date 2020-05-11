using BS;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace NecromancerScepter
{
    public enum Mode
    {
        Summon,
        Portal
    }

    public class ItemNecromancerScepter : MonoBehaviour
    {

        public Item item;

        public ItemModuleNecromancerScepter module;       

        public ItemShooter.ItemShooter shootController;

        public ItemNecromancerBlast blast;

        public CreatureData allyData;
        public CreatureData enemyData;
        public List<CreatureData> creatureDatas = new List<CreatureData>();

        public ParticleSystem indicatorVFX;

        public GameObject portal;

        public bool heldTrigger;
        public bool heldButton;

        bool active = false;

        public Animator animator;

        protected void Start()
        {
            
            item = this.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleNecromancerScepter>();

            

            item.OnHeldActionEvent += OnHeldAction;

            if (item.GetComponent<ItemShooter.ItemShooter>())
            {
                shootController = item.GetComponent<ItemShooter.ItemShooter>();
                shootController.isShootingAllowed = false;
            }
            else
            {
                Debug.Log("ItemShooter not found");
            }

            animator = gameObject.GetComponentInChildren<Animator>();

            portal = transform.Find("Portal").gameObject;

            indicatorVFX = transform.Find("Indicator").GetComponent<ParticleSystem>();
            indicatorVFX.Stop();

            RetrieveCreatureData();

            Invoke("Activate", 1f);

        }
        
        public void Activate()
        {
            active = true;
        }

        public IEnumerator ChangeFaction(Creature ally, int faction)
        {
            while(!ally.initialized)
            {                
                yield return null;
            }

            ally.SetFaction(faction);
            Debug.Log("NPC succesfully changed faction");
                        
        }

        public void RetrieveCreatureData()
        {
            foreach (List<CatalogData> data in Catalog.current.data)
            {
                foreach (CatalogData _data in data)
                {
                    if (_data is CreatureData)
                    {


                        CreatureData _creatureData = _data as CreatureData;

                        creatureDatas.Add(_creatureData);

                        if(_creatureData.id == module.allyCreatureId)
                        {
                            allyData = _creatureData;
                        }

                        if(_creatureData.id == module.enemyCreatureId)
                        {
                            enemyData = _creatureData;
                        }
                       

                    }
                }
            }

            if (allyData == null)
            {
                Debug.LogError("No ally data found!");
                allyData = creatureDatas[0];
            }

            if (enemyData == null)
            {
                Debug.LogError("No enemy data found!");
                enemyData = creatureDatas[0];
            }
        }
        
        public void CreatePortal(Vector3 position, CreatureData summonData, int summonFaction)
        {
            GameObject newPortal = Instantiate(portal);

            newPortal.transform.position = position;
            newPortal.transform.LookAt(transform.position + Vector3.up*100f);
            newPortal.SetActive(true);
            Portal portalScript = newPortal.AddComponent<Portal>();
            portalScript.summonData = summonData;
            portalScript.summonFaction = summonFaction;
            portalScript.summonRate = module.portalSummonRate;


        }

        public void OnHeldAction(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (!active)
            {
                return;
            }


            if (action == Interactable.Action.UseStart)
            {
                indicatorVFX.Play();
                heldTrigger = true;
                if (animator)
                {
                    animator.SetBool("isPressing", true);
                }
                handle.SetSliding(interactor, false);
            }

            if(action == Interactable.Action.AlternateUseStart)
            {
                indicatorVFX.Play();
                heldButton = true;
                if (animator)
                {
                    animator.SetBool("isPressing", true);
                }
                
            }

            if(action == Interactable.Action.UseStop)
            {
                indicatorVFX.Stop();
                heldTrigger = false;

                if (animator)
                {
                    animator.SetBool("isPressing", false);
                }


                if (heldButton)
                {
                    heldButton = false;
                    ShootBlast(3, Mode.Portal);
                }
                else
                {

                    ShootBlast(2, Mode.Summon);
                }               
                
                
            }

            if(action == Interactable.Action.AlternateUseStop)
            {
                indicatorVFX.Stop();

                if (animator)
                {
                    animator.SetBool("isPressing", false);
                }
                heldButton = false;

                if (heldTrigger)
                {
                    ShootBlast(2, Mode.Portal);
                    heldTrigger = false;
                }
                else
                {
                    ShootBlast(3, Mode.Summon);
                }
                
                
            }

        }


      
        public void ShootBlast(int faction, Mode mode)
        {
            Item bullet = shootController.TryShootInmediate();

            if (bullet)
            {
                Debug.Log("Necromancer blast successfully spawned");
            }
            else
            {
                Debug.LogError("Necromancer scepter error: No bullet was instantiated");
                return;
            }

            blast = bullet.GetComponent<ItemNecromancerBlast>();
            if (!blast)
            {
                Debug.LogError("Necromancer scepter error: Designed bullet is not NecromancerBlast");
                return;
            }
            Debug.Log("Necromancer blast controller found");

            blast.scepter = this;
            blast.summonFaction = faction;
            blast.mode = mode;

            Debug.Log("Blast created with mode: " + mode.ToString());

            if(faction == 2)
            {
                blast.summonData = allyData;                
            }
            else
            {
                blast.summonData = enemyData;
            }
        }


        

    }
}