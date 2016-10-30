using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getequ.B4Refactoring.Domain
{
    using EnvDTE;

    static class ClassStorage
    {
        static Dictionary<string, object> _elements;

        public static void AddRange(IDictionary<string, CodeClass> elements)
        {
            foreach (var exist in _elements.Keys.Where(k => elements.ContainsKey(k)).ToList())
            {
                _elements.Remove(exist);
            }

            foreach (var pair in elements)
            {
                _elements.Add(pair.Key, pair.Value);
            }
        }

        public static IEnumerable<CodeClass> GetClasses(string mask)
        {
            return _elements.OfType<CodeClass>().Where(c => c.FullName.StartsWith(mask));
        }

        public static IEnumerable<CodeInterface> GetInterfaces(string mask)
        {
            return _elements.OfType<CodeInterface>().Where(c => c.FullName.StartsWith(mask));
        }
    }
}
