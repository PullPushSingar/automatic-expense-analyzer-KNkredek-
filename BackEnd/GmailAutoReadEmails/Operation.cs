using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Operation
{
    public DateTime OperationDate { get; set; }
    public string OperationTime { get; set; }
    public decimal OperationAmount { get; set; }
    public decimal AccountAmountAfterOperation { get; set; }
    public string Description { get; set; }

    public Operation(DateTime operationDate, string operationTime, decimal amount1, decimal amount2, string description)
    {
        OperationDate = operationDate;
        OperationTime = operationTime;
        OperationAmount = amount1;
        AccountAmountAfterOperation = amount2;
        Description = description;
    }
}
