using System;

namespace Supermodel.Persistence.Entities;

[Obsolete("Use EF.Core mechanism to set up many-to-many relationships instead")]
public interface IM2M
{
    #region Methods
    IEntity GetConnectionToOther(Type otherType);
    void SetConnectionToOther(IEntity other);
    #endregion
}