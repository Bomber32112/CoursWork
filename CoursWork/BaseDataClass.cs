using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursWork
{
    public class BaseDataClass
    {
        [MySqlColumn("ID")]
        public int ID { get; set; }
    }
}
