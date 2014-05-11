using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderDash.Models
{
    [Serializable]
    public class Shortcut
    {
        public string filename { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
    }
}
