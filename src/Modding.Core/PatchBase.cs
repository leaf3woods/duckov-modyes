

namespace Modding.Core
{
    public abstract class PatchBase
    {
        protected PatchBase()
        {
            InitializeLogger();
        }
        protected abstract void InitializeLogger();
    }
}
