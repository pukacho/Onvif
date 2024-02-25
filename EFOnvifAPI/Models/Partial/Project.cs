using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFOnvifAPI.Models
{
    public partial class Project
    {
        [NotMapped]
        public byte[] Image { get; set; }
    }
}
