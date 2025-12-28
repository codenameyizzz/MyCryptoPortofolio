using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCryptoPortfolio.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Ticker { get; set; } = string.Empty; 

        [Column(TypeName = "decimal(18, 8)")] 
        public decimal Quantity { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Fee { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public TransactionType Type { get; set; } 
        
        public decimal TotalAmount => (Type == TransactionType.Buy) 
            ? (Quantity * Price) + Fee 
            : (Quantity * Price) - Fee;
    }
}