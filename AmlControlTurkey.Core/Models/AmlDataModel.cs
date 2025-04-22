using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.Models
{
    public class AmlDataModel
    {
        public string UniqId { get; set; }
        public string NameTitle { get; set; }
        public string IdentityNumber { get; set; }
        public string OtherNameTitle { get; set; }
        public string Nationality { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Organization { get; set; }
        public string Emails { get; set; }
        public string Addresses { get; set; }
        public string Phones { get; set; }
        public double Score { get; set; }
        public string Source { get; set; }
        public string PhotoUrl { get; set; }
        public string LastUpdateTime { get; internal set; }
    }
}
