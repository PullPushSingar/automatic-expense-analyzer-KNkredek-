using System;
using System.Collections.Generic;

namespace gmailReaderWebApi.Models
{
    public partial class Operation
    {
        public int OperationId { get; set; }
        public DateTime OperationDate { get; set; }
        public TimeSpan? OperationTime { get; set; }
        public decimal? OperationAmount { get; set; }
        public decimal? AccountAmountAfterOperation { get; set; }
        public string? Description { get; set; }
    }
}
