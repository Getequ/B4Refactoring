using System.Collections.Generic;

namespace Getequ.B4Refactoring.Models
{
    public class ClassGenerationResult
    {
        public CodeClassWrapper Class;
        public string EntityName;
        public string ViewModelClass;
        public string ViewModelInterface;
        public string ServiceClass;
        public string ServiceInterface;
        public string DomainServiceClass;
        public string Controller;
        public List<string> KnownTypes;
        public Getequ.B4Refactoring.Domain.ControllerDomain.ControllerModification Modification;
    }
}
