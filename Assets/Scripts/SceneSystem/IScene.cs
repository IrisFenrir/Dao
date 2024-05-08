namespace Dao.SceneSystem
{
    public abstract class IScene
    {
        public string name;

        public virtual void Enable()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnUpdate(float deltaTime)
        {

        }

        public virtual void Disable()
        {

        }

        public virtual void Show()
        {
            
        }

        public virtual void Hide()
        {
            
        }
    }
}
