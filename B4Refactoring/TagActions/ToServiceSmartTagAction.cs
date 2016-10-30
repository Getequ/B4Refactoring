using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Text;

namespace Getequ.B4Refactoring.TagActions
{
    using Domain;
    using Windows;

    internal class ToServiceSmartTagAction : BaseSmartTagAction
    {
        CodeClass2 _controller;
        string     _actionName;

        public ToServiceSmartTagAction(ITrackingSpan span, FileCodeModel codeModel, CodeClass2 controller, string actionName = null)
            : base(span, codeModel)
        {
            _controller = controller;
            _actionName = actionName;
        }

        public override string DisplayText
        {
            get { return "Вынести " + (_actionName == null ? "методы" : _actionName) + " в сервис"; }
        }

        public override void Invoke()
        {
            var stickProcessor = new ControllerDomain(_codeModel.Parent.ContainingProject);

            var stincks = stickProcessor.FindStinck(new[] { _controller });
            var stinck = stincks.FirstOrDefault();

            var form = new SelectElementsForm();
            form.SetWindowData(stinck, _actionName == null ? RefactorTarget.ServiceActions : RefactorTarget.ServiceAction, _actionName);

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                stickProcessor.FindControllerRelativeClasses(stinck.Name);
                var result = stickProcessor.GenerateRefactoringCode(form.GetData());
                stickProcessor.AddToProject(result);
            }
        }
    }
}
