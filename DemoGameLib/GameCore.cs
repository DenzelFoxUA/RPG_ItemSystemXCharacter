namespace DemoGameLib
{
    public class GameCore
    {
        public ItemsCreator ItemsCreator { get; private init; }
        
        public GameCore()
        {
            ItemsCreator = new();
        }
    }
}
