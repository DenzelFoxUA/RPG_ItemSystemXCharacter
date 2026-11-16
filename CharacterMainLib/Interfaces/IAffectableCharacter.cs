
namespace CharacterMainLib
{
    public interface ICanMove : IAffectable
    {
        public void Move<DVector>(DVector? directionVector);
        public void Accelerate();

        public void MovementSpeedUp(float amount);
        public void MovementSlowDown(float amount);
    }

    public interface IAffectableActor : IAffectable
    {
        public void RestoreVital(string name, uint amount);
        public void DamageVital(string name, uint amount);
    }

    public interface IAffectableCharacter
        : ICanMove, IAffectableActor
    {
        
        public void CarryWeightUp(double value);
        public void CarryWeightDown(double value);

        public void IncreaseAttribute(string name, uint amonut);
        public void DecreaseAttribute(string name, uint amonut);
        
    }
}
