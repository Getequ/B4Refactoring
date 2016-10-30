using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using FastColoredTextBoxNS;

namespace Getequ.B4Refactoring.Windows
{
    using Domain;
    using Models;

    public partial class SelectElementsForm : Form
    {
        ControllerStinck _stinck;
        FastColoredTextBox _code;
        IEnumerable<CodeFunctionWrapper> _funcs;

        public SelectElementsForm()
        {
            InitializeComponent();

            _code = CreateEditor(codePanel, "code");
            _code.Tag = new List<string> { "ActionResult", "IDomainService", "BaseParams", "IDataTransaction",
                                           "JsonListResult", "JsonGetResult", "JsonNetResult", "IRepository" };
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

            var func = _funcs.First(x => x.Name == funcName);
            _code.Text = string.Join(Environment.NewLine, func.Code.Select(x => x.Ind(-2)));
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
}
