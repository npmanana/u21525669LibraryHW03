//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibraryHW03.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class students
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public students()
        {
            this.borrows = new HashSet<borrows>();
        }
    
        public int studentId { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public Nullable<System.DateTime> birthdate { get; set; }
        public string gender { get; set; }
        public string @class { get; set; }   //change made
        public Nullable<int> point { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<borrows> borrows { get; set; }
    }
}
