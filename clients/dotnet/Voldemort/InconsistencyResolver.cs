﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Voldemort
{
    public interface InconsistencyResolver
    {
        void resolveConflicts(IList<Versioned> items);
    }
}
