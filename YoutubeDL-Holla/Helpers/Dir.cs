using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;

namespace YoutubeDL_Holla.Helpers
{
    public class Dir
    {
        public string OpenDialog()
        {
            string folderPath = null;
            if (CommonFileDialog.IsPlatformSupported)
            {
                using (var cof = new CommonOpenFileDialog())
                {
                    cof.EnsureReadOnly = true;
                    cof.IsFolderPicker = true;
                    cof.AllowNonFileSystemItems = false;
                    cof.Multiselect = false;
                    cof.EnsurePathExists = true;
                    cof.EnsureFileExists = false;
                    cof.DefaultFileName = Directory.GetCurrentDirectory();
                    cof.Title = "Select The Folder To Save Files";
                    cof.InitialDirectory = Directory.GetCurrentDirectory();
                    if (cof.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        folderPath = cof.FileName;
                    }
                }
            }

            return folderPath;
        }

        public bool HasAccessToFolder(string folderPath)
        {
            //https://stackoverflow.com/questions/1410127/c-sharp-test-if-user-has-write-access-to-a-folder
            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public bool CheckFolderPath(string folderPath)
        {
            if (folderPath.Length > 0)
            {
                return HasAccessToFolder(folderPath);
            }
            return false;
        }
    }
}
