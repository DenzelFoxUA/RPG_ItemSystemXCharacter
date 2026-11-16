
using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface ICollectEffects<TTarget> : IAffectableActor
        where TTarget : IAffectable
    {
        public void AddTemporaryEffect(IAffectTemporary<TTarget> effect);

    }
}
