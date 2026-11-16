
namespace CharacterMainLib
{
    public interface IModifiableStat : IAffectable
    {
        public void Increase(uint value);
        public void Decrease(uint value);
        public void Set(uint value);
    }

}
