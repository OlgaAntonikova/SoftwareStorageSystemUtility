using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareStorageSystemUtility.ViewModels.Dialogs.Notifications
{
 public class EditAuditConfirmation : Confirmation
  {
    public string IdAudit { get; set; }
    //Ревизия
    public string Audit { get; set; }
    public string Version  {  get; set; }
    public string Comment { get; set; }
    //Заказчик
    public string Customer { get; set; }

    //Плата
    public string Plata { get; set; }   

    //Исполнитель
    public string Player { get; set; }

    public string Order { get; set; }


    public EditAuditConfirmation()
    {
      Title = "Редактирование ревизии"; 
    }
  }
}
