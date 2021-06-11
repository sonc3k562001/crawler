using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DanTriNew.Entity
{
    [DebuggerDisplay("{title},{content},{img},{link}")]
    class Event
    {
        public String title { get; set; }
        public String content { get; set; }
        public String img { get; set; }
        public String link { get; set; }
    }
}
