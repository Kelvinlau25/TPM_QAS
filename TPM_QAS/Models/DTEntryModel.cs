using System;
using System.Collections.Generic;

namespace DTEntryModel
{
    public class DailyJobEntry
    {
        public string JobNo { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public List<DailyJobEntry> Entries { get; set; } = new List<DailyJobEntry>();
    }
}
