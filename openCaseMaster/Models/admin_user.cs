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
    
    public partial class admin_user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public admin_user()
        {
            this.M_runScene = new HashSet<M_runScene>();
            this.caseFramework = new HashSet<caseFramework>();
        }
    
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Permission { get; set; }
        public Nullable<System.DateTime> GreatDate { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public Nullable<int> Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<M_runScene> M_runScene { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<caseFramework> caseFramework { get; set; }
        public virtual user_type user_type { get; set; }
    }
}
