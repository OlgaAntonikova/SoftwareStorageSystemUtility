using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareStorageSystemUtility.ObjectModel
{
 public class AuditItem : INotifyPropertyChanged
  {
    private int _state;
    private string _player;
    private string _customer;
    private string _version;
    private string _comment;
    private string _plata;
    private string _order;
    private string _emailRecipient;

    //Идентификатор
    public string IdAudit { get; set; }
    //Ревизия
    public string Audit { get; set; }

    //М60
    public string SoftName { get; set; }

    //Дата выпуска ревизии
    public DateTime ReleaseSoftDate { get; set; }

    public string Version {
      get => _version;
      set
      {
        if (Equals(value, _version)) return;
        _version = value;
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

    public string Comment {
      get => _comment;
      set
      {
        if (Equals(value, _comment)) return;
        _comment = value;
        OnPropertyChanged();
      }
    }

    //Направление
    public string Direction { get; set; }

    //Категория
    public string Category { get; set; }

    //Идентификатор
    public Guid? ParentId { get; set; }

    //Релиз
    public string ReferenceReriase { get; set; }

    //Тестовая ревизия
    public string ReferencePreliminary { get; set; }

    //Заказчик
    public string Customer {
      get => _customer;
      set
      {
        if (Equals(value, _customer)) return;
        _customer = value;
        OnPropertyChanged();
      }
    }

    //Плата
    public string Plata {
      get => _plata;
      set
      {
        if (Equals(value, _plata)) return;
        _plata = value;
        OnPropertyChanged();
      }
    }

    //Описание
    public string Description { get; set; }

    //Исполнитель
    public string Player
    {
      get => _player;
      set
      {
        if (Equals(value, _player)) return;
        _player = value;
        OnPropertyChanged();
      }
    }

    //Сотрудник, которому необходимо отправить Email
    public string EmailRecipient
    {
      get => _emailRecipient;
      set
      {
        if (Equals(value, _emailRecipient)) return;
        _emailRecipient = value;
        OnPropertyChanged();
      }
    }

    //Путь ревизии (где лежит папка с ревизией)
    public string Path { get; set; }

    //Статус отгрузки на SharePoint
    public int State
    {
      get => _state;
      set
      {
        if (Equals(value, _state)) return;
        _state = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
