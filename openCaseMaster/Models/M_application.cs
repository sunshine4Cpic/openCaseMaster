//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace openCaseMaster.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class M_application
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public M_application()
        {
            this.M_publicTask = new HashSet<M_publicTask>();
            this.openTestTask = new HashSet<openTestTask>();
            this.project_app = new HashSet<project_app>();
        }
    
        public int ID { get; set; }
        public string name { get; set; }
        public string mark { get; set; }
        public string runApkName { get; set; }
        public string package { get; set; }
        public string mainActiviy { get; set; }
        public string package2 { get; set; }
        public Nullable<bool> isClear { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<M_publicTask> M_publicTask { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<openTestTask> openTestTask { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<project_app> project_app { get; set; }
    }
}
