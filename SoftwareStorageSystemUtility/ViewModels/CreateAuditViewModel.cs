using SoftwareStorageSystemUtility.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.SharePoint.Client;
using Prism.Commands;
using SoftwareStorageSystemUtility.Helpers;
using MessageBox = System.Windows.MessageBox;
using Word = Microsoft.Office.Interop.Word;
using File = System.IO.File;
using Prism.Interactivity.InteractionRequest;
using SoftwareStorageSystemUtility.ViewModels.Dialogs.Notifications;
using System.Diagnostics;

namespace SoftwareStorageSystemUtility.ViewModels
{
  public class CreateAuditViewModel : INotifyPropertyChanged
  {
    #region |Fields|

    private static string _connectionString
      = @"Data Source=MySQL;Initial Catalog=OATest;Persist Security Info=True;User ID=OAUser;Password=321";

    private string _pathDirectory;
    private string _audit;
    private string _version;
    private string _comment;
    private string _plata;
    private string _player;
    private string _customer;
    private string _order;
    private string _emailRecipient;

    private bool _isCreateAuditSelected = false;
    private bool _isEnabledCreateAudit = false;

    private IdItem _idItem;
    private AuditItem _selectedAudit;
    private DirectionItem _directionItem;
    private CategoryItem _categoryItem;

    private ICollectionView _auditItems;
    private ICollectionView _idItems;
    private ICollectionView _categoryItems;

    private ObservableCollection<AuditItem> _auditColl;
    private ObservableCollection<IdItem> _idColl;

    private List<string> _players;
    private List<string> _custumers;
    private List<string> _recipients;

    #endregion

    #region |Properties|

    public IdItem SelectedId
    {
      get => _idItem;
      set
      {
        if (Equals(value, _idItem)) return;
        _idItem = value;
        OnPropertyChanged();
      }
    }

    public string Audit
    {
      get => _audit;
      set
      {
        if (Equals(value, _audit)) return;
        _audit = value;
        OnPropertyChanged();
      }
    }

    public string Version
    {
      get => _version;
      set
      {
        if (Equals(value, _version)) return;
        _version = value;
        OnPropertyChanged();
      }
    }

    public string Comment
    {
      get => _comment;
      set
      {
        if (Equals(value, _comment)) return;
        _comment = value;
        OnPropertyChanged();
      }
    }

    public string Plata
    {
      get => _plata;
      set
      {
        if (Equals(value, _plata)) return;
        _plata = value;
        OnPropertyChanged();
      }
    }

    public string Player
    {
      get { return _player; }
      set
      {
        if (Equals(value, _player)) return;
        _player = value;
        OnPropertyChanged();

      }
    }

    public string Customer
    {
      get { return _customer; }
      set
      {
        if (Equals(value, _customer)) return;
        _customer = value;
        OnPropertyChanged();

      }
    }

    public string EmailRecipient
    {
      get { return _emailRecipient; }
      set
      {
        if (Equals(value, _emailRecipient)) return;
        _emailRecipient = value;
        OnPropertyChanged();

      }
    }

    //Список получателей, введенных в поле Отправить Email через , 
    public List<String> Recipients
    {
      get => _recipients;
      set
      {
        if (Equals(value, _recipients)) return;
        _recipients = value;
        OnPropertyChanged();
      }
    }

    //Список заказчиков, введенных в поле заказчик через , 
    public List<String> Customers
    {
      get => _custumers;
      set
      {
        if (Equals(value, _custumers)) return;
        _custumers = value;
        OnPropertyChanged();
      }
    }

    //Список заказчиков, введенных в поле заказчик через , 
    public List<String> Players
    {
      get => _players;
      set
      {
        if (Equals(value, _players)) return;
        _players = value;
        OnPropertyChanged();
      }
    }

    public string Order
    {
      get => _order;
      set
      {
        if (Equals(value, _order)) return;
        _order = value;
        OnPropertyChanged();
      }
    }

    public bool IsEnabledCreateAudit
    {
      get { return _isEnabledCreateAudit; }
      set
      {
        if (Equals(value, _isEnabledCreateAudit)) return;
        _isEnabledCreateAudit = value;
        OnPropertyChanged();
      }
    }

    public bool IsCreateAuditSelected
    {
      get { return _isCreateAuditSelected; }
      set
      {
        if (value != _isCreateAuditSelected)
        {
          _isCreateAuditSelected = value;
          OnPropertyChanged("IsCreateAuditSelected");
        }
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
        OnPropertyChanged();
      }
    }

    #endregion

    #region |Lists & Collections|

    public ObservableCollection<AuditItem> AuditColl
    {
      get => _auditColl;
      set
      {
        if (Equals(value, _auditColl)) return;
        _auditColl = value;
        OnPropertyChanged();
      }
    }

    public AuditItem SelectedAudit
    {
      get => _selectedAudit;
      set
      {
        if (Equals(value, _selectedAudit)) return;
        _selectedAudit = value;
        OnPropertyChanged();
      }
    }

    public ICollectionView AuditItems
    {
      get => _auditItems;
      set
      {
        if (Equals(value, _auditItems)) return;
        _auditItems = value;
        OnPropertyChanged();
      }
    }

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

    public ICollectionView IdItems //{ get; set; }
    {
      get => _idItems;
      set
      {
        if (Equals(value, _idItems)) return;
        _idItems = value;
        OnPropertyChanged();
      }
    }

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

    public ICommand CreateAuditCommand { get; }
    public ICommand CopyCommand { get; }

    public ICommand AddFileCommand { get; }

    public ICommand ApplyFilterCommand { get; }

    public ICommand ResetFilterCommand { get; }

    public ICommand EditAuditCommand { get; }

    public ICommand DeleteLastAuditCommand { get; }

    public ICommand OpenFolderCommand { get; }

    public CreateAuditViewModel()
    {
      Version = "0.0.0.0";

      CreateAuditCommand = new DelegateCommand<object>(CreateAuditExecute);
      CopyCommand = new DelegateCommand(CopyAuditExecute);
      AddFileCommand = new DelegateCommand(AddFileExecute);
      DeleteLastAuditCommand = new DelegateCommand(DeleteExecute);
      ApplyFilterCommand = new DelegateCommand(ApplyFilterExecute);
      ResetFilterCommand = new DelegateCommand(ResetFilterExecute);
      EditAuditCommand = new DelegateCommand(EditAuditExecute);

      OpenFolderCommand = new DelegateCommand(OpenFolderExecute);


      SelectFromDirection();

      SelectFromIds(null, null);

      PropertyChanged += CreateAuditPropertyChanged;

      EditAuditRequest = new InteractionRequest<EditAuditConfirmation>();
    }

    #region |Metods|

    #region |Filter|

    private void ApplyFilterExecute()
    {
      SelectFromIds(SelectedDirection.Text, SelectedCategory.Text);
      SelectFromAudits();

      int count;
      if (SelectedId != null)
      {
        if (SelectedId.Id == Guid.Empty)
        {
          var selectCurrentParentId = SelectCurrentParentId();
          if (selectCurrentParentId != null) SelectedId.Id = (Guid)selectCurrentParentId;
        }

        SelectFromAudits();
        count = AuditColl.Count;
        Audit = $"{SelectedId.IdAudit}-{count/*:00*/}";
        var lastOrDefault = AuditColl.LastOrDefault();

        Plata = lastOrDefault != null ? lastOrDefault.Plata : String.Empty;
        Player = lastOrDefault != null ? lastOrDefault.Player : String.Empty;
        Customer = lastOrDefault != null ? lastOrDefault.Customer : String.Empty;
        Order = lastOrDefault != null ? lastOrDefault.Order : String.Empty;
        EmailRecipient = lastOrDefault != null ? lastOrDefault.EmailRecipient : String.Empty;
      }
      else
      {
        count = 0;
        AuditColl = new ObservableCollection<AuditItem>();
        Audit = $"M60-XXXXXX-{DateTime.Today.Date:yyMMdd}-{count/*:00*/}";
        Plata = String.Empty;
      }

      AuditItems = CollectionViewSource.GetDefaultView(AuditColl);
      AuditItems.SortDescriptions.Add(new SortDescription(nameof(AuditItem.ReleaseSoftDate), ListSortDirection.Ascending));

      Comment = String.Empty;

    }

    private void ResetFilterExecute()
    {
      DirectionItems.MoveCurrentToFirst();
      DirectionCurrentChanged(null, null);
      SelectFromIds(null, null);
      IdCurrentChanged(null, null);
    }

    #endregion

    #region |Create|

    //Создать папку с № count+1. Дописать в файл список "Перечень ревизий"
    private void CreateAuditExecute(object obj)
    {
      var playerId = 0;
      var customerId = 0;
      var customersId = new List<int>();
      // var order = false;

      Customers = Customer.Split(',', ';').ToList<string>();

      var users = CheckCorrectUser();

      Players = Player.Split(',', ';').ToList<string>();
      

      if (EmailRecipient != null)
        Recipients = EmailRecipient.Split(',', ';').ToList<string>();
      else
        Recipients = null;

      List<FieldUserValue> customersList = new List<FieldUserValue>();

      foreach (var us in Customers)
      {
        var correctUserName = us.TrimStart(' ');
        customersList.Add(FieldUserValue.FromUser(correctUserName));
      }

      List<FieldUserValue> playersList = new List<FieldUserValue>();
      foreach (var us in Players)
      {
        var correctUserName = us.TrimStart(' ');
        playersList.Add(FieldUserValue.FromUser(correctUserName));
      }

      // TODO: Что-то накрылось и не работает проверка правильности заказа
      //if (!string.IsNullOrEmpty(Order))
      //{
      //  order = CheckCorrectOrder(Order);
      //  if (order == false)
      //  {
      //    MessageBox.Show($"Заказ № {Order} не существует. Проверьте правильность заполнения поля - Заказ:");
      //    return;
      //  }
      //}

      foreach (User user in users)
      {
        //if (!string.IsNullOrEmpty(Player) && user.Title.Contains(Player))
        //  playerId = user.Id;

        foreach (var player in playersList)
        {
          if (user.Title.Contains(player.LookupValue))
            playerId += 1;
        }

        foreach (var customer in customersList)
        {
          if (user.Title.Contains(customer.LookupValue))
            customerId += 1;
        }


        //if (!string.IsNullOrEmpty(Customer) && user.Title.Contains(Customer))
        //  customerId = user.Id;
      }
      if (!string.IsNullOrEmpty(Customer) && customerId == 0 || (customersList.Count() > 1 && customerId == 1))
      {
        if (customersList.Count() > 1)
          MessageBox.Show($"Заказчики {Customer} не найдены. Проверьте правильность заполнения поля");
        else
          MessageBox.Show($"Заказчик {Customer} не найден. Проверьте правильность заполнения поля");
        return;
      }
      if (!string.IsNullOrEmpty(Player) && playerId == 0 || (playersList.Count() > 1 && playerId == 1))
      {
        if (playersList.Count() > 1)
          MessageBox.Show($"Заказчики {Player} не найдены. Проверьте правильность заполнения поля");
        else
          MessageBox.Show($"Исполнитель {Player} не найден. Проверьте правильность заполнения поля");
        return;
      }

      _pathDirectory = SelectedId.ReleaseType == 1 ? ConfigurationManager.AppSettings["PathDirectoryRelease"] : ConfigurationManager.AppSettings["PathDirectoryPreliminary"];

      var pathD = $"{_pathDirectory}{((IdItem)SelectedId).IdAudit}";

      var pathFile = $@"{pathD}/{((IdItem)SelectedId).IdAudit}.docx";

      if (!File.Exists(pathFile))
      {
        MessageBox.Show($"Файл {pathFile} не существует");
        return;
      }

      var fileInfo = new FileInfo(pathFile);

      if (DirectoryManager.IsFileLocked(fileInfo))
      {
        MessageBox.Show($"Файл {pathFile} в данный момент доступен только для чтения. " +
                        $"Закройте выбранный документ и повторите попытку создания ревизии.");
        return;
      }

      //Создать папку с № count + 1.
      var name = DirectoryManager.CreateAuditFolder(pathD, Audit);

      if (name == string.Empty) return;

      var p = SelectCurrentParentId();

      var newItem = new AuditItem
      {
        Direction = SelectedId.Direction,
        Category = SelectedId.Category,
        ParentId = SelectCurrentParentId(), //SelectedId.Id,
        IdAudit = SelectedId.IdAudit,
        Audit = Audit,
        SoftName = SelectedId.SoftName,
        ReleaseSoftDate = new DirectoryInfo(name).CreationTime, //.ToString(/*"dd.MM.yyyy"*/),
        Version = Version,
        Comment = Comment,
        Path = $@"file://{name}/",
        State = 1,
        Plata = Plata,
        Player = Player,
        Customer = Customer,
        Order = Order,
        EmailRecipient = EmailRecipient
      };

      if (p != null) SelectedId.Id = (Guid)p;

      if (SelectedId.ReleaseType == 2)
        newItem.ReferencePreliminary = $@"file://{name}/";
      else
        newItem.ReferenceReriase = $@"file://{name}/";

      // Выгрузили новую ревизию на портал
      var res = ExportToSharePoint(newItem, playerId, customersId);

      if (res == null)
      {
        if (Directory.Exists(name))
        {
          Directory.Delete(name, true);
          if (SelectedAudit.Audit != null)
            MessageBox.Show($"{SelectedAudit.Audit} удалена");
        }
        return;
      }

      //Добавить новую ревизию в файл 
      InsertToFile(pathD, newItem);

      //Добавить новую Ревизию в БД
      InsertAudit(newItem);

      AuditColl.Add(newItem);

      var count = AuditItems.Cast<object>().Count();
      Audit = $"{((IdItem)SelectedId).IdAudit}-{count/*:00*/}";
      Comment = String.Empty;
    }

    private void InsertToFile(string path, AuditItem newItem)
    {
      var wApp = new Word.Application { Visible = false };

      //найти файл с SelectedId.Name.docx
      var wDoc = wApp.Documents.Open($@"{path}/{((IdItem)SelectedId).IdAudit}.docx");

      //Добавить ревизию в список
      var table = wDoc.Tables[3];
      table.Borders.Enable = 1;

      //Word Export
      int rowIndex = 2;

      if (AuditItems.Cast<object>().Count() + 1 >= 1)
      {
        rowIndex = AuditItems.Cast<object>().Count() + 2;
        table.Rows.Add();
      }

      table.Cell(rowIndex, 1).Range.Text = $"{AuditItems.Cast<object>().Count()/*:00*/}";
      table.Cell(rowIndex, 2).Range.Text = newItem.Audit;
      table.Cell(rowIndex, 3).Range.Text = newItem.ReleaseSoftDate.ToString("dd.MM.yyyy");
      table.Cell(rowIndex, 4).Range.Text = newItem.Version;
      table.Cell(rowIndex, 5).Range.Text = newItem.Comment;

      wDoc.Save();
      wApp.Visible = false;

      try
      {
        wDoc.Close();
        wApp.Quit();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
    }

    #endregion

    #region |Select|

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

        //var coll = new List<DirectionItem>(list);
        //coll.Add(new DirectionItem
        //{
        //  Id = 7,
        //  Code = "Все",
        //  Text = "Все категории"
        //});

        DirectionItems = CollectionViewSource.GetDefaultView(list);
        SelectedDirection = (DirectionItem)DirectionItems.CurrentItem;
      }
      catch (Exception ex)
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

    private void SelectFromIds(string direction, string category)
    {
      SqlConnection connection = new SqlConnection(_connectionString);
      string sql = String.Empty;

      if (direction == null && category == null)
        sql = $"SELECT * FROM Ids";
      if (category == "Все категории")
        sql = $"SELECT * FROM Ids i where i.Direction = '{direction}'";
      else if (direction != null && category != null)
        sql = $"SELECT * FROM Ids i where i.Direction = '{direction}' and i.Category = '{category}'";

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
                     IdAudit_SoftName = $"{drRow.Field<String>("IdAudit")} {drRow.Field<String>("SoftName")}"
                   };

        IdColl = new ObservableCollection<IdItem>(list);

        IdItems = CollectionViewSource.GetDefaultView(IdColl);


        IdItems.CollectionChanged += IdItemsCollectionChanged;
        IdItems.CurrentChanged += IdCurrentChanged;

        SelectedId = (IdItem)IdItems.CurrentItem;

        if (IdItems != null)
          IsEnabledCreateAudit = true;
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
    }

    private Guid? SelectCurrentParentId()
    {
      string sql = $"SELECT * FROM Ids i where i.IdAudit = '{SelectedId.IdAudit}'";
      var idsTable = new DataTable();
      var connection = new SqlConnection(_connectionString);
      try
      {
        connection.Open();
        SqlCommand command = new SqlCommand(sql, connection);
        var adapter = new SqlDataAdapter(command);

        //Получаем данные из БД и осуществляем привязку
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

        var parentId = list.FirstOrDefault()?.Id;

        return parentId;

      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        return Guid.Empty;
      }
      finally
      {
        connection?.Close();
      }
    }

    private void SelectFromAudits()
    {
      if (SelectedId == null)
      {
        AuditColl = new ObservableCollection<AuditItem>();
        IsEnabledCreateAudit = false;
        Player = String.Empty;
        Customer = String.Empty;
        return;
      }

      string sql = $"SELECT * FROM Audits a where a.ParentId = '{SelectedId.Id}'";
      var auditsTable = new DataTable();
      var connection = new SqlConnection(_connectionString);
      try
      {
        SqlCommand command = new SqlCommand(sql, connection);
        var adapter = new SqlDataAdapter(command);

        //Получаем данные из БД и осуществляем привязку
        connection.Open();
        adapter.Fill(auditsTable);

        var list = from drRow in auditsTable.AsEnumerable()
                   select new AuditItem()
                   {
                     ParentId = drRow.Field<Guid>("ParentId"),
                     Audit = drRow.Field<String>("Audit"),
                     ReleaseSoftDate = drRow.Field<DateTime>("ReleaseDate"),
                     SoftName = drRow.Field<String>("ReleaseName"),
                     Version = drRow.Field<String>("Version"),
                     Comment = drRow.Field<String>("Comment"),
                     IdAudit = SelectedId.IdAudit,
                     Category = SelectedId.Category,
                     Direction = SelectedId.Direction,
                     Path = drRow.Field<string>("Path"),
                     State = drRow.Field<int>("State"),
                     Plata = drRow.Field<string>("Plata"),
                     Player = drRow.Field<string>("Player"),
                     Customer = drRow.Field<string>("Customer"),
                     Order = drRow.Field<string>("OrderNum"),
                     EmailRecipient = drRow.Field<string>("EmailRecipient"),
                   };

        AuditColl = new ObservableCollection<AuditItem>(list);

        IsEnabledCreateAudit = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        return;
      }
      finally
      {
        connection.Close();
      }
    }

    #endregion

    #region |Insert|

    public void InsertAudit(AuditItem auditItem)
    {
      SqlConnection sqlConnection = new SqlConnection(_connectionString);
      // var alreadySendedValue = "Нет";
      try
      {
        sqlConnection.Open();
        SqlCommand cmd = new SqlCommand($"INSERT into Audits (ParentId, Audit, ReleaseDate, ReleaseName, Version, Comment, Path, State, Plata, OrderNum, Player, Customer, EmailRecipient)" +
                                        $" VALUES ('{auditItem.ParentId}','{auditItem.Audit}', '{auditItem.ReleaseSoftDate}', N'{auditItem.SoftName}', '{auditItem.Version}', N'{auditItem.Comment}', '{auditItem.Path}', '{auditItem.State}', N'{auditItem.Plata}', N'{auditItem.Order}', N'{auditItem.Player}', N'{auditItem.Customer}', N'{auditItem.EmailRecipient}')", sqlConnection);
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

    #region |Export|

    private AuditItem ExportToSharePoint(AuditItem audit, int playerId, List<int> customersId)
    {
      //тестовый портал http://ipm-fz-srv085/po
      using (ClientContext ctx = new ClientContext("http://shpointservnew/po"))
      {
        var myWeb = ctx.Web;
        ctx.Load(myWeb);
        ctx.ExecuteQuery();

        try
        {
          var alreadySendedValue = "Нет";

          var list = myWeb.Lists.GetByTitle("Программное обеспечение");
          var itemCreationInfo = new ListItemCreationInformation();
          var newItem = list.AddItem(itemCreationInfo);

          newItem["Direction"] = audit.Direction;
          newItem["Category"] = audit.Category;
          newItem["Id0"] = audit.IdAudit;
          newItem["Audit"] = audit.Audit;
          newItem["SoftwareName"] = audit.SoftName;
          newItem["Version"] = audit.Version;
          if (SelectedId.ReleaseType == 1)
          {
            var releaseUrlValue = new FieldUrlValue();
            releaseUrlValue.Description = "Release";
            releaseUrlValue.Url = audit.Path;
            newItem["Release"] = releaseUrlValue;
          }
          else
          {
            var preliminaryUrlValue = new FieldUrlValue();
            preliminaryUrlValue.Description = "Preliminary";
            preliminaryUrlValue.Url = audit.Path;
            newItem["Preliminary"] = preliminaryUrlValue;
          }

          newItem["Plata"] = audit.Plata;
          newItem["Description"] = audit.Comment;

          var pathInMessage = new FieldUrlValue();
          pathInMessage.Description = $"{audit.Order}";
          pathInMessage.Url = @"http://shpointservnew/po/Lists/List21/view1.aspx?View={5FD65A67-9B84-43EC-9AF6-C8C6FC9473FA}&FilterField1=Audit&FilterValue1=" +
                              audit.Audit + "&InitialTabId=Ribbon%2EListItem&VisibilityContext=WSSTabPersistence";
          newItem["PathInMessage"] = pathInMessage;

          // if (playerId != 0)
          List<FieldUserValue> playersList = new List<FieldUserValue>();
          foreach (var user in Players)
          {
            playersList.Add(FieldUserValue.FromUser(user));
          }
          newItem["Player"] = playersList;

          var customerId = "";
          foreach (var custId in customersId)
          {
            if (custId != 0)
              customerId += $"{custId}";
            //newItem["Customer"] = customerId;
          }

          List<FieldUserValue> usersList = new List<FieldUserValue>();
          foreach (var user in Customers)
          {
            usersList.Add(FieldUserValue.FromUser(user));
          }

          newItem["Customer"] = usersList;

          if(Recipients != null)
          if (!string.IsNullOrEmpty(Recipients.FirstOrDefault()))
          {
            List<FieldUserValue> recipientsList = new List<FieldUserValue>();
            foreach (var recipient in Recipients)
            {
              recipientsList.Add(FieldUserValue.FromUser(recipient));
            }
            newItem["EmailRecipient"] = recipientsList;
          }


          // var y = new FieldUserValue();
          // y.LookupId.CompareTo(1071);
          // y.LookupId.CompareTo(1755);

          // newItem["Customer"] = y;
          //// newItem["Customer"] = 1071;
          // //customerId;

          newItem["AlreadySendedPr"] = alreadySendedValue;
          newItem["AlreadySendedRel"] = alreadySendedValue;

          if (!String.IsNullOrEmpty(audit.Order))
          {
            var orderUrlValue = new FieldUrlValue();
            orderUrlValue.Description = audit.Order;
            orderUrlValue.Url = @"http://shpointservnew/po/Lists/List8/AllItems.aspx?View={647E0FEA-5492-47C1-A767-E43951949A21}&FilterField1=Title&FilterValue1="
            + audit.Order +
            @"&InitialTabId=Ribbon%2EListItem&VisibilityContext=WSSTabPersistence";
            newItem["Order0"] = orderUrlValue;
          }
          else newItem["Order0"] = string.Empty;

          newItem.Update();

          ctx.ExecuteQuery();
        }

        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          return null;
        }
      }
      return audit;
    }


    #endregion    

    #region |Update|

    public static void UpdateFieldInShPoint(DirectoryInfo path, AuditItem auditItem, int customerId, int playerId, List<String> customers, List<string> players)
    {
      using (ClientContext ctx = new ClientContext("http://shpointservnew/po"))
      //using (ClientContext ctx = new ClientContext("http://ipm-fz-srv085/po"))
      {
        var myWeb = ctx.Web;
        var list = myWeb.Lists.GetByTitle("Программное обеспечение");
        // var list = myWeb.Lists.GetByTitle("ПО_Тест");

        ListItemCollection items = list.GetItems(CamlQuery.CreateAllItemsQuery());
        ctx.Load(items); // loading all the fields
        ctx.ExecuteQuery();

        foreach (var item in items)
        {
          if ((string)item.FieldValues["Audit"] == auditItem.Audit)
          {
            if (path != null)
            {
              var releaseUrlValue = new FieldUrlValue();
              releaseUrlValue.Description = "Release";
              releaseUrlValue.Url = path.FullName;
              item["Release"] = releaseUrlValue;
              item["Preliminary"] = "";
            }

            else if (path == null)
            {
              item["Plata"] = auditItem.Plata;


              List<FieldUserValue> playersList = new List<FieldUserValue>();

              foreach (var player in players)
                playersList.Add(FieldUserValue.FromUser(player));

              item["Player"] = playersList;
             

              item["Version"] = auditItem.Version;

              List<FieldUserValue> customersList = new List<FieldUserValue>();

              foreach (var customer in customers)
                customersList.Add(FieldUserValue.FromUser(customer));

              item["Customer"] = customersList;

              // item["Customer"] = customerId;
              item["Description"] = auditItem.Comment;

              if (!String.IsNullOrEmpty(auditItem.Order))
              {
                var orderUrlValue = new FieldUrlValue();
                orderUrlValue.Description = auditItem.Order;
                orderUrlValue.Url = @"http://shpointservnew/po/Lists/List8/AllItems.aspx?View={647E0FEA-5492-47C1-A767-E43951949A21}&FilterField1=Title&FilterValue1="
                + auditItem.Order +
                @"&InitialTabId=Ribbon%2EListItem&VisibilityContext=WSSTabPersistence";
                item["Order0"] = orderUrlValue;
              }
            }
            item.Update();
          }
        }
        ctx.ExecuteQuery(); // important, commit changes to the server
      }
    }

    private void UpdatePath(string auditName, DirectoryInfo newPath)
    {
      SqlConnection sqlConnection = new SqlConnection(_connectionString);

      var path = $@"file://{newPath}/".Replace('\\', '/');

      try
      {
        sqlConnection.Open();
        using (SqlCommand cmd =
          new SqlCommand("UPDATE Audits SET Path=@Path" +
                         " WHERE Audit=@Audit", sqlConnection))
        {
          cmd.Parameters.AddWithValue("@Audit", auditName);
          cmd.Parameters.AddWithValue("@Path", path);
          cmd.ExecuteNonQuery();
        }

        SelectedAudit.Path = path;
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


    static void Update(int newState, string auditName)
    {
      SqlConnection sqlConnection = new SqlConnection(_connectionString);

      try
      {
        sqlConnection.Open();
        using (SqlCommand cmd =
          new SqlCommand("UPDATE Audits SET State=@State" +
                         " WHERE Audit=@Audit", sqlConnection))
        {
          cmd.Parameters.AddWithValue("@Audit", auditName);
          cmd.Parameters.AddWithValue("@State", newState);
          cmd.ExecuteNonQuery();
        }
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

    private void AddFileExecute()
    {
      var path = new DirectoryInfo($"{SelectedAudit.Path.Replace("file://", "")}");

      OpenFileDialog(path, "All files (*.*)|*.*");
    }

    #region |Copy to Release|

    private void CopyAuditExecute()
    {
      var pathFrom = new DirectoryInfo($"{ConfigurationManager.AppSettings["PathDirectoryPreliminary"]}{((IdItem)SelectedId).IdAudit}");

      var pathTo = new DirectoryInfo($"{ConfigurationManager.AppSettings["PathDirectoryRelease"]}{((IdItem)SelectedId).IdAudit}");

      CopyAudit(pathFrom, pathTo);
    }

    //Проверка на корректность заполнения полей Заказчик и Исполнитель
    public UserCollection CheckCorrectUser()
    {
      using (ClientContext ctx = new ClientContext("http://shpointservnew/po"))
      {
        var myWeb = ctx.Web;
        ctx.Load(myWeb);
        ctx.ExecuteQuery();
        var collGroups = myWeb.SiteGroups;
        ctx.Load(collGroups);
        ctx.ExecuteQuery();
        Group mainGroup = collGroups.GetById(708);

        ctx.Load(mainGroup.Users);
        ctx.ExecuteQuery();

        return mainGroup.Users;
      }
    }

    private void GetSubDir(List<DirectoryInfo> dirs, DirectoryInfo pathTo)
    {
      foreach (var dir in dirs)
      {
        if (Directory.Exists(pathTo.FullName) == false)
          Directory.CreateDirectory(pathTo.FullName);

        var subDir =
            pathTo.CreateSubdirectory(dir.Name);

        // Copy each file into new directory.
        foreach (FileInfo fi in dir.GetFiles())
          fi.CopyTo(Path.Combine($"{pathTo}\\{dir.Name}\\", fi.Name), true);

        //проверяем наличие вложенных папок
        var list = dir.GetDirectories().ToList();
        //Если есть аложения, то рекурсия
        if (list.Count != 0) GetSubDir(list, subDir);
      }
    }

    private void CopySubdirs(DirectoryInfo pathFrom, DirectoryInfo pathTo)
    {
      if (pathFrom.FullName.ToLower() == pathTo.FullName.ToLower()) return;

      // Copy each file into new directory.
      foreach (FileInfo fi in pathFrom.GetFiles())
        fi.CopyTo(Path.Combine(pathTo.ToString(), fi.Name), true);

      var allDir = pathFrom.GetDirectories().ToList();
      if (allDir.Count != 0) GetSubDir(allDir, pathTo);
    }

    private void CopyAudit(DirectoryInfo pathFrom, DirectoryInfo pathTo)
    {
      if (pathFrom.FullName.ToLower() == pathTo.FullName.ToLower()) return;

      var selectedDir = pathFrom.GetDirectories().Where(x => x.Name == SelectedAudit.Audit).FirstOrDefault();

      var subDir = pathTo.CreateSubdirectory(selectedDir.Name);

      //add .pdf file to a new folder
      var res = OpenFileDialog(subDir, "pdf files (*.pdf)|*.pdf");
      if (res)
      {
        // Check if the target directory exists, if not, create it.
        if (Directory.Exists(pathTo.FullName) == false)
          Directory.CreateDirectory(pathTo.FullName);

        //Копируем файл Перечень ревизий из PRELIMINARY в RELEASE  
        foreach (var fi in pathFrom.GetFiles())
          fi.CopyTo(Path.Combine(pathTo.ToString(), fi.Name), true);

        if (selectedDir != null)
        {
          CopySubdirs(selectedDir, subDir);

          UpdateFieldInShPoint(subDir, SelectedAudit, 0, 0, new List<string>(), new List<string>());

          UpdatePath(SelectedAudit.Audit, subDir);

          // 1 - PRELIMINARY
          // 2 - RELEASE
          SelectedAudit.State = 2;
          Update(SelectedAudit.State, SelectedAudit.Audit);

          var playerEmail = String.Empty;
          var customerEmail = String.Empty;

          //Проверка на корректность заполнения полей Заказчик и Исполнитель
          var users = CheckCorrectUser();

          foreach (User user in users)
          {
            if (!string.IsNullOrEmpty(Player) && user.Title.Contains(Player))
              playerEmail = user.Email;

            if (!string.IsNullOrEmpty(Customer) && user.Title.Contains(Customer))
              customerEmail = user.Email;
          }

          MessageBox.Show($"Скопировано в релиз!");
        }
        else
        {
          MessageBox.Show("Копирование не произошло! Не выбран файл .pdf");
        }
      }
    }

    private bool OpenFileDialog(DirectoryInfo auditPath, string filtertype)
    {
      var result = false;
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Filter = filtertype,
        FilterIndex = 2,
        RestoreDirectory = true
      };

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        var fileName = openFileDialog.FileName;
        try
        {
          File.Copy(fileName, $"{auditPath.FullName}/{openFileDialog.SafeFileName}");
          result = true;
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          result = false;
        }
      }
      return result;
    }



    #endregion

    #region |Changed|

    private void IdItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      IdItems.MoveCurrentToLast();
    }

    private void IdCurrentChanged(object sender, EventArgs e)
    {
      int count;
      if (SelectedId != null)
      {
        if (SelectedId.Id == Guid.Empty)
        {
          var selectCurrentParentId = SelectCurrentParentId();
          if (selectCurrentParentId != null) SelectedId.Id = (Guid)selectCurrentParentId;
        }

        SelectFromAudits();
        count = AuditColl.Count;
        Audit = $"{SelectedId.IdAudit}-{count/*:00*/}";
        var lastOrDefault = AuditColl.OrderBy(x => x.ReleaseSoftDate).LastOrDefault();

        Plata = lastOrDefault != null ? lastOrDefault.Plata : String.Empty;
        Player = lastOrDefault != null ? lastOrDefault.Player : String.Empty;
        Customer = lastOrDefault != null ? lastOrDefault.Customer : String.Empty;
        Order = lastOrDefault != null ? lastOrDefault.Order : String.Empty;
        EmailRecipient = lastOrDefault != null ? lastOrDefault.EmailRecipient : String.Empty;
      }
      else
      {
        count = 0;
        AuditColl = new ObservableCollection<AuditItem>();
        Audit = $"M60-XXXXXX-{DateTime.Today.Date:yyMMdd}-{count/*:00*/}";
        Plata = String.Empty;
        Player = String.Empty;
        Customer = String.Empty;
        Order = String.Empty;
        EmailRecipient = String.Empty;
      }

      AuditItems = CollectionViewSource.GetDefaultView(AuditColl);
      AuditItems.SortDescriptions.Add(new SortDescription(nameof(AuditItem.ReleaseSoftDate), ListSortDirection.Ascending));
      Comment = String.Empty;
    }

    private void CreateAuditPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsCreateAuditSelected")
      {
        SelectFromIds(null, null);
        IdCurrentChanged(null, null);
        DirectionItems.MoveCurrentToFirst();
      }
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

        var id = list.LastOrDefault().Id + 1;

        list.Add(new CategoryItem
        {
          Id = id,
          Code = "Все",
          Text = "Все категории",
        });

        CategoryItems = CollectionViewSource.GetDefaultView(list);

        SelectedCategory = list.LastOrDefault();
        //(CategoryItem)CategoryItems.CurrentItem;
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

    #region  |Edit Audits|   

    public InteractionRequest<EditAuditConfirmation> EditAuditRequest { get; }

    private void EditAuditExecute()
    {
      //Вызов диалога для изменения ревизии 
      EditAuditRequest.Raise(new EditAuditConfirmation
      {
        IdAudit = SelectedAudit.IdAudit,
        Audit = SelectedAudit.Audit,
        Plata = SelectedAudit.Plata,
        Version = SelectedAudit.Version,
        Customer = SelectedAudit.Customer,
        Player = SelectedAudit.Player,
        Comment = SelectedAudit.Comment,
        Order = SelectedAudit.Order,
      },
       res =>
      {
        if (!res.Confirmed) return;
        try
        {
          SelectedAudit.Plata = res.Plata;
          SelectedAudit.Version = res.Version;
          SelectedAudit.Customer = res.Customer;
          SelectedAudit.Player = res.Player;
          SelectedAudit.Comment = res.Comment;
          SelectedAudit.Order = res.Order;

          EditAuditToFile(SelectedAudit);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      });
    }

    //Редактировать ревизию в файле
    private void EditAuditToFile(AuditItem selectedItem)
    {
      _pathDirectory = SelectedId.ReleaseType == 1 ? ConfigurationManager.AppSettings["PathDirectoryRelease"] : ConfigurationManager.AppSettings["PathDirectoryPreliminary"];

      var pathD = $"{_pathDirectory}{((IdItem)SelectedId).IdAudit}";

      var wApp = new Word.Application { Visible = false };

      //найти файл с SelectedId.Name.docx
      var wDoc = wApp.Documents.Open($@"{pathD}/{((IdItem)SelectedId).IdAudit}.docx");

      //Добавить ревизию в список
      var table = wDoc.Tables[3];
      table.Borders.Enable = 1;

      var index = Array.IndexOf(AuditColl.ToArray(), selectedItem) + 2;

      // table.Cell(index, 3).Range.Text = selectedItem.ReleaseSoftDate.ToString("dd.MM.yyyy");
      table.Cell(index, 4).Range.Text = selectedItem.Version;
      table.Cell(index, 5).Range.Text = selectedItem.Comment;

      wDoc.Save();
      wApp.Visible = false;

      try
      {
        wDoc.Close();
        wApp.Quit();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }

    }

    #endregion

    #region |Delete|

    public void DeleteExecute()
    {
      var last = AuditColl.OrderBy(x => x.ReleaseSoftDate).LastOrDefault();

      //if (MessageBox.Show($"Вы уверены, что хотите удалить ревизию {last.Audit}?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == DialogResult.Yes)      

      AuditColl.Remove(last);

      DeleteAuditFromDB(last);

      DeleteAuditFromShPoint(last);

      DeleteFromFile(last);

      DeleteAuditFromFolder(last);

      var count = AuditItems.Cast<object>().Count();
      Audit = $"{((IdItem)SelectedId).IdAudit}-{count/*:00*/}";
      Comment = String.Empty;
      var lastOrDefault = AuditColl.OrderBy(x => x.ReleaseSoftDate).LastOrDefault();
      Plata = lastOrDefault != null ? lastOrDefault.Plata : String.Empty;
      Player = lastOrDefault != null ? lastOrDefault.Player : String.Empty;
      Customer = lastOrDefault != null ? lastOrDefault.Customer : String.Empty;
    }

    private void DeleteAuditFromDB(AuditItem auditItem)
    {
      SqlConnection sqlConnection = new SqlConnection(_connectionString);

      sqlConnection.Open();
      SqlCommand cmd = new SqlCommand($"DELETE from Audits Where Audit = '{auditItem.Audit}'", sqlConnection);

      try
      {
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

    private void DeleteAuditFromShPoint(AuditItem auditItem)
    {
      using (ClientContext ctx = new ClientContext("http://shpointservnew/po"))
      {
        var myWeb = ctx.Web;
        var list = myWeb.Lists.GetByTitle("Программное обеспечение");

        ListItemCollection items = list.GetItems(CamlQuery.CreateAllItemsQuery());
        ctx.Load(items);
        ctx.ExecuteQuery();

        foreach (var item in items)
        {
          if ((string)item.FieldValues["Audit"] == auditItem.Audit)
          {
            item.DeleteObject();
            ctx.ExecuteQuery();
            return;
          }
        }
      }
    }

    private void DeleteFromFile(AuditItem auditItem)
    {
      _pathDirectory = SelectedId.ReleaseType == 1 ? ConfigurationManager.AppSettings["PathDirectoryRelease"] : ConfigurationManager.AppSettings["PathDirectoryPreliminary"];

      var pathD = $"{_pathDirectory}{((IdItem)SelectedId).IdAudit}";

      var wApp = new Word.Application { Visible = false };

      //найти файл с SelectedId.Name.docx
      var wDoc = wApp.Documents.Open($@"{pathD}/{((IdItem)SelectedId).IdAudit}.docx");

      var table = wDoc.Tables[3];
      table.Borders.Enable = 1;

      int rowIndex = 2;

      rowIndex = AuditItems.Cast<object>().Count() + 2;
      table.Rows[rowIndex].Delete();

      wDoc.Save();
      wApp.Visible = false;

      try
      {
        wDoc.Close();
        wApp.Quit();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
    }

    private void DeleteAuditFromFolder(AuditItem auditItem)
    {
      var path = auditItem.Path.Remove(0, 7);

      var str = $@"{path.Remove(path.Length - 1, 1)}";

      if (Directory.Exists(str))
      {
        Directory.Delete(str, true);
        MessageBox.Show($"{auditItem.Audit} удалена");
      }

      //Удаление папок из Прилиминари
      if (str.Contains("RELEASE"))
      {
        //Удаление папок из Релиз 
        var strRelease1 = str.Replace("RELEASE", "PRELIMINARY");
        if (Directory.Exists(strRelease1))
        {
          Directory.Delete(strRelease1, true);
          MessageBox.Show($"{auditItem.Audit} удалена из PRELIMINARY");
        }
      }

      if (str.Contains("PRELIMINARY"))
      {
        //Удаление папок из Релиз 
        var strRelease = str.Replace("PRELIMINARY", "RELEASE");
        if (Directory.Exists(strRelease))
        {
          Directory.Delete(strRelease, true);
          MessageBox.Show($"{auditItem.Audit} удалена из RELEASE");
        }
      }
    }

    #endregion

    #region |Open Folder|

    private void OpenFolderExecute()
    {
      var path = SelectedAudit.Path.Replace("file://", "").Replace('/', '\\');

      var StartInformation = new ProcessStartInfo();

      StartInformation.FileName = $@"{path}";

      Process process = Process.Start(StartInformation);
    }

    #endregion

    #region |Order|

    //Проверка существования заказа по номеру
    private bool CheckCorrectOrder(string id)
    {
      //http://ipm-fz-srv085/po
      //http://shpointservnew/po
      using (ClientContext ctx = new ClientContext("http://shpointservnew/po"))
      {
        var myWeb = ctx.Web;
        ctx.Load(myWeb);
        ctx.ExecuteQuery();

        var appList = myWeb.Lists.GetByTitle("Заказы");

        ctx.Load(appList);
        ctx.ExecuteQuery();

        CamlQuery camlQuery = new CamlQuery();
        camlQuery.ViewXml = "<View Scope=\"RecursiveAll\"></View>";

        var items = appList.GetItems(camlQuery);
        var res = false;
        try
        {
          ctx.Load(items);
          ctx.ExecuteQuery();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          return res;
        }

        foreach (var item in items)
        {
          if (item.FieldValues["Title"].ToString() == id)
          {
            res = true;
            return res;
          }
        }

        return res;
      }
    }

    #endregion

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
