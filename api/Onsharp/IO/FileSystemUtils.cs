using System.IO;

namespace Onsharp.IO
{
    /// <summary>
    /// Offers some utility functions for the file system.
    /// </summary>
    public static class FileSystemUtils
    {
        /// <summary>
        /// Deletes the given file or directory silently, causing no crash.
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="isDir">If the given file is a file or a directory. True, if it is a directory</param>
        /// <returns>True on success</returns>
        public static bool DeleteSilently(string path, bool isDir = false)
        {
            try
            {
                if (!isDir)
                {
                    File.Delete(path);
                }
                else
                {
                    Directory.Delete(path);
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the contents out of the given folder and its subfolders.
        /// </summary>
        /// <param name="path">The path the target folder</param>
        /// <param name="delete">If true, the folder will be deleted after clearing</param>
        public static void ClearFolder(string path, bool delete = false)
        {
            if(!Directory.Exists(path)) return;
            bool realDelete = delete;
            foreach (string file in Directory.GetFiles(path))
            {
                if (!DeleteSilently(file))
                    realDelete = false;
            }

            foreach (string folder in Directory.GetDirectories(path))
            {
                ClearFolder(folder, delete);
            }

            if (realDelete)
                DeleteSilently(path, true);
        }
    }
}