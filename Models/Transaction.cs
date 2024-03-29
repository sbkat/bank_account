using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace bank_account.Models
{public class Transaction
    {
        [Key]
        public int TransactionId { get; set; } 
        [Display (Name="Withdrawal(-)/Deposit(+) Amount: ")]
        [Required (ErrorMessage = "Enter an amount.")]
        public int Amount { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public User User { get; set; }
    }
}