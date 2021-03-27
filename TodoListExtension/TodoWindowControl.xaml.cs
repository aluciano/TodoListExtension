using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Collections;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace TodoListExtension
{
    [Guid("72de1eAD-a00c-4f57-bff7-57edb162d0be")]
    public class TodoWindowTaskProvider : TaskProvider
    {
        public TodoWindowTaskProvider(IServiceProvider sp)
            : base(sp)
        {
        }
    }

    /// <summary>
    /// Interaction logic for TodoWindowControl.
    /// </summary>
    public partial class TodoWindowControl : UserControl
    {
        public TodoWindow parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoWindowControl"/> class.
        /// </summary>
        public TodoWindowControl(TodoWindow window)
        {
            this.InitializeComponent();
            parent = window;
        }

        private TodoWindowTaskProvider taskProvider;

        private void CreateProvider()
        {
            if (taskProvider == null)
            {
                taskProvider = new TodoWindowTaskProvider(parent);
                taskProvider.ProviderName = "To Do";
            }
        }

        private void ClearError()
        {
            CreateProvider();
            taskProvider.Tasks.Clear();
        }
        private void ReportError(string errorText)
        {
            CreateProvider();
            var errorTask = new Task();
            errorTask.CanDelete = false;
            errorTask.Category = TaskCategory.Comments;
            errorTask.Text = errorText;

            taskProvider.Tasks.Add(errorTask);

            taskProvider.Show();

            var taskList = parent.GetVsService(typeof(SVsTaskList)) as IVsTaskList2;
            
            if (taskList == null)
            {
                return;
            }

            var guidProvider = typeof(TodoWindowTaskProvider).GUID;
            taskList.SetActiveProvider(ref guidProvider);
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtTodo.Text.Length > 0)
            {
                var item = new TodoItem(this, txtTodo.Text);
                lstTodo.Items.Add(item);

                var outputWindow = parent.GetVsService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                IVsOutputWindowPane pane;
                Guid guidGeneralPane = VSConstants.GUID_OutWindowGeneralPane;
                outputWindow.GetPane(ref guidGeneralPane, out pane);

                if (pane != null)
                {
                    pane.OutputString($"To Do item created: {item}\r\n");
                    //pane.OutputString(string.Format("To Do item created: {0}\r\n", item.ToString()));
                }
                else
                {
                    Guid paneGuid = new Guid();
                    bool visible = true;
                    bool clearWithSolution = false;
                    // Create a new pane.
                    outputWindow.CreatePane(ref paneGuid,
                                            "MyOutput",
                                            Convert.ToInt32(visible),
                                            Convert.ToInt32(clearWithSolution));

                    // Retrieve the new pane.
                    outputWindow.GetPane(ref paneGuid, out pane);

                    pane.OutputString($"To Do item created: {item}\r\n");
                }

                TrackSelection();
                CheckForErrors();
            }
        }

        public void CheckForErrors()
        {
            ClearError();

            foreach (TodoItem item in lstTodo.Items)
            {
                if (item.DueDate < DateTime.Now)
                {
                    ReportError($"To Do Item is out of date: {item}");
                }
            }
        }

        public void UpdateList(TodoItem item)
        {
            var index = lstTodo.SelectedIndex;
            lstTodo.Items.RemoveAt(index);
            lstTodo.Items.Insert(index, item);
            lstTodo.SelectedItem = index;
        }

        private void lstTodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TrackSelection();
        }

        private IVsWindowFrame frame = null;
        private SelectionContainer mySelContainer;
        private ArrayList mySelItems;

        private void TrackSelection()
        {
            if (frame == null)
            {
                var shell = parent.GetVsService(typeof(SVsUIShell)) as IVsUIShell;
                if (shell != null)
                {
                    var guidPropertyBrowser = new
                    Guid(ToolWindowGuids.PropertyBrowser);
                    shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate,
                    ref guidPropertyBrowser, out frame);
                }
            }
            if (frame != null)
            {
                frame.Show();
            }
            if (mySelContainer == null)
            {
                mySelContainer = new SelectionContainer();
            }

            mySelItems = new ArrayList();

            var selected = lstTodo.SelectedItem as TodoItem;
            if (selected != null)
            {
                mySelItems.Add(selected);
            }

            mySelContainer.SelectedObjects = mySelItems;

            ITrackSelection track = parent.GetVsService(typeof(STrackSelection))
                                    as ITrackSelection;
            if (track != null)
            {
                track.OnSelectChange(mySelContainer);
            }
        }
    }
}