using VContainer;
using VContainer.Unity;

namespace DI
{
    public class ProjectScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
        }
    }
}
