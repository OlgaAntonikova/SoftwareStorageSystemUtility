using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Commands;
using SoftwareStorageSystemUtility.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using Microsoft.SharePoint.Client;
using SoftwareStorageSystemUtility.Helpers;
using File = System.IO.File;


namespace SoftwareStorageSystemUtility.ViewModels
{
  public class CreateIdViewModel : INotifyPropertyChanged
  {
    #region |Fields|

    private static string _connectionString 
      = @"Data Source=MySQL;Initial Catalog=OATest;Persist Security Info=True;User ID=OAUser;Password=321";
    // @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Antonikova Documents\REPOSITORYES\SoftwareStorageSystemUtility\SoftwareStorageSystemUtility\DataBase\SystemDatabase.mdf;Integrated Security=True";

    private string _id;
    private string _softwareName;
   
    private DirectionItem _directionItem;
    private CategoryItem _categoryItem;
   
    private ICollectionView _categoryItems;
    private ObservableCollection<IdItem> _idColl;
   

    #endregion

    #region |Properties|

    public string Id
    {
      get => _id;
      set
      {
        if (Equals(value, _id)) return;
        _id = value;
        OnPropertyChanged();
      }
    }

    public string SoftwareName
    {
      get => _softwareName;
      set
      {
        if (Equals(value, _softwareName)) return;
        _softwareName = value;
        OnPropertyChanged();
      }
    }

    public DirectionItem SelectedDirection
    {
      get => _directionItem;
      set
      {
        if (Equals(value, _directionItem)) return;
        _directionItem = value;
        OnPropertyChanged();
      }
    }

    public CategoryItem SelectedCategory
    {
      get => _categoryItem;
      set
      {
        if (Equals(value, _categoryItem)) return;
        _categoryItem = value;
        Id = $"M60-{SelectedDirection.Code}_{SelectedCategory.Code}-{DateTime.Today.Date:yyMMdd}0";
        SoftwareName = $"{SelectedDirection.Code}_{SelectedCategory.Code}";
        OnPropertyChanged();
      }
    }

    #endregion

    #region |Lists & Collections|

    //Список папок с идентификатором
    public ObservableCollection<IdItem> IdColl
    {
      get => _idColl;
      set
      {
        if (Equals(value, _idColl)) return;
        _idColl = value;
        OnPropertyChanged();
      }
    }
    
    public ICollectionView IdItems { get; set; }
    public ICollectionView DirectionItems { get; set; }
    public ICollectionView CategoryItems
    {
      get => _categoryItems;
      set
      {
        if (Equals(value, _categoryItems)) return;
        _categoryItems = value;
        OnPropertyChanged();
      }
    }

    #endregion

    public ICommand CreateIdCommand { get; }

    public CreateIdViewModel()
    {
      CreateIdCommand = new DelegateCommand<object>(CreateIdExecute);

      SelectFromDirection();
      SelectFromIds();
    }

    #region |Select|

    private void SelectFromIds()
    {
      SqlConnection connection = new SqlConnection(_connectionString);

      string sql = "SELECT * FROM Ids";
      var idsTable = new DataTable();
      try
      {
        SqlCommand command = new SqlCommand(sql, connection);
        var adapter = new SqlDataAdapter(command);

        //Получаем данные из БД и осуществляем привязку
        connection.Open();
        adapter.Fill(idsTable);

        var list = from drRow in idsTable.AsEnumerable()
                   select new IdItem()
                   {
                     Id = drRow.Field<Guid>("Id"),
                     IdAudit = drRow.Field<String>("IdAudit"),
                     Title = drRow.Field<String>("Title"),
                     Direction = drRow.Field<String>("Direction"),
                     Category = drRow.Field<String>("Category"),
                     SoftName = drRow.Field<String>("SoftName"),
                     ReleaseType = drRow.Field<int>("TypeRelease"),
                   };

        IdColl = new ObservableCollection<IdItem>(list);

        IdItems = CollectionViewSource.GetDefaultView(IdColl);
       // SelectedId = (IdItem)IdItems.CurrentItem;

      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        return;
      }
      finally
      {
        connection?.Close();
      }

      IdItems.CollectionChanged += IdItemsCollectionChanged;
     // IdItems.CurrentChanged += IdCurrentChanged;
    }

    private void SelectFromDirection()
    {
      string sql = "SELECT * FROM Direction";
      var directionTable = new DataTable();
      var connection = new SqlConnection(_connectionString);

      try
      {
        SqlCommand command = new SqlCommand(sql, connection);
        var adapter = new SqlDataAdapter(command);

        //Получаем данные из БД и осуществляем привязку
        connection.Open();
        adapter.Fill(directionTable);

        var list = from drRow in directionTable.AsEnumerable()
                    select new DirectionItem()
                    {
                      Id = drRow.Field<int>("Id"),
                      Code = drRow.Field<String>("Code"),
                      Text = drRow.Field<String>("Text"),
                    };

        DirectionItems = CollectionViewSource.GetDefaultView(list);
        SelectedDirection = (DirectionItem)DirectionItems.CurrentItem;
      }
      catch(Exception ex)
      {
        MessageBox.Show($"{ex.Message}");
        return;
      }
      finally
      {
        connection?.Close();
      }

      DirectionItems.CollectionChanged += DirectionItemsCollectionChanged;
      DirectionItems.CurrentChanged += DirectionCurrentChanged;
    }

    #endregion

    #region |Insert|

    public void InsertId(IdItem idItem)
    {
      SqlConnection sqlConnection = new SqlConnection(_connectionString);
      try
      {
        sqlConnection.Open();
        SqlCommand cmd = new SqlCommand($"INSERT into Ids (Direction, Category, IdAudit, SoftName, Title, TypeRelease)" +
                                        $" VALUES (N'{idItem.Direction}', N'{idItem.Category}', '{idItem.IdAudit}', N'{idItem.SoftName}', N'{idItem.Title}', '{idItem.ReleaseType}')", sqlConnection);
        cmd.ExecuteNonQuery();

      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        return;
      }
      finally
      {
        sqlConnection.Close();
      }
    }
    
    #endregion

    #region |Create|

    //Создать Id-папку. Создать файл "Перечень ревизий"-с названием Id-папки.docx в папке M60-XXXXXX-ddMMyy
    private void CreateIdExecute(object obj)
    {
      var type = 2;
      var pathsToCreate = new List<string>()
      {
        ConfigurationManager.AppSettings["PathDirectoryRelease"],
        ConfigurationManager.AppSettings["PathDirectoryPreliminary"],
      };

      var itemToAdd = new IdItem
      {
        Title = $"Title{SoftwareName}",
        IdAudit = Id,
        Category = SelectedCategory.Text,
        Direction = SelectedDirection.Text,
        SoftName = SoftwareName,
        ReleaseType = type,
      };

      foreach (var path in pathsToCreate)
      {
        var folderName = DirectoryManager.CreateIdFolder(path, Id);

        if (folderName == String.Empty) return;

        var wordApp = new Word.Application { Visible = false };

        //обьект для управления документом Form.docx
        var formByte = Properties.Resources.Form;

        if (!File.Exists(@"c:\Form M72-32_011_Rev00_Перечень ревизий.docx"))
        {
          File.WriteAllBytes(@"c:\Form M72-32_011_Rev00_Перечень ревизий.docx", formByte);
        }

        var wordDoc = wordApp.Documents.Open(@"c:\Form M72-32_011_Rev00_Перечень ревизий.docx");
        
        ReplaceWordStub("{softwareName}", SoftwareName, wordDoc);
        ReplaceWordStub("{softwareId}", itemToAdd.IdAudit, wordDoc);

        var table = wordDoc.Tables[3];
        table.Borders.Enable = 1;

        wordDoc.SaveAs($@"{path}/{itemToAdd.IdAudit}/{itemToAdd.IdAudit}.docx");
        wordApp.Visible = false;

        try
        {
          wordDoc.Close();
          wordApp.Quit();
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
          throw;
        }
      }

      //Добавить новый Id в БД
      InsertId(itemToAdd);

      IdColl.Add(itemToAdd);

      IdItems.SortDescriptions.Add(new SortDescription(nameof(IdItem.IdAudit), ListSortDirection.Ascending));
    }

    private void ReplaceWordStub(string stubToReplace, string text, Word.Document wordDocument)
    {
      var range = wordDocument.Content;
      range.Find.ClearFormatting();
      range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
    }

    #endregion
    

    #region |Changed|

    private void IdItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      IdItems.MoveCurrentToLast();
    }

    private void DirectionCurrentChanged(object sender, EventArgs e)
    {
      string sql = $"SELECT * FROM Category c where c.DirectionId = '{SelectedDirection.Id}'";
      var categoryTable = new DataTable();
      var connection = new SqlConnection(_connectionString);
      try
      {
        SqlCommand command = new SqlCommand(sql, connection);
        var adapter = new SqlDataAdapter(command);

        //Получаем данные из БД и осуществляем привязку
        connection.Open();
        adapter.Fill(categoryTable);

        var list = (from drRow in categoryTable.AsEnumerable()
                    select new CategoryItem()
                    {
                      Id = drRow.Field<int>("Id"),
                      Code = drRow.Field<String>("Code"),
                      Text = drRow.Field<String>("Text"),

                    }).ToList();

        CategoryItems = CollectionViewSource.GetDefaultView(list);

        SelectedCategory = (CategoryItem)CategoryItems.CurrentItem;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      finally
      {
        connection.Close();
      }
    }

    private void DirectionItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      DirectionItems.MoveCurrentToLast();
    }

    #endregion


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
