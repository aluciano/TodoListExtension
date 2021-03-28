using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;

namespace TodoListExtension
{
    public class TodoItem
    {
        private TodoWindowControl parent;
        private string name;
        private DateTime dueDate;

        public TodoItem(TodoWindowControl control, string itemName)
        {
            parent = control;
            name = itemName;
            dueDate = DateTime.Now;

            double daysAhead = 0;
            IVsPackage package = parent.parent.Package as IVsPackage;

            if (package != null)
            {
                object obj;
                package.GetAutomationObject("TodoListExtension.General", out obj);

                TodoWindowToolsOptions options = obj as TodoWindowToolsOptions;

                if (options != null)
                {
                    daysAhead = options.DaysAhead;
                }
            }

            dueDate = dueDate.AddDays(daysAhead);
        }

        [Description("Name of the Todo item.")]
        [Category("Todo Fields")]
        public string Name {
            get { return name; }
            set
            {
                name = value;
                parent.UpdateList(this);
            }
        }

        [Description("Due date of the ToDo item")]
        [Category("ToDo Fields")]
        public DateTime DueDate {
            get { return dueDate; }
            set
            {
                dueDate = value;
                parent.UpdateList(this);
                parent.CheckForErrors();
            }
        }

        public override string ToString()
        {
            return $"{name} Due: {dueDate.ToShortDateString()}";
        }
    }
}
