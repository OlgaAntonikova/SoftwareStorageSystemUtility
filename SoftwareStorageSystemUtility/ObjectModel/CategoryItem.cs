﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareStorageSystemUtility.ObjectModel
{
 public class CategoryItem
  {
    public int Id { get; set; }
    public string Code { get; set; }
    public string Text { get; set; }
    public int CategoryId { get; set; }
  }
}
