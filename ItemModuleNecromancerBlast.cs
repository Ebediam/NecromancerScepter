using BS;

namespace NecromancerScepter
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleNecromancerBlast : ItemModule
    {

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemNecromancerBlast>();
        }
    }
}
