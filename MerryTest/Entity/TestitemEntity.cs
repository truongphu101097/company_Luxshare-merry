
using System.Collections.Generic;

namespace MerryTest.Entity
{
    public class TestitemEntity
    {
        public static List<TestitemEntity> ItemData = new List<TestitemEntity>();
        public int MethodId { get; set; }
        public string 测试项目 { get; set; }
        public string 耳机指令 { get; set; }
        public string 单位 { get; set; }
        public string 数值下限 { get; set; }
        public string 数值上限 { get; set; }
        public int 编号 { get; set; }
    }

}
