using Microsoft.SharePoint.Client;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using SoftwareStorageSystemUtility.ObjectModel;
using SoftwareStorageSystemUtility.ViewModels.Dialogs.Notifications;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SoftwareStorageSystemUtility.ViewModels.Dialogs
{
  public class EditAuditViewModel : BindableBase, IInteractionRequestAware
  {
    private static string _connectionString
     = @"Data Source=MySQL;Initial Catalog=OATest;Persist Security Info=True;User ID=OAUser;Password=321";

    private INotification _notification;
    private string _id;
    private string _audit;
    private string _version;
    private string _comment;
    private string _plata;
    private string _player;
    private string _customer;
    private string _order;


    public INotification Notification
    {
      get { return _notification; }
      set { SetProperty(ref _notification, value); }
    }

    public string IdAudit
    {
      get { return _id; }
      set { SetProperty(ref _id, value); }
    }
    //Ревизия
    public string Audit
    {
      get { return _audit; }
      set { SetProperty(ref _audit, value); }
    }
    public string Version
    {
      get { return _version; }
      set { SetProperty(ref _version, value); }
    }
    public string Comment
    {
      get { return _comment; }
      set { SetProperty(ref _comment, value); }
    }
    //Заказчик
    public string Customer
    {
      get { return _customer; }
      set { SetProperty(ref _customer, value); }
    }

    //Плата
    public string Plata
    {
      get { return _plata; }
      set { SetProperty(ref _plata, value); }
    }

    //Исполнитель
    public string Player
    {
      get { return _player; }
      set { SetProperty(ref _player, value); }
    }

    public string Order
    {
      get { return _order; }
      set { SetProperty(ref _order, value); }
    }

    public Action FinishInteraction { get; set; }

    public ICommand SaveCommand { get; }

    public ICommand CancelCommand { get; }

    public EditAuditViewModel()
    {
      SaveCommand = new DelegateCommand(SaveExecute);
      CancelCommand = new DelegateCommand(CancleExecute);

      PropertyChanged += (sender, args) =>
     {
       if (args.PropertyName == nameof(Notification))
       {
         var special = Notification as EditAuditConfirmation;
         if (special != null)
         {
           IdAudit = special.IdAudit;
           Audit = special.Audit;
           Plata = special.Plata;
           Version = special.Version;
           Comment = special.Comment;
           Player = special.Player;
           Customer = special.Customer;
           Order = special.Order;
         }
       }
     };
    }

    private void SaveExecute()
    {
      var playerId = 0;
      var customerId = 0;

      var customersId = new List<int>();

      var customers = Customer.Split(',', ';').ToList();
      var players = Player.Split(',', ';').ToList();

      var users = CheckCorrectUser();

      List<FieldUserValue> customersList = new List<FieldUserValue>();

      foreach (var customer in customers)
      {
        var correctUserName = customer.TrimStart(' ');
        customersList.Add(FieldUserValue.FromUser(correctUserName));
      }

      List<FieldUserValue> playersList = new List<FieldUserValue>();

      foreach (var player in players)
      {
        var correctUserName = player.TrimStart(' ');
        playersList.Add(FieldUserValue.FromUser(correctUserName));
      }


      //foreach (User user in users)
      //{
      //  if (!string.IsNullOrEmpty(Player) && user.Title.Contains(Player))
      //    playerId = user.Id;
      //  //if (!string.IsNullOrEmpty(Customer) && user.Title.Contains(Customer))
      //  //  customerId = user.Id;

      //  foreach (var customer in customersList)
      //  {
      //    if (user.Title.Contains(customer.LookupValue))
      //      customerId += 1;
      //  }
      //}
      //if (!string.IsNullOrEmpty(Customer) && customerId == 0)
      //{
      //  MessageBox.Show($"Заказчик {Customer} не найден. Проверьте правильность заполнения поля");
      //  return;
      //}
      //if (!string.IsNullOrEmpty(Customer) || (customersList.Count() > 1 ))
      //{
      //  if (customersList.Count() > 1)
      //    MessageBox.Show($"Заказчики {Customer} не найдены. Проверьте правильность заполнения поля");
      //  else
      //    MessageBox.Show($"Заказчик {Customer} не найден. Проверьте правильность заполнения поля");
      //  return;
      //}
      //if (!string.IsNullOrEmpty(Player) && playerId == 0)
      //{
      //  MessageBox.Show($"Исполнитель {Player} не найден. Проверьте правильность заполнения поля");
      //  return;
      //}

      var selectedAudit = new AuditItem
      {
        Audit = Audit,
        Comment = Comment,
        Version = Version,
        Plata = Plata,
        Player = Player,
        Customer = Customer,
        Order = Order,
      };

      //Редактировать ревизию в БД
      EditAuditToDB(selectedAudit);

      //Редактировать ревизию на портале
      CreateAuditViewModel.UpdateFieldInShPoint(null, selectedAudit, customerId, playerId, customers, players);

      var special = (EditAuditConfirmation)Notification;
      special.Confirmed = true;
      special.Audit = Audit;
      special.Comment = Comment;
      special.Version = Version;
      special.Plata = Plata;
      special.Player = Player;
      special.Customer = Customer;
      special.Order = Order;

      FinishInteraction();
    }

    private void EditAuditToDB(AuditItem selectedItem)
    {
      SqlConnection sqlConnection = new SqlConnection(_connectionString);

      try
      {
        sqlConnection.Open();
        using (SqlCommand cmd =
          new SqlCommand("UPDATE Audits SET Version=@Version, Comment=@Comment, Plata=@Plata, Player=@Player, Customer=@Customer, OrderNum=@OrderNum" +
                         " WHERE Audit=@Audit", sqlConnection))
        {
          cmd.Parameters.AddWithValue("@Audit", selectedItem.Audit);
          //cmd.Parameters.AddWithValue("@State", selectedItem.State);

          if (selectedItem.Comment != null)
            cmd.Parameters.AddWithValue("@Comment", selectedItem.Comment);
          else
            cmd.Parameters.AddWithValue("@Comment", DBNull.Value);

          if (selectedItem.Plata != null)
            cmd.Parameters.AddWithValue("@Plata", selectedItem.Plata);
          else
            cmd.Parameters.AddWithValue("@Plata", DBNull.Value);

          cmd.Parameters.AddWithValue("@Version", selectedItem.Version);

          if (selectedItem.Player != null)
            cmd.Parameters.AddWithValue("@Player", selectedItem.Player);
          else
            cmd.Parameters.AddWithValue("@Player", DBNull.Value);

          if (selectedItem.Customer != null)
            cmd.Parameters.AddWithValue("@Customer", selectedItem.Customer);
          else
            cmd.Parameters.AddWithValue("@Customer", DBNull.Value);

          if (selectedItem.Order != null)
            cmd.Parameters.AddWithValue("@OrderNum", selectedItem.Order);
          else
            cmd.Parameters.AddWithValue("@OrderNum", DBNull.Value);

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

    private void CancleExecute()
    {
      FinishInteraction();
    }

    //Проверка на корректность заполнения полей Заказчик и Исполнитель
    private UserCollection CheckCorrectUser()
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
        //ctx.Load(mainGroup.Users);
        //ctx.ExecuteQuery();        

        return mainGroup.Users;
      }
    }
  }
}
