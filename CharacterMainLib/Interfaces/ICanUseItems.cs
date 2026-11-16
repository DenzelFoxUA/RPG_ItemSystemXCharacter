

namespace CharacterMainLib
{
    public interface ICanUseItems<ItemType>
    {
        public void SetItemTarget(ref ItemType Item, IAffectableActor? target);

        public void UseItem(ItemType? item, IAffectableActor? target);

        public void DoNothing();
    }
}
