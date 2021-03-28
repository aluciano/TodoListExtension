using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace TodoListExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(TodoListExtensionPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(TodoWindow))]
    [ProvideOptionPage(typeof(TodoWindowToolsOptions), "TodoListExtension", "General", 101, 106, true)]
    public sealed class TodoListExtensionPackage : AsyncPackage
    {
        public const string PackageGuidString = "532d8b1c-ec6d-4db0-a60d-fa14f88f5b2b";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TodoWindowCommand.InitializeAsync(this);
        }

        #endregion
    }
}
