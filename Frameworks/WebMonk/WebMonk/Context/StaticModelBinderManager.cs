using WebMonk.ModeBinding;

namespace WebMonk.Context;

public class StaticModelBinderManager : StaticModelBinderManagerBase
{
    #region IStaticModelBinderManager implementation
    public override IStaticModelBinder GetStaticModelBinder()
    {
        //We return singleton because it is thread safe
        return StaticModelBinder;
    }
    #endregion

    #region Properties
    protected IStaticModelBinder StaticModelBinder { get; } = new DefaultStaticModelBinder();
    #endregion
}