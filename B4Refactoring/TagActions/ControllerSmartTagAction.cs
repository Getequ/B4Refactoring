using System.Linq;

using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Text;

namespace Getequ.B4Refactoring.TagActions
{
    using Domain;
    using Windows;

    internal class ControllerSmartTagAction : BaseSmartTagAction
    {
        CodeClass2 _controller;

        public ControllerSmartTagAction(ITrackingSpan span, FileCodeModel codeModel, CodeClass2 controller)
            : base(span, codeModel)
        {
            _controller = controller;
        }

        public override string DisplayText
        {
            get { return "Вынести методы с логикой контроллера " + _display; }
        }

        public override void Invoke()
        {
            var stickProcessor = new ControllerDomain(_codeModel.Parent.ContainingProject);

            var stincks = stickProcessor.FindStinck(new[] { _controller });
            var stinck = stincks.FirstOrDefault();

            var form = new SelectElementsForm();
            form.SetWindowData(stinck, RefactorTarget.Controller);

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                stickProcessor.FindControllerRelativeClasses(stinck.Name);
                var result = stickProcessor.GenerateRefactoringCode(form.GetData());
                stickProcessor.AddToProject(result);
            }
        }
    }
}
