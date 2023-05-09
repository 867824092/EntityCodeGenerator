using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECG.Contracts {
    public class TableStructure {
        public TableStructure(string nameSpace,string tableName,int capacity) {
            NameSpace = nameSpace;
            TableName = tableName;
            Columns = new List<ColumnDescription>(capacity);
        }
        public List<ColumnDescription> Columns { get; set; }
        public string NameSpace { get; set; }
        public string TableName { get; set; }
    }
    
    
    public sealed class ColumnDescription {
        public string? ColumnName { get; set; }
        public string? ColumnType { get; set; }
        public string? Comment { get; set; }
        public bool IsCanNull { get; set; }
    }
}
