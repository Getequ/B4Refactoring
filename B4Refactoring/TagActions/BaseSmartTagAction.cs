using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Windows.Media;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Classification;

using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Windows.Media.Imaging;

namespace Getequ.B4Refactoring.TagActions
{
    internal abstract class BaseSmartTagAction : ISmartTagAction
    {
        protected ITrackingSpan _span;
        protected string        _display;
        protected ITextSnapshot _snapshot;
        protected FileCodeModel _codeModel;

        public BaseSmartTagAction(ITrackingSpan span, FileCodeModel codeModel)
        {
            _span = span;
            _snapshot = span.TextBuffer.CurrentSnapshot;
            _display = span.GetText(_snapshot);
            _codeModel = codeModel;
        }

        public abstract string DisplayText{ get; }

        public ImageSource Icon
        {
            get
            {
                var iconResourceName = string.Format(@"pack://application:,,,/{0};component/Resources/method.png",
                                                   this.GetType().Assembly.FullName.Split(',')[0]);
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(iconResourceName);
                img.EndInit();
                return img;
            }
        }
        public bool IsEnabled
        {
            get { return true; }
        }

        public ISmartTagSource Source
        {
            get;
            private set;
        }

        public ReadOnlyCollection<SmartTagActionSet> ActionSets
        {
            get { return null; }
        }

        public abstract void Invoke();
    }
}
