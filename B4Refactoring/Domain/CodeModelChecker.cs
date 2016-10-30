using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.Text.Operations;
using EnvDTE;

namespace Getequ.B4Refactoring.Domain
{
    public static class CodeModelChecker
    {
        public static bool HasBadActions(FileCodeModel fcm, TextExtent word)
        {
            var ctl = GetController(fcm);
            var methods = ctl.Members.OfType<CodeFunction>().ToList();
            return false;
        }

        private static CodeClass GetController(FileCodeModel fcm)
        {
            foreach (CodeNamespace ns in fcm.CodeElements.OfType<CodeNamespace>())
            {
                foreach (CodeClass cls in ns.Members.OfType<CodeClass>().Where(c => c.Name.EndsWith("Controller")))
                {
                    return cls;
                }
            }
            return null;
        }

        private static bool IsActionBad(CodeFunction function)
        {
            return (function.StartPoint.Line + 5 < function.EndPoint.Line) || function.Access == vsCMAccess.vsCMAccessPrivate;
        }
    }
}
