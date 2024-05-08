using System;
using System.Threading.Tasks;

namespace Dao.WordSystem
{
    public abstract class IDialog
    {
        public IDialog[] Next { get; protected set; }

        public Action onStart;
        public Action onStop;

        public abstract Task Show();

        public abstract void Close();
    }
}
