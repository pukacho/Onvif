//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ONVIFPTZControl
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailAndWhatsAppSender
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int ProjectId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool IsDisabled { get; set; }
    
        public virtual Project Project { get; set; }
    }
}
