using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS;
using UnityEngine;
using System.Collections;

namespace NecromancerScepter
{
    

    public class Portal: MonoBehaviour
    {
        public CreatureData summonData;
        public int summonFaction;
        public float timer = 0f;
        public float summonRate;
        public int count;
        public int maxSummons = 5;

        public void Start()
        {
            Debug.Log("Portal successfully created");
        }

        public void Update()
        {
            timer += Time.deltaTime;

            if(timer > summonRate)
            {
                if(count > maxSummons)
                {
                    Destroy(gameObject);
                }

                timer = 0f;
                Summon();
                count++;
            }

        }

        public void Summon()
        {
            Creature creature = summonData.Spawn(transform.position, transform.rotation);
            StartCoroutine(ChangeFaction(creature, summonFaction));
            
        }


        public IEnumerator ChangeFaction(Creature ally, int faction)
        {
            while (!ally.initialized)
            {

                yield return null;
            }

            ally.SetFaction(faction);
            Debug.Log("NPC succesfully changed faction");

        }

    }
}
