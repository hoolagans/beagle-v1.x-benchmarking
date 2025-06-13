using System;

namespace Supersonic
{
    namespace IndexedCollections
    {
        public abstract class SupersonicListItem : ISupersonicListItem
        {
            public Guid Guid { get; } = Guid.NewGuid();
        }
    }
}
