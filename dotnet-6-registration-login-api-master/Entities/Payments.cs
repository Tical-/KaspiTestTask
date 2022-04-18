using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Entities;

namespace WebApi.Entities
{
    public class Payments
    {
        public long Id { get; set; }
        [Column(TypeName = "money")]
        public decimal Money { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsRejected { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public User ToUser { get; set; }
        public int ToUserId { get; set; }
        public bool IsPayed { get; set; }
    }
}
