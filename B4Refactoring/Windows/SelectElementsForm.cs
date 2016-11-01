using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using FastColoredTextBoxNS;

namespace Getequ.B4Refactoring.Windows
{
    using DiffMergeStuffs;

    using Domain;
    using Models;

    public partial class SelectElementsForm : Form
    {
        int updating;
        Style greenStyle;
        Style redStyle;

        ControllerStinck _stinck;
        
        FastColoredTextBox _code;  // source method's code
        FastColoredTextBox _clear; // converted code

        IEnumerable<CodeFunctionWrapper> _funcs;    // detected methods
        List<Domain.ControllerDomain.MethodConvertResult> _convertedCode; // converted code for all sticky methods

        public SelectElementsForm()
        {
            InitializeComponent();

            _code = CreateEditor(codePanel, "code");
            _code.Tag = new List<string> { "ActionResult", "IDomainService", "BaseParams", "IDataTransaction",
                                           "JsonListResult", "JsonGetResult", "JsonNetResult", "IRepository" };

            _clear = CreateEditor(clearPanel, "clear");
            _clear.Tag = new List<string> { "IDataResult", "IDomainService", "BaseParams", "IDataTransaction",
                                            "ListDataResult", "BaseDataResult", "IRepository" };

            greenStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Lime)));
            redStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Red)));
        }

        FastColoredTextBox CreateEditor(Control panel, string name)
        {
            var editor = new FastColoredTextBox();
            editor.Name = "fctb" + name;
            editor.Left = 0;
            editor.Top = 0;
            editor.Language = FastColoredTextBoxNS.Language.Custom;
            editor.Width = panel.ClientSize.Width;
            editor.Height = panel.ClientSize.Height;
            editor.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            editor.BorderStyle = BorderStyle.FixedSingle;
            editor.Font = new Font("Consolas", 10F);
            editor.ReadOnly = true;
            editor.CharHeight = 15;
            editor.CharWidth = 8;
            editor.Tag = new List<string>();

            TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
            TextStyle BoldStyle = new TextStyle(new SolidBrush(Color.FromArgb(43, 145, 175)), null, FontStyle.Regular);
            TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
            TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
            TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
            TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
            TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
            MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

            editor.TextChanged += (s, e) =>
            {
                if (((FastColoredTextBox)s).Language == FastColoredTextBoxNS.Language.Custom)
                {
                    ((FastColoredTextBox)s).LeftBracket = '(';
                    ((FastColoredTextBox)s).RightBracket = ')';
                    ((FastColoredTextBox)s).LeftBracket2 = '\x0';
                    ((FastColoredTextBox)s).RightBracket2 = '\x0';
                    //clear style of changed range
                    e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);

                    //string highlighting
                    e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
                    //comment highlighting
                    e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
                    e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
                    e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
                    //number highlighting
                    e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
                    //attribute highlighting
                    e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
                    //class name highlighting
                    e.ChangedRange.SetStyle(BoldStyle, @"\b(" + string.Join("|", ((IEnumerable<string>)((FastColoredTextBox)s).Tag)) + @")\b");
                    //keyword highlighting
                    e.ChangedRange.SetStyle(BlueStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|var|where|yield)\b|#region\b|#endregion\b");

                    //clear folding markers
                    e.ChangedRange.ClearFoldingMarkers();

                    //set folding markers
                    e.ChangedRange.SetFoldingMarkers("{", "}");//allow to collapse brackets block
                    e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");//allow to collapse #region blocks
                    e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");//allow to collapse comment block
                }
            };

            panel.Controls.Add(editor);
            return editor;
        }

        void UpdateScroll(FastColoredTextBox tb, int vPos, int curLine)
        {
            if (updating > 0)
                return;
            //
            BeginUpdate();
            //
            if (vPos <= tb.VerticalScroll.Maximum)
            {
                tb.VerticalScroll.Value = vPos;
                tb.UpdateScrollbars();
            }

            if (curLine < tb.LinesCount)
                tb.Selection = new Range(tb, 0, curLine, 0, curLine);
            //
            EndUpdate();
        }

        private void EndUpdate()
        {
            updating--;
        }

        private void BeginUpdate()
        {
            updating++;
        }

        private void Process(Lines lines)
        {
            foreach (var line in lines)
            {
                switch (line.state)
                {
                    case DiffType.None:
                        _code.AppendText(line.line + Environment.NewLine);
                        _clear.AppendText(line.line + Environment.NewLine);
                        break;
                    case DiffType.Inserted:
                        _code.AppendText(Environment.NewLine);
                        _clear.AppendText(line.line + Environment.NewLine, greenStyle);
                        break;
                    case DiffType.Deleted:
                        _code.AppendText(line.line + Environment.NewLine, redStyle);
                        _clear.AppendText(Environment.NewLine);
                        break;
                }
                if (line.subLines != null)
                    Process(line.subLines);
            }
        }

        public void SetWindowData(ControllerStinck controller, RefactorTarget target, string targetName = null)
        {
            _stinck = controller.Clone();

            clbViewModel.Items.Clear();
            clbService.Items.Clear();

            foreach (var vmAction in controller.ViewModelMethods.Union(controller.BadPublicMethods))
            {
                var check = (target == RefactorTarget.ViewModelAction && targetName == vmAction.Name) ||
                            (target == RefactorTarget.BaseViewModel && (vmAction.Name == "Get" || vmAction.Name == "List")) ||
                            (target == RefactorTarget.ViewModelActions || target == RefactorTarget.Controller);

                clbViewModel.Items.Add(vmAction.Name, check);
            }

            foreach (var svcAction in controller.ServiceMethods)
            {
                var check = (target == RefactorTarget.ServiceAction && targetName == svcAction.Name) ||
                            (target == RefactorTarget.ServiceActions || target == RefactorTarget.Controller);

                clbService.Items.Add(svcAction.Name, check);
            }
            
            _funcs = _stinck.ViewModelMethods.Union(_stinck.BadPublicMethods).Union(_stinck.ServiceMethods);

            var stickProcessor = new ControllerDomain(controller.ProjectItem.ContainingProject);
            stickProcessor.FindControllerRelativeClasses(controller.Name);
            var result = stickProcessor.GenerateRefactoringCode(_stinck);

            try
            {
                 _convertedCode = result.Modification.ConvertedMethods;
            }
            catch(Exception e)
            { 
                MessageBox.Show(e.Message + "     " + e.StackTrace);
            }
        }

        public ControllerStinck GetData()
        {
            return _stinck;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            
            var vmNames = clbViewModel.CheckedItems.OfType<string>().ToArray();
            var vmActions = _stinck.ViewModelMethods.Union(_stinck.ServiceMethods).Where(a => vmNames.Contains(a.Name)).ToList();

            var svcNames = clbService.CheckedItems.OfType<string>().ToArray();
            var svcActions = _stinck.ViewModelMethods.Union(_stinck.ServiceMethods).Where(a => svcNames.Contains(a.Name)).ToList();

            _stinck.ViewModelMethods = vmActions;
            _stinck.ServiceMethods = svcActions;

            Close();
        }

        private void btnVM_Click(object sender, EventArgs e)
        {
            var indices = clbService.SelectedIndices;
            foreach (int index in indices)
            {
                clbViewModel.Items.Add(clbService.Items[index], clbService.GetItemChecked(index));
            }
            foreach (int index in indices.OfType<int>().OrderByDescending(x => x))
            {
                clbService.Items.RemoveAt(index);
            }
        }

        private void btnSvc_Click(object sender, EventArgs e)
        {
            var names = clbViewModel.SelectedItems.OfType<string>().ToArray();
            if (names.Any(x => x == "List" || x == "Get"))
            {
                MessageBox.Show("Действия Get, List нельзя выносить в сервис");
                return;
            }

            var indices = clbViewModel.SelectedIndices;
            foreach (int index in indices)
            {
                clbService.Items.Add(clbViewModel.Items[index], clbViewModel.GetItemChecked(index));
            }
            foreach (int index in indices.OfType<int>().OrderByDescending(x => x))
            {
                clbViewModel.Items.RemoveAt(index);
            }
        }

        private void SetCodeText(string funcName)
        {
            if (string.IsNullOrEmpty(funcName)) return;

            var oldFunc = _funcs.First(x => x.Name == funcName);
            _code.Text = string.Join(Environment.NewLine, oldFunc.Code.Select(x => x.Ind(-2)));

            var newFunc = _convertedCode.FirstOrDefault(x => x.Name == funcName);
            if (newFunc != null)
                _clear.Text = string.Join(Environment.NewLine, newFunc.Code.Select(x => x.Ind(-2)));

            if (showDiff.Checked)
            {
                Cursor = Cursors.WaitCursor;

                var source1 = Lines.Load(_code.Text);
                var source2 = Lines.Load(_clear.Text);

                _code.Clear();
                _clear.Clear();

                source1.Merge(source2);

                BeginUpdate();

                Process(source1);

                EndUpdate();

                Cursor = Cursors.Default;
            }
        }

        private void clbViewModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCodeText((string)clbViewModel.SelectedItem);
        }

        private void clbService_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCodeText((string)clbService.SelectedItem);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbViewModel.Items.Count; i++)
            {
                clbViewModel.SetItemChecked(i, true);
            }
            for (int i = 0; i < clbService.Items.Count; i++)
            {
                clbService.SetItemChecked(i, true);
            }
        }

        private void btnDeselect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbViewModel.Items.Count; i++)
            {
                clbViewModel.SetItemChecked(i, false);
            }
            for (int i = 0; i < clbService.Items.Count; i++)
            {
                clbService.SetItemChecked(i, false);
            }
        }
    }


    #region Merge stuffs

    namespace DiffMergeStuffs
    {
        public class SimpleDiff<T>
        {
            private IList<T> _left;
            private IList<T> _right;
            private int[,] _matrix;
            private bool _matrixCreated;
            private int _preSkip;
            private int _postSkip;

            private Func<T, T, bool> _compareFunc;

            public SimpleDiff(IList<T> left, IList<T> right)
            {
                _left = left;
                _right = right;

                InitializeCompareFunc();
            }

            public event EventHandler<DiffEventArgs<T>> LineUpdate;

            public TimeSpan ElapsedTime { get; private set; }

            /// <summary>
            /// This is the sole public method and it initializes
            /// the LCS matrix the first time it's called, and 
            /// proceeds to fire a series of LineUpdate events
            /// </summary>
            public void RunDiff()
            {
                if (!_matrixCreated)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    CalculatePreSkip();
                    CalculatePostSkip();
                    CreateLCSMatrix();
                    sw.Stop();
                    this.ElapsedTime = sw.Elapsed;
                }

                for (int i = 0; i < _preSkip; i++)
                {
                    FireLineUpdate(DiffType.None, i, -1);
                }

                int totalSkip = _preSkip + _postSkip;
                ShowDiff(_left.Count - totalSkip, _right.Count - totalSkip);

                int leftLen = _left.Count;
                for (int i = _postSkip; i > 0; i--)
                {
                    FireLineUpdate(DiffType.None, leftLen - i, -1);
                }
            }

            /// <summary>
            /// This method is an optimization that
            /// skips matching elements at the end of the 
            /// two arrays being diff'ed.
            /// Care's taken so that this will never
            /// overlap with the pre-skip.
            /// </summary>
            private void CalculatePostSkip()
            {
                int leftLen = _left.Count;
                int rightLen = _right.Count;
                while (_postSkip < leftLen && _postSkip < rightLen &&
                       _postSkip < (leftLen - _preSkip) &&
                       _compareFunc(_left[leftLen - _postSkip - 1], _right[rightLen - _postSkip - 1]))
                {
                    _postSkip++;
                }
            }

            /// <summary>
            /// This method is an optimization that
            /// skips matching elements at the start of
            /// the arrays being diff'ed
            /// </summary>
            private void CalculatePreSkip()
            {
                int leftLen = _left.Count;
                int rightLen = _right.Count;
                while (_preSkip < leftLen && _preSkip < rightLen &&
                       _compareFunc(_left[_preSkip], _right[_preSkip]))
                {
                    _preSkip++;
                }
            }

            /// <summary>
            /// This traverses the elements using the LCS matrix
            /// and fires appropriate events for added, subtracted, 
            /// and unchanged lines.
            /// It's recursively called till we run out of items.
            /// </summary>
            /// <param name="leftIndex"></param>
            /// <param name="rightIndex"></param>
            private void ShowDiff(int leftIndex, int rightIndex)
            {
                if (leftIndex > 0 && rightIndex > 0 &&
                    _compareFunc(_left[_preSkip + leftIndex - 1], _right[_preSkip + rightIndex - 1]))
                {
                    ShowDiff(leftIndex - 1, rightIndex - 1);
                    FireLineUpdate(DiffType.None, _preSkip + leftIndex - 1, -1);
                }
                else
                {
                    if (rightIndex > 0 &&
                        (leftIndex == 0 ||
                         _matrix[leftIndex, rightIndex - 1] >= _matrix[leftIndex - 1, rightIndex]))
                    {
                        ShowDiff(leftIndex, rightIndex - 1);
                        FireLineUpdate(DiffType.Inserted, -1, _preSkip + rightIndex - 1);
                    }
                    else if (leftIndex > 0 &&
                             (rightIndex == 0 ||
                              _matrix[leftIndex, rightIndex - 1] < _matrix[leftIndex - 1, rightIndex]))
                    {
                        ShowDiff(leftIndex - 1, rightIndex);
                        FireLineUpdate(DiffType.Deleted, _preSkip + leftIndex - 1, -1);
                    }
                }

            }

            /// <summary>
            /// This is the core method in the entire class,
            /// and uses the standard LCS calculation algorithm.
            /// </summary>
            private void CreateLCSMatrix()
            {
                int totalSkip = _preSkip + _postSkip;
                if (totalSkip >= _left.Count || totalSkip >= _right.Count)
                    return;

                // We only create a matrix large enough for the
                // unskipped contents of the diff'ed arrays
                _matrix = new int[_left.Count - totalSkip + 1, _right.Count - totalSkip + 1];

                for (int i = 1; i <= _left.Count - totalSkip; i++)
                {
                    // Simple optimization to avoid this calculation
                    // inside the outer loop (may have got JIT optimized 
                    // but my tests showed a minor improvement in speed)
                    int leftIndex = _preSkip + i - 1;

                    // Again, instead of calculating the adjusted index inside
                    // the loop, I initialize it under the assumption that
                    // incrementing will be a faster operation on most CPUs
                    // compared to addition. Again, this may have got JIT
                    // optimized but my tests showed a minor speed difference.
                    for (int j = 1, rightIndex = _preSkip + 1; j <= _right.Count - totalSkip; j++, rightIndex++)
                    {
                        _matrix[i, j] = _compareFunc(_left[leftIndex], _right[rightIndex - 1])
                                            ? _matrix[i - 1, j - 1] + 1
                                            : Math.Max(_matrix[i, j - 1], _matrix[i - 1, j]);
                    }
                }

                _matrixCreated = true;
            }

            private void FireLineUpdate(DiffType diffType, int leftIndex, int rightIndex)
            {
                var local = this.LineUpdate;

                if (local == null)
                    return;

                T lineValue = leftIndex >= 0 ? _left[leftIndex] : _right[rightIndex];

                local(this, new DiffEventArgs<T>(diffType, lineValue, leftIndex, rightIndex));
            }

            private void InitializeCompareFunc()
            {
                // Special case for String types
                if (typeof(T) == typeof(String))
                {
                    _compareFunc = StringCompare;
                }
                else
                {
                    _compareFunc = DefaultCompare;
                }
            }

            /// <summary>
            /// This comparison is specifically
            /// for strings, and was nearly thrice as 
            /// fast as the default comparison operation.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            private bool StringCompare(T left, T right)
            {
                return Object.Equals(left, right);
            }

            private bool DefaultCompare(T left, T right)
            {
                return left.Equals(right);
            }
        }

        [Flags]
        public enum DiffType
        {
            None = 0,
            Inserted = 1,
            Deleted = 2
        }

        public class DiffEventArgs<T> : EventArgs
        {
            public DiffType DiffType { get; set; }

            public T LineValue { get; private set; }
            public int LeftIndex { get; private set; }
            public int RightIndex { get; private set; }

            public DiffEventArgs(DiffType diffType, T lineValue, int leftIndex, int rightIndex)
            {
                this.DiffType = diffType;
                this.LineValue = lineValue;
                this.LeftIndex = leftIndex;
                this.RightIndex = rightIndex;
            }
        }

        /// <summary>
        /// Line of file
        /// </summary>
        public class Line
        {
            /// <summary>
            /// Source string
            /// </summary>
            public readonly string line;

            /// <summary>
            /// Inserted strings
            /// </summary>
            public Lines subLines;

            /// <summary>
            /// Line state
            /// </summary>
            public DiffType state;

            public Line(string line)
            {
                this.line = line;
            }

            /// <summary>
            /// Equals
            /// </summary>
            public override bool Equals(object obj)
            {
                return Object.Equals(line, ((Line)obj).line);
            }

            public static bool operator ==(Line line1, Line line2)
            {
                return Object.Equals(line1.line, line2.line);
            }

            public static bool operator !=(Line line1, Line line2)
            {
                return !Object.Equals(line1.line, line2.line);
            }

            public override string ToString()
            {
                return line;
            }
        }

        /// <summary>
        /// File as list of lines
        /// </summary>
        public class Lines : List<Line>, IEquatable<Lines>
        {
            //эта строка нужна для хранения строк, вставленных в самом начале, до первой строки исходного файла
            private Line fictiveLine = new Line("===fictive line===") { state = DiffType.Deleted };

            public Lines()
            {
            }


            public Lines(int capacity)
                : base(capacity)
            {
            }

            public Line this[int i]
            {
                get
                {
                    if (i == -1) return fictiveLine;
                    return base[i];
                }

                set
                {
                    if (i == -1) fictiveLine = value;
                    base[i] = value;
                }
            }

            /// <summary>
            /// Load from file
            /// </summary>
            public static Lines Load(string codeText, Encoding enc = null)
            {
                Lines lines = new Lines();
                foreach (var line in codeText.Split(new string[]{"\r\n"}, StringSplitOptions.None))
                    lines.Add(new Line(line));

                return lines;
            }

            /// <summary>
            /// Merge lines
            /// </summary>
            public void Merge(Lines lines)
            {
                SimpleDiff<Line> diff = new SimpleDiff<Line>(this, lines);
                int iLine = -1;

                diff.LineUpdate += (o, e) =>
                {
                    if (e.DiffType == DiffType.Inserted)
                    {
                        if (this[iLine].subLines == null)
                            this[iLine].subLines = new Lines();
                        e.LineValue.state = DiffType.Inserted;
                        this[iLine].subLines.Add(e.LineValue);
                    }
                    else
                    {
                        iLine++;
                        this[iLine].state = e.DiffType;
                        if (iLine > 0 &&
                            this[iLine - 1].state == DiffType.Deleted &&
                            this[iLine - 1].subLines == null &&
                            e.DiffType == DiffType.None)
                            this[iLine - 1].subLines = new Lines();
                    }
                };
                //запускаем алгоритм нахождения максимальной подпоследовательности (LCS)
                diff.RunDiff();
            }

            /// <summary>
            /// Clone
            /// </summary>
            public Lines Clone()
            {
                Lines result = new Lines(this.Count);
                foreach (var line in this)
                    result.Add(new Line(line.line));

                return result;
            }

            /// <summary>
            /// Is lines equal?
            /// </summary>
            public bool Equals(Lines other)
            {
                if (Count != other.Count)
                    return false;
                for (int i = 0; i < Count; i++)
                    if (this[i] != other[i])
                        return false;
                return true;
            }

            /// <summary>
            /// Transform tree to list
            /// </summary>
            public Lines Expand()
            {
                return Expand(-1, Count - 1);
            }

            /// <summary>
            /// Transform tree to list
            /// </summary>
            public Lines Expand(int from, int to)
            {
                Lines result = new Lines();
                for (int i = from; i <= to; i++)
                {
                    if (this[i].state != DiffType.Deleted)
                        result.Add(this[i]);
                    if (this[i].subLines != null)
                        result.AddRange(this[i].subLines.Expand());
                }

                return result;
            }
        }

        /// <summary>
        /// Строка, содержащая несколько конфликтных версий
        /// </summary>
        public class ConflictedLine : Line
        {
            public readonly Lines version1;
            public readonly Lines version2;

            public ConflictedLine(Lines version1, Lines version2)
                : base("?")
            {
                this.version1 = version1;
                this.version2 = version2;
            }
        }
    }
    #endregion
}
