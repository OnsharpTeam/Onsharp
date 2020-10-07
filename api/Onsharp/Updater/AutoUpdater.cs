using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using Ionic.Zip;
using Konsole;
using Newtonsoft.Json;
using Onsharp.Native;
using Onsharp.Plugins;

namespace Onsharp.Updater
{
    /// <summary>
    /// The class which manages the updating process.
    /// </summary>
    internal class AutoUpdater
    {
        /// <summary>
        /// The currently loaded domain for the plugin.
        /// </summary>
        public PluginDomain Domain { get; }

        private readonly string _tmpZip;
        private readonly string _tmpDir;
        private readonly ManualResetEvent _lock;
        private readonly WebClient _client;
        private readonly ProgressBar _progress;
        private readonly string _changelog;

        internal AutoUpdater(PluginDomain domain)
        {
            Domain = domain;
            string tmpName = Guid.NewGuid().ToString().Replace("-", "");
            _tmpDir = Path.Combine(Bridge.TempPath, tmpName);
            Directory.CreateDirectory(_tmpDir);
            _tmpZip = Path.Combine(Bridge.TempPath, tmpName + ".zip");
            _changelog = domain.UpdatingData.Changelog;
            _lock = new ManualResetEvent(false);
            _client = new WebClient();
            _client.DownloadProgressChanged += DownloadProgress;
            _client.DownloadFileCompleted += DownloadCompleted;
            _progress = new ProgressBar(100);
        }

        /// <summary>
        /// Starts the updating process.
        /// </summary>
        internal void Start()
        {
            _client.DownloadFileAsync(new Uri(Domain.UpdatingData.Files), _tmpZip);
            _lock.WaitOne();
        }

        /// <summary>
        /// Finishes the process and unlocks the main thread.
        /// </summary>
        private void Finish()
        {
            _progress.Refresh(100, "Finished!");
            Bridge.Logger.Info("Update v{VER} successfully installed! {CHANGELOG}", Domain.UpdatingData.Version, 
                (string.IsNullOrEmpty(_changelog) ? "" : _changelog));
            _lock.Set();
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                _progress.Refresh(0, "Extracting...");
                using (ZipFile zip = ZipFile.Read(_tmpZip))
                {
                    zip.ExtractProgress += (o, args) =>
                    {
                        if (args.TotalBytesToTransfer > 0)
                        {
                            _progress.Refresh(Convert.ToInt32(100 * args.BytesTransferred / args.TotalBytesToTransfer), "Extracting...");
                        }
                    };
                    zip.ExtractAll(_tmpDir, ExtractExistingFileAction.OverwriteSilently);
                }

                DeleteFileSilently(_tmpZip);
                _progress.Refresh(0, "Installing...");
                DeleteFileSilently(Domain.Path);
                string[] files = Directory.GetFiles(_tmpDir);
                int current = 0;
                foreach (string file in files)
                {
                    if(Path.GetExtension(file).ToLower() != ".dll") continue;
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string filePath =
                        Path.Combine(fileName == Domain.UpdatingData.PluginFile ? Bridge.PluginsPath : Bridge.LibsPath,
                            fileName + ".dll");
                    File.Move(file, filePath, true);
                    current++;
                    _progress.Refresh(current / files.Length, "Installing...");
                }
            
                DeleteDirectorySilently(_tmpDir);
                Finish();
            }
            catch (Exception ex)
            {
                Bridge.Logger.Error(ex, "The updater ran into a problem!");
                _lock.Set();
            }
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            _progress.Refresh(e.ProgressPercentage, "Downloading...");
        }

        private static void DeleteFileSilently(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                if (Bridge.Config.IsDebug)
                {
                    Bridge.Logger.Error(ex, "An error occurred while deleting file {PATH} silently!", path);
                }
            }
        }

        private static void DeleteDirectorySilently(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                if (Bridge.Config.IsDebug)
                {
                    Bridge.Logger.Error(ex, "An error occurred while deleting directory {PATH} silently!", path);
                }
            }
        }

        /// <summary>
        /// Retrieves the updating data from the given url.
        /// </summary>
        /// <param name="url">The url containing the updating data</param>
        /// <returns>The updating data or null</returns>
        internal static UpdatingData RetrieveData(string url)
        {
            try
            {
                using WebClient client = new WebClient();
                return JsonConvert.DeserializeObject<UpdatingData>(client.DownloadString(url));
            }
            catch (Exception ex)
            {
                Bridge.Logger.Error(ex, "An error occurred while reading updating data from {URL}!", url);
                return null;
            }
        }
    }
}