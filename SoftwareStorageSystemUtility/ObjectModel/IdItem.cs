using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareStorageSystemUtility.ObjectModel
{
   public class IdItem
    {
      public string Title { get; set; }
      public Guid Id { get; set; }
      public string IdAudit { get; set; }
      public string Direction { get; set; }
      public string Category { get; set; }
      public string SoftName { get; set; }
      public int ReleaseType { get; set; }


    public string IdAudit_SoftName { get; set; }
  }
}
