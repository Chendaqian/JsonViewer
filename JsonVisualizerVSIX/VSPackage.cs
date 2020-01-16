using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using Task = System.Threading.Tasks.Task;

namespace JsonVisualizerVSIX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(VSPackage.PackageGuidString)]
    //[ProvideAutoLoad(cmdUiContextGuid: VSConstants.UICONTEXT.Debugging_string)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class VSPackage : AsyncPackage
    {
        /// <summary>
        /// VSPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "c615cf3e-791d-4304-a21b-3202589b7f03";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSPackage"/> class.
        /// </summary>
        public VSPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            List<string> PAYLOAD_FILE_NAMES = new List<string>()
                {
                    "Json.Viewer.dll",
                    "ICSharpCode.TextEditorEx.dll",
                    "ICSharpCode.TextEditorEx.dll",
                    "JsonVisualizerVSIX.dll",
                    "Newtonsoft.Json.dll",
                };

            string sourceFolderFullName;
            string destinationFolderFullName;
            IVsShell shell;
            object documentsFolderFullNameObject = null;
            string documentsFolderFullName;

            try
            {
                // The Visualizer dll is in the same folder than the package because its project is added as reference to this project,
                // so it is included inside the .vsix file. We only need to deploy it to the correct destination folder.
                sourceFolderFullName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                // Get the destination folder for visualizers
                shell = await base.GetServiceAsync(typeof(SVsShell)) as IVsShell;
                shell.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out documentsFolderFullNameObject);
                documentsFolderFullName = documentsFolderFullNameObject.ToString();
                destinationFolderFullName = Path.Combine(documentsFolderFullName, "Visualizers");

                foreach (string payload in PAYLOAD_FILE_NAMES)
                {
                    string sourceFileFullName = Path.Combine(sourceFolderFullName, payload);
                    string destinationFileFullName = Path.Combine(destinationFolderFullName, payload);

                    CopyFileIfNewerVersion(sourceFileFullName, destinationFileFullName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void CopyFileIfNewerVersion(string sourceFileFullName, string destinationFileFullName)
        {
            FileVersionInfo destinationFileVersionInfo;
            FileVersionInfo sourceFileVersionInfo;
            bool copy = false;

            if (File.Exists(destinationFileFullName))
            {
                sourceFileVersionInfo = FileVersionInfo.GetVersionInfo(sourceFileFullName);
                destinationFileVersionInfo = FileVersionInfo.GetVersionInfo(destinationFileFullName);
                if (sourceFileVersionInfo.FileMajorPart > destinationFileVersionInfo.FileMajorPart)
                {
                    copy = true;
                }
                else if (sourceFileVersionInfo.FileMajorPart == destinationFileVersionInfo.FileMajorPart
                   && sourceFileVersionInfo.FileMinorPart > destinationFileVersionInfo.FileMinorPart)
                {
                    copy = true;
                }
            }
            else
            {
                // First time
                copy = true;
            }

            if (copy)
            {
                File.Copy(sourceFileFullName, destinationFileFullName, true);
            }
        }

        #endregion Package Members
    }
}