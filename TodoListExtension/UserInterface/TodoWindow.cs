using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace TodoListExtension
{
    [Guid("e6cdba59-8678-49cb-b692-c995e8ece4e1")]
    public class TodoWindow : ToolWindowPane
    {
        public TodoWindow() : base(null)
        {
            this.Caption = "Minha Todo List";

            //Esta linha de código está gerando erro
            //this.BitmapImageMoniker = KnownMonikers.Save; 

            //this.BitmapResourceID = 301;
            //this.BitmapIndex = 1;

            this.Content = new TodoWindowControl(this);
        }

        internal object GetVsService(Type service)
        {
            return GetService(service);
        }
    }
}
