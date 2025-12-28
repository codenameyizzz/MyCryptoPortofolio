using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Untuk Dropdown

namespace MyCryptoPortfolio.Web.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kode Aset (Ticker) wajib diisi")]
        [StringLength(10, ErrorMessage = "Maksimal 10 karakter")]
        [Display(Name = "Kode Aset (ex: BTC, BBCA)")]
        public string Ticker { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jumlah wajib diisi")]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Jumlah harus valid")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Harga wajib diisi")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Harga harus valid")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Tanggal Transaksi")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Tipe Transaksi")]
        public string TransactionType { get; set; } = "Buy";

        public List<SelectListItem>? TypeOptions { get; set; } 

        [Display(Name = "Biaya (Fee)")]
        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }
    }
}