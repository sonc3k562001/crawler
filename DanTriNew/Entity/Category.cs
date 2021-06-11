using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DanTriNew.Entity
{
    [DebuggerDisplay("{id},{name},{slug},{link}")]
    class Category
    {
        public int id { get; set; }
        public String name { get; set; }
        public String slug { get; set; }
        public String link { get; set; }
        
    }
}
