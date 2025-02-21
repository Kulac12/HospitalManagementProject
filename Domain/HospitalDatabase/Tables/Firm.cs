using Domain_one.AC.Abstract;
using Domain_one.HospitalDatabase.Tables.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_one.HospitalDatabase.Tables
{
    public class Firm:BaseEntity,IEntity
    {
        public string Name { get; set; }
        public string FirmCode { get; set; }
        public string FirmShortName { get; set; }
        public Guid? ParentFirmId { get; set; }
        [ForeignKey("ParentFirmId")]
        public virtual Firm ParentFirm { get; set; }
        /// <summary>
        ///  virtual olarak işaretlendiği için EF Core lazy loading kullanarak yalnızca ihtiyaç duyulduğunda yükler. 
        /// </summary>
        public string TaxNumber { get; set; }
        public bool IsParentFirm { get; set; }
        public bool IsOwnFirm { get; set; }
        public bool IsCompany { get; set; } = true;
        public List<Firm> Firms { get; set; }
    
    }
}
