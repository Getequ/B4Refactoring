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
using EnvDTE80;

namespace Getequ.B4Refactoring.Tags
{
    using TagActions;

    internal class B4SmartTag : SmartTag
    {
        public B4SmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets) :
            base(SmartTagType.Ephemeral, actionSets) { }
    }

    internal class B4SmartTagger : ITagger<B4SmartTag>, IDisposable
    {
        private ITextBuffer     _buffer;
        private ITextView       _view;
        //private TestSmartTaggerProvider _provider;
        private bool            _disposed;
        private FileCodeModel   _codeModel;
        //private SnapshotPoint RequestedPoint;
        //private SnapshotSpan? CurrentWord;
        private NormalizedSnapshotSpanCollection _wordSpans;
        private ITextSearchService      _textSearchService;
        private ITextStructureNavigator _textStructureNavigator;
        private CodeClass2 _controller;

        object updateLock = new object();

        public B4SmartTagger(ITextBuffer buffer, ITextView view, B4SmartTaggerProvider provider,
                               ITextSearchService textSearchService,
                               ITextStructureNavigator textStructureNavigator, DTE dte)
        {
            _buffer = buffer;
            _view = view;
            //_provider = provider;
            _view.LayoutChanged += OnLayoutChanged;

            ITextDocument document;

            view.TextDataModel.DataBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
            _textSearchService = textSearchService;
            _textStructureNavigator = textStructureNavigator;
            _codeModel = GetDocumentCodeModel(document, dte.Windows);
        }

        public IEnumerable<ITagSpan<B4SmartTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            NormalizedSnapshotSpanCollection wordSpans = _wordSpans;

            if (_wordSpans == null)
                yield break;

            if (spans.Count == 0 || wordSpans.Count == 0 || _codeModel == null)
                yield break;

            // If the requested snapshot isn't the same as the one our words are on, translate our spans to the expected snapshot 
            if (spans[0].Snapshot != wordSpans[0].Snapshot)
            {
                wordSpans = new NormalizedSnapshotSpanCollection(
                    wordSpans.Select(span => span.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive)));
            }

            // Second, yield all the other words in the file 
            foreach (SnapshotSpan span in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans))
            {
                yield return new TagSpan<B4SmartTag>(span, new B4SmartTag(GetSmartTagActions(span)));
            }
        }

        FileCodeModel GetDocumentCodeModel(ITextDocument document, EnvDTE.Windows windows)
        {
            foreach (Window win in windows)
            {
                try
                {
                    if (win.ProjectItem != null && win.ProjectItem.FileCount > 0 && win.ProjectItem.FileNames[1] == document.FilePath)
                        return win.ProjectItem.FileCodeModel;
                }
                catch { }
            }
            return null;
        }

        private void FindBadActions(List<SnapshotSpan> wordSpans, SnapshotSpan currentWord)
        {
            foreach (CodeNamespace ns in _codeModel.CodeElements.OfType<CodeNamespace>())
            {
                foreach (CodeClass2 cls in ns.Members.OfType<CodeClass2>().Where(c => c.Name.EndsWith("Controller")))
                {
                    _controller = cls;
                    var hasBadActions = false;
                    if (cls.ProjectItem != null && cls.ProjectItem.FileCodeModel == _codeModel)
                    {
                        foreach (CodeFunction fn in cls.Members.OfType<CodeFunction>())
                        {
                            if (IsActionBad(fn))
                            {
                                FindData fData = new FindData(fn.Name, currentWord.Snapshot);
                                fData.FindOptions = FindOptions.WholeWord | FindOptions.MatchCase;

                                var res = _textSearchService.FindNext(fn.StartPoint.AbsoluteCharOffset + fn.StartPoint.Line - 1, false, fData);
                                if (res.HasValue)
                                {
                                    wordSpans.Add(res.Value);
                                    hasBadActions = true;
                                }
                            }
                        }

                        if (hasBadActions)
                        {
                            FindData fData = new FindData(cls.Name, currentWord.Snapshot);
                            fData.FindOptions = FindOptions.WholeWord | FindOptions.MatchCase;

                            var res = _textSearchService.FindNext(cls.StartPoint.AbsoluteCharOffset + cls.StartPoint.Line - 1, false, fData);
                            if (res.HasValue)
                            {
                                wordSpans.Add(res.Value);
                            }
                        }
                    }
                    break;
                }
            }
        }

        private bool IsActionBad(CodeFunction function)
        {
            return (function.StartPoint.Line + 5 < function.EndPoint.Line) || function.Access == vsCMAccess.vsCMAccessPrivate;
        }

        private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span)
        {
            List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet>();
            List<ISmartTagAction> actionList = new List<ISmartTagAction>();

            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);
            var mName = trackingSpan.GetText(span.Snapshot);

            if (mName.EndsWith("Controller"))
            {
                actionList.Add(new ControllerSmartTagAction(trackingSpan, _codeModel, _controller));
                actionList.Add(new BaseViewModelSmartTagAction(trackingSpan, _codeModel, _controller));
                actionList.Add(new ToViewModelSmartTagAction(trackingSpan, _codeModel, _controller));
                actionList.Add(new ToServiceSmartTagAction(trackingSpan, _codeModel, _controller));
            }
            else 
            if (mName.Contains("Get") || mName.Contains("List"))
            {
                if (mName == "Get" || mName == "List")
                    actionList.Add(new BaseViewModelSmartTagAction(trackingSpan, _codeModel, _controller));

                actionList.Add(new ToViewModelSmartTagAction(trackingSpan, _codeModel, _controller, mName));
                actionList.Add(new ToViewModelSmartTagAction(trackingSpan, _codeModel, _controller));
            }
            else
            {
                actionList.Add(new ToServiceSmartTagAction(trackingSpan, _codeModel, _controller, mName));
                actionList.Add(new ToServiceSmartTagAction(trackingSpan, _codeModel, _controller));
            }
            SmartTagActionSet actionSet = new SmartTagActionSet(actionList.AsReadOnly());
            actionSetList.Add(actionSet);
            return actionSetList.AsReadOnly();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var caretPosition = _view.Caret.Position;

            SnapshotPoint? point = caretPosition.Point.GetPoint(_buffer, caretPosition.Affinity);

            if (!point.HasValue)
                return;

            /*/ If the new caret position is still within the current word (and on the same snapshot), we don't need to check it 
            if (CurrentWord.HasValue
                && CurrentWord.Value.Snapshot == m_view.TextSnapshot
                && point.Value >= CurrentWord.Value.Start
                && point.Value <= CurrentWord.Value.End)
            {
                return;
            }*/

            List<SnapshotSpan> wordSpans = new List<SnapshotSpan>();
            //Find all words in the buffer like the one the caret is on
            TextExtent word = _textStructureNavigator.GetExtentOfWord(point.Value);

            if (_codeModel == null)
            {
                //If we couldn't find a word, clear out the existing markers
                SynchronousUpdate(new NormalizedSnapshotSpanCollection());
                return;
            }

            FindBadActions(wordSpans, word.Span);

            SynchronousUpdate(new NormalizedSnapshotSpanCollection(wordSpans));
        }

        void SynchronousUpdate(NormalizedSnapshotSpanCollection newSpans)
        {
            lock (updateLock)
            {
                _wordSpans = newSpans;

                var tempEvent = TagsChanged;
                if (tempEvent != null)
                    tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _view.LayoutChanged -= OnLayoutChanged;
                    _view = null;
                }

                _disposed = true;
            }
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [Order(Before = "default")]
    [TagType(typeof(SmartTag))]
    internal class B4SmartTaggerProvider : IViewTaggerProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer == null || textView == null)
            {
                return null;
            }

            if (buffer == textView.TextBuffer)
            {
                ITextStructureNavigator textStructureNavigator = NavigatorService.GetTextStructureNavigator(buffer);

                return new B4SmartTagger(buffer, textView, this, TextSearchService, textStructureNavigator, 
                                            (DTE)ServiceProvider.GetService(typeof(DTE))) as ITagger<T>;
            }
            else 
                return null;
        }
    }
}
