using CharacterMainLib;
using InventoryCoreLib;

namespace InventoryDemo.UI;

public sealed class TrainingDummy : IDemoActor
{
    private readonly EffectsManager<IAffectableActor> _effectsManager;

    public string Name { get; }
    public uint Health { get; private set; }
    public uint MaxHealth { get; }

    public TrainingDummy(string name = "Training Dummy", uint maxHealth = 250)
    {
        Name = name;
        MaxHealth = maxHealth;
        Health = maxHealth;
        _effectsManager = new EffectsManager<IAffectableActor>(this);
    }

    public void RestoreVital(string name, uint amount)
    {
        if (!string.Equals(name, "Health", StringComparison.OrdinalIgnoreCase))
            return;

        Health = amount >= MaxHealth - Health ? MaxHealth : Health + amount;
    }

    public void DamageVital(string name, uint amount)
    {
        if (!string.Equals(name, "Health", StringComparison.OrdinalIgnoreCase))
            return;

        Health = amount >= Health ? 0 : Health - amount;
    }

    public void AddTemporaryEffect(IAffectTemporary<IAffectableActor> effect)
        => _effectsManager.AddToTemporary(effect);

    public void Update(float deltaTime) => _effectsManager.Update(deltaTime);
}
