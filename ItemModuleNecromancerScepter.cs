using BS;

namespace NecromancerScepter
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleNecromancerScepter : ItemModule
    {
        public string allyCreatureId = "Human";
        public string enemyCreatureId = "Human";
        public float portalSummonRate = 5f;
        public int portalMaxSummons = 5;


        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemNecromancerScepter>();
        }
    }
}
