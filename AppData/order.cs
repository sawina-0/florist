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
    
    public partial class order
    {
        public int orderID { get; set; }
        public int basketID { get; set; }
        public int salePointID { get; set; }
        public int totalPrice { get; set; }
    
        public virtual basket basket { get; set; }
        public virtual salePoint salePoint { get; set; }
    }
}
