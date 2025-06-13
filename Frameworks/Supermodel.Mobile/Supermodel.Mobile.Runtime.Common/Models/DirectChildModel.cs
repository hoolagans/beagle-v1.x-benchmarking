using System;

namespace Supermodel.Mobile.Runtime.Common.Models;

public abstract class DirectChildModel : ChildModel
{
    protected DirectChildModel()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        ParentGuidIdentities = Array.Empty<Guid>();
    }
}