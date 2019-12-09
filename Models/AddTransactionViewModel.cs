using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bank_account.Models
{
    public class AddTransactionViewModel
    {
        [NotMapped]
        public User User { get; set; }
        public Transaction Transaction { get; set; }
        public List<Transaction> ListofTransactions { get; set; } = new List<Transaction>();
        public int UserId { get; set; }
    }
}