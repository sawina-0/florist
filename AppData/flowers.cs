//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace florist.AppData
{
    using System;
    using System.Collections.Generic;
    
    public partial class flowers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public flowers()
        {
            this.allProducts = new HashSet<allProducts>();
            this.bouquetStructure = new HashSet<bouquetStructure>();
        }
    
        public int flowerID { get; set; }
        public int typeID { get; set; }
        public int colorID { get; set; }
        public int sizeID { get; set; }
        public int supplierID { get; set; }
        public decimal price { get; set; }
        public string img { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<allProducts> allProducts { get; set; }
        public virtual blossomSize blossomSize { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bouquetStructure> bouquetStructure { get; set; }
        public virtual color color { get; set; }
        public virtual supplier supplier { get; set; }
        public virtual type type { get; set; }
    }
}
