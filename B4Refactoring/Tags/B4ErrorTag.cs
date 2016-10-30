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


namespace Getequ.B4Refactoring.Tags
{
    public class B4ErrorTag : IErrorTag
    {
        [Order(Before = "End")]
        [Name(Identifier)]
        [DisplayName(Identifier)]
        [Export(typeof(ErrorTypeDefinition))]
        public static ErrorTypeDefinition B4ErrorTypeDefinition
        {
            get { throw new NotImplementedException(); }
        }


        [Name(Identifier)]
        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(true)]
        public class FormatDefinition : EditorFormatDefinition
        {
            public FormatDefinition()
            {
                ForegroundColor = Colors.Purple;
                BackgroundCustomizable = false;
                DisplayName = Identifier;
            }
        }

        public const string Identifier = "B4 error";

        //public IServiceProvider ServiceProvider { get; private set; }
        //public ITrackingSpan TrackingSpan { get; private set; }
        public string MisspelledWord { get; private set; }

        public string ErrorType{ get { return Identifier; } }
        
        public object ToolTipContent
        {
            get
            {
                return string.Format
                (
                    "The spelling of the word '{0}' is not recognized.",
                    MisspelledWord
                );
            }
        }
        
        public B4ErrorTag(string misspelledWord)
        {
            //if (serviceProvider == null)
            //{
            //    throw new ArgumentNullException("serviceProvider");
            //}

            //if (trackingSpan == null)
            //{
            //    throw new ArgumentNullException("trackingSpan");
            //}

            if (string.IsNullOrEmpty(misspelledWord))
            {
                throw new ArgumentNullException("misspelledWord");
            }

            //ServiceProvider = serviceProvider;
            //TrackingSpan = trackingSpan;
            //MisspelledWord = misspelledWord;
        }
         
    }

    internal class B4ErrorTagger : ITagger<B4ErrorTag>
    {
        ITextView View { get; set; }
        ITextBuffer SourceBuffer { get; set; }
        ITextSearchService TextSearchService { get; set; }
        ITextStructureNavigator TextStructureNavigator { get; set; }
        NormalizedSnapshotSpanCollection WordSpans { get; set; }
        SnapshotSpan? CurrentWord { get; set; }
        SnapshotPoint RequestedPoint { get; set; }
        FileCodeModel FCM { get; set; }
        object updateLock = new object();

        public B4ErrorTagger(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService, ITextStructureNavigator textStructureNavigator, DTE dte)
        {
            this.View = view;
            this.SourceBuffer = sourceBuffer;
            this.TextSearchService = textSearchService;
            this.TextStructureNavigator = textStructureNavigator;
            this.WordSpans = new NormalizedSnapshotSpanCollection();
            this.CurrentWord = null;
            this.View.LayoutChanged += ViewLayoutChanged;

            ITextDocument document;

            view.TextDataModel.DataBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);

            FCM = GetDocumentCodeModel(document, dte.Windows);
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
                catch { };
            }
            return null;
        }

        void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            SnapshotPoint? point = View.Caret.Position.Point.GetPoint(SourceBuffer, View.Caret.Position.Affinity);

            if (!point.HasValue)
                return;

            // If the new caret position is still within the current word (and on the same snapshot), we don't need to check it 
            if (CurrentWord.HasValue
                && CurrentWord.Value.Snapshot == View.TextSnapshot
                && point.Value >= CurrentWord.Value.Start
                && point.Value <= CurrentWord.Value.End)
            {
                return;
            }

            RequestedPoint = point.Value;
            UpdateWordAdornments();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        void UpdateWordAdornments()
        {
            SnapshotPoint currentRequest = RequestedPoint;
            List<SnapshotSpan> wordSpans = new List<SnapshotSpan>();
            //Find all words in the buffer like the one the caret is on
            TextExtent word = TextStructureNavigator.GetExtentOfWord(currentRequest);

            if (FCM == null)
            {
                //If we couldn't find a word, clear out the existing markers
                SynchronousUpdate(new NormalizedSnapshotSpanCollection());
                return;
            }

            FindBadElements(wordSpans, word.Span);

            SynchronousUpdate(new NormalizedSnapshotSpanCollection(wordSpans));
        }

        private void FindBadElements(List<SnapshotSpan> wordSpans, SnapshotSpan currentWord)
        {
            foreach (CodeNamespace ns in FCM.CodeElements.OfType<CodeNamespace>())
            {
                foreach (CodeClass cls in ns.Members.OfType<CodeClass>().Where(c => c.Name.EndsWith("Controller")))
                {
                    var hasBadActions = false;
                    if (cls.ProjectItem != null && cls.ProjectItem.FileCodeModel == FCM)
                    {
                        foreach (CodeFunction fn in cls.Members.OfType<CodeFunction>())
                        {
                            if (IsActionBad(fn))
                            {
                                FindData fData = new FindData(fn.Name, currentWord.Snapshot);
                                fData.FindOptions = FindOptions.WholeWord | FindOptions.MatchCase;

                                var res = TextSearchService.FindNext(fn.StartPoint.AbsoluteCharOffset + fn.StartPoint.Line - 1, false, fData);
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

                            var res = TextSearchService.FindNext(cls.StartPoint.AbsoluteCharOffset + cls.StartPoint.Line - 1, false, fData);
                            if (res.HasValue)
                            {
                                wordSpans.Add(res.Value);
                            }
                        }
                    }
                }
            }
        }

        private bool IsActionBad(CodeFunction function)
        {
            return (function.StartPoint.Line + 5 < function.EndPoint.Line) || function.Access == vsCMAccess.vsCMAccessPrivate;
        }

        void SynchronousUpdate(NormalizedSnapshotSpanCollection newSpans)
        {
            lock (updateLock)
            {
                WordSpans = newSpans;

                var tempEvent = TagsChanged;
                if (tempEvent != null)
                    tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        /*public IEnumerable<ITagSpan<HighlightWordTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            //if (CurrentWord == null)
            //    yield break;

            // Hold on to a "snapshot" of the word spans and current word, so that we maintain the same
            // collection throughout
            //SnapshotSpan currentWord = CurrentWord.Value;
            NormalizedSnapshotSpanCollection wordSpans = WordSpans;

            if (spans.Count == 0 || wordSpans.Count == 0 || FCM == null)
                yield break;

            // If the requested snapshot isn't the same as the one our words are on, translate our spans to the expected snapshot 
            if (spans[0].Snapshot != wordSpans[0].Snapshot)
            {
                wordSpans = new NormalizedSnapshotSpanCollection(
                    wordSpans.Select(span => span.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive)));

                //currentWord = currentWord.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive);
            }

            // First, yield back the word the cursor is under (if it overlaps) 
            // Note that we'll yield back the same word again in the wordspans collection; 
            // the duplication here is expected. 
            //if (spans.OverlapsWith(new NormalizedSnapshotSpanCollection(currentWord)))
            //    yield return new TagSpan<HighlightWordTag>(currentWord, new HighlightWordTag());

            // Second, yield all the other words in the file 
            foreach (SnapshotSpan span in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans))
            {
                yield return new TagSpan<HighlightWordTag>(span, new HighlightWordTag());
            }
        }*/

        public IEnumerable<ITagSpan<B4ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            NormalizedSnapshotSpanCollection wordSpans = WordSpans;

            if (spans.Count == 0 || wordSpans.Count == 0 || FCM == null)
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
                yield return new TagSpan<B4ErrorTag>(span, new B4ErrorTag(/*PredefinedErrorTypeNames.SyntaxError, */(" - - -53453453")));
            }
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(B4ErrorTag))]
    internal class B4ErrorTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
                return null;

            ITextStructureNavigator textStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);
            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));

            return new B4ErrorTagger(textView, buffer, TextSearchService, textStructureNavigator, dte) as ITagger<T>;
        }
    }
}
