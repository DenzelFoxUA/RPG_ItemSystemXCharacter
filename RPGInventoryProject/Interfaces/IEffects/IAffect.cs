

using CharacterMainLib;

namespace InventoryCoreLib
{
    public interface IAffect
    {
        //logic marker

        public void Apply();

        public void SetTarget(IAffectable? target);

        public IAffect MakeMyCopy();
    }


    public interface IAffect<in T> : IAffect 
        where T: IAffectable
    {
        public void SetTarget(T? target);
    }
}
