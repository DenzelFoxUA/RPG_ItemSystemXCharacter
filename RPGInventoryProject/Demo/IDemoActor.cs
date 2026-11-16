using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface IDemoActor : ICollectEffects<IAffectableActor>
    {
        public void Update(float deltatime);
    }
}
