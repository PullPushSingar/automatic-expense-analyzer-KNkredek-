using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
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

    public Operation(DateTime operationDate, string operationTime, string description)
    {
        OperationDate = operationDate;
        OperationTime = operationTime;
        Description = description;

        try
        {

            String[] descriptoinsParts = description.Split(" ");
            /*
            foreach (var part in descriptoinsParts)
            {
                Console.WriteLine(part);
            }
            */
            if (descriptoinsParts[1].Equals("Obciazenie"))
            {
                if (decimal.TryParse(descriptoinsParts[6].Trim(), NumberStyles.Any, new CultureInfo("pl-PL"), out decimal operationAmount))
                {
                    OperationAmount = operationAmount * - 1;
                }

                if (decimal.TryParse(descriptoinsParts[descriptoinsParts.Length - 2].Trim(), NumberStyles.Any, new CultureInfo("pl-PL"), out decimal accountAmountAfterOperation))
                {
                    AccountAmountAfterOperation = accountAmountAfterOperation;
                }
                /*
                string operationAmountToStrong = descriptoinsParts[6].Trim();
                OperationAmount = decimal.Parse(operationAmountToStrong) * -1;
                string accountAmountAfterOperationToStrong = descriptoinsParts[descriptoinsParts.Length - 2].Trim();
                AccountAmountAfterOperation = decimal.Parse(accountAmountAfterOperationToStrong);
                */

            }
            else if (descriptoinsParts[1].Equals("Przelew"))
            {
                /*
                string operationAmountToStrong = descriptoinsParts[10].Trim();
                OperationAmount = decimal.Parse(operationAmountToStrong);
                string accountAmountAfterOperationToStrong = descriptoinsParts[descriptoinsParts.Length - 2].Trim();
                AccountAmountAfterOperation = decimal.Parse(accountAmountAfterOperationToStrong);
                */
                if (decimal.TryParse(descriptoinsParts[10].Trim(), NumberStyles.Any, new CultureInfo("pl-PL"), out decimal operationAmount))
                {
                    OperationAmount = operationAmount;
                }

                if (decimal.TryParse(descriptoinsParts[descriptoinsParts.Length - 2].Trim(), NumberStyles.Any, new CultureInfo("pl-PL"), out decimal accountAmountAfterOperation))
                {
                    AccountAmountAfterOperation = accountAmountAfterOperation;
                }


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas przetwarzania operacji: {ex.Message}");
        }

    
    }

    public override string ToString()
    {
        return $"Data operacji: {OperationDate.ToString("yyyy-MM-dd")}, " +
               $"Czas operacji: {OperationTime}, " +
               $"Kwota operacji: {OperationAmount.ToString("C2")}, " +
               $"Środki po operacji: {AccountAmountAfterOperation.ToString("C2")}, " +
               $"Opis: {Description}";
    }
}
