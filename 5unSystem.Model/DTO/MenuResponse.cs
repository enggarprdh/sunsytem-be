using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5unSystem.Model.DTO
{
    public class MenuResponse
    {
        public string MenuID { get; set; }
        public string DisplayName { get; set; }
        public string? Icon { get; set; }
        public string? Path { get; set; }
        public bool IsHasSubMenu { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int Sequence { get; set; }
        public List<SubMenu> SubMenu { get; set; } = new List<SubMenu>();
    }

    public class SubMenu
    {
        public string MenuID { get; set; }
        public string DisplayName { get; set; }
        public string? Path { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int Sequence { get; set; }
    }
}
