using System.IO;
using System.Windows.Forms;

namespace SoftwareStorageSystemUtility.Helpers
{
  public class DirectoryManager
  {
    public static string CreateIdFolder(string directoryPath, string idFolderName /*, IdItem itemToAdd*/)
    {
      var folderName = $@"{directoryPath}{idFolderName}";

      if (!Directory.Exists(folderName))
      {
        Directory.CreateDirectory(folderName);

        MessageBox.Show($@"Папка {idFolderName} создана");
      }
      else
      {
        MessageBox.Show($@"Папка {idFolderName} уже существует...");
        return string.Empty;
      }
      return folderName;
    }

    //Создать ревизию (папку = М60-ХХХХХХ-УУУУУУ-0i) и записать в файл 
    public static string CreateAuditFolder(string directoryPath, string auditFolderName)
    {
      var folderName = $@"{directoryPath}/{auditFolderName}";

      if (!Directory.Exists(folderName))
      {
        Directory.CreateDirectory(folderName);
        MessageBox.Show($@"Папка {auditFolderName} создана");
      }
      else
      {
        MessageBox.Show($@"Папка {auditFolderName} уже существует...");
        return string.Empty;
      }
      return folderName;
    }

    public static bool IsFileLocked(FileInfo file)
    {
      FileStream stream = null;
      try
      {
        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch (IOException)
      {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)
        return true;
      }
      finally
      {
        stream?.Close();
      }

      //file is not locked
      return false;
    }
  }
}
