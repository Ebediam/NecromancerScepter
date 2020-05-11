using BS;
using UnityEngine;
using System.Collections;

namespace NecromancerScepter
{

    public class ItemNecromancerBlast : MonoBehaviour
    {
        protected Item item;

        public Rigidbody rb;

        public ItemNecromancerScepter scepter;

        public ParticleSystem bulletVFX;
        public ParticleSystem impactVFX;
        public AudioSource impactSFX;
        public Collider col;

        public int summonFaction;
        public CreatureData summonData;

        public Mode mode;


        protected void Start()
        {
            item = this.GetComponent<Item>();
            rb = item.GetComponent<Rigidbody>();

            bulletVFX = transform.Find("BulletVFX").GetComponent<ParticleSystem>();
            impactVFX = transform.Find("ImpactVFX").GetComponent<ParticleSystem>();
            if (impactVFX)
            {
                Debug.Log("ImpactVFX found");
            }
            else
            {
                Debug.LogError("ImpactVFX not found");
            }
            impactVFX.Stop();

            

            col = gameObject.GetComponentInChildren<Collider>();

            item.OnCollisionEvent += OnBubbleCollisionEvent;
            impactSFX = transform.Find("ImpactSFX").GetComponent<AudioSource>();

            if (!impactSFX)
            {
                Debug.LogError("ImpactSFX not found");
            }

        }


        

        void OnBubbleCollisionEvent(ref CollisionStruct collisionInstance)
        {
            Debug.Log("Collision detected with " + collisionInstance.targetCollider.transform.root.name);
            EbediamHandyFunctions.TargetType targetType = EbediamHandyFunctions.Utils.GetTargetType(collisionInstance);

            rb.isKinematic = true;
            col.enabled = false;
            SummonVFX();
            switch (targetType)
            {
                case EbediamHandyFunctions.TargetType.Environment:

                    Portal portal = collisionInstance.targetCollider.GetComponentInParent<Portal>();
                    if (portal)
                    {
                        portal.maxSummons = 0;
                        portal.timer = 99f;
                    }
                    else
                    {
                        if (mode == Mode.Summon)
                        {
                            Summon();
                        }
                        else
                        {
                            Debug.Log("Create portal starts");
                            scepter.CreatePortal(transform.position, summonData, summonFaction);
                        }

                    }

                    break;


                case EbediamHandyFunctions.TargetType.NPC:
                    Creature creature = collisionInstance.targetCollider.GetComponentInParent<Creature>();
                    if (creature)
                    {
                        if(creature.state == Creature.State.Dead)
                        {
                            creature.health.Resurrect(10f, Creature.player);
                            creature.ragdoll.SetState(Creature.State.Destabilized);                            
                            
                        }

                        creature.SetFaction(2);

                    }

                    break;


                

                default:
                    Debug.Log("TargetType not considered");
                    break;
            }



        }

        public void SummonVFX()
        {
            impactVFX.Play();
            bulletVFX.Stop();
            impactSFX.Play();

        }


        public void Summon()
        {
            Creature creature = summonData.Spawn(transform.position, transform.rotation, false);
            StartCoroutine(scepter.ChangeFaction(creature, summonFaction));
            StartCoroutine(Deactivate(item, 2f));   
        }

        IEnumerator Deactivate(Item item, float time)
        {

            yield return new WaitForSeconds(time);

            if (item)
            {
                item.gameObject.SetActive(false);
            }

        }

    }
}