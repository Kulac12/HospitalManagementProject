using Core.Domain_one.Abstract;
using Domain_one.HospitalDatabase.Tables.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_one.HospitalDatabase.Tables
{
    public class Polyclinic : BaseEntity, IEntity
    {
        public string PoliclinicName { get; set; }
        public string PoliclinicDescription { get; set; }
       

    }
}
