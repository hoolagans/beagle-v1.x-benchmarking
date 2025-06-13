using System;
using System.Collections.Generic;

namespace Supermodel.Persistence.Entities;

[Obsolete("Use EF.Core mechanism to set up many-to-many relationships instead")]
public class M2MList<TM2M> : List<TM2M> where TM2M : IM2M { }