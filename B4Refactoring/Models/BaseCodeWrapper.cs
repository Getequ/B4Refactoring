using System;
using System.Collections.Generic;
using System.Drawing;

namespace Getequ.B4Refactoring.Models
{
    public abstract class BaseCodeWrapper
    {
        protected CodeClassWrapper _parent;

        public abstract string Name { get; }

        public abstract int StartLine { get; }
        public abstract int EndLine { get; }
        public abstract Point Range { get; }

        public abstract IList<string> Code { get; }
    }
}
