using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using EnvDTE;
using EnvDTE80;

namespace Getequ.B4Refactoring.Models
{
    public class CodeFunctionWrapper : BaseCodeWrapper
    {
        CodeFunction2 _func;

        public CodeFunctionWrapper(CodeFunction2 func, CodeClassWrapper parent)
        {
            _func = func;
            _parent = parent;
        }

        public override string Name { get { return _func.Name; } }

        public bool IsOverride { get { return _func.OverrideKind == vsCMOverrideKind.vsCMOverrideKindOverride; } }

        public bool IsPublic { get { return _func.Access == vsCMAccess.vsCMAccessPublic; } }

        public bool IsGeneric { get { return _func.IsGeneric; } }

        public IList<string> GenericTypes { 
            get 
            { 
                if (!_func.IsGeneric)
                    return new List<string>();

                return _func.FullName.Unwrap("<>").Split(',').Select(x => x.Trim()).ToList();
            } 
        }


        public string DocComment { get { return _func.DocComment; } }

        public IEnumerable<CodeParameter2> Parameters { get { return _func.Parameters.OfType<CodeParameter2>(); } }

        public override int StartLine { get { return _func.StartPoint.Line; } }
        public override int EndLine { get { return _func.EndPoint.Line; } }
        public override Point Range { get { return new Point { X = StartLine, Y = EndLine }; } }

        IList<string> _code = null;

        public override IList<string> Code
        {
            get
            {
                if (_code == null)
                    _code = _parent.FileCode.GetRange(_func.StartPoint.Line - 1, _func.EndPoint.Line - 1).ToList();

                return _code;
            }
        }

        public bool ContainsWord(string word) 
        { 
            return Code.Any(x => x.ContainsWord(word));
        }
    }
}
