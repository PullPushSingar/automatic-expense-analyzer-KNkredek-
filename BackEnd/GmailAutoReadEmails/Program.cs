using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Net.Http;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

class Program
{
    static string[] Scopes = { GmailService.Scope.GmailReadonly };
    static string ApplicationName = "Gmail API .NET Application";

    static async Task Main(string[] args)
    {
        UserCredential credential;

        using (var stream = new FileStream("C:\\Users\\huber\\Desktop\\automatic-expense-analyzer-KNkredek-\\BackEnd\\GmailAutoReadEmails\\client_secret.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            Console.WriteLine("Credential file saved to: " + credPath);
        }

        
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        DateTime today = DateTime.UtcNow;
        Console.WriteLine($"Today: {today}");
        
   
        
        string queryDate = today.ToString("yyyy/MM/dd").Replace(".","/");
        UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List("me");
        string queryDatePlusOne = today.AddDays(1).ToString("yyyy/MM/dd").Replace(".", "/");

        
        Console.WriteLine(queryDate);
        Console.WriteLine(queryDatePlusOne);
 
        request.Q = $"after:{"2024/01/01"} before:{"2024/02/01"}";
      //  request.Q = $"after:{queryDate} before:{queryDatePlusOne}";
       




        IList<Message> messages = request.Execute().Messages;
        Console.WriteLine("Messages:");
        if (messages != null && messages.Count > 0)
        {
            foreach (var messageItem in messages)
            {
                var emailInfoReq = service.Users.Messages.Get("me", messageItem.Id);
                var emailInfoResponse = emailInfoReq.Execute();

                if (emailInfoResponse != null)
                {
                    if (emailInfoResponse.Payload.Parts != null)
                    {
                        foreach (var part in emailInfoResponse.Payload.Parts)
                        {
                            if (part.MimeType == "text/html" && part.Body.AttachmentId != null)
                            {
                                string htmlContent = await GetAttachmentHtml(service, "me", messageItem.Id, part.Body.AttachmentId);
                                if (!string.IsNullOrEmpty(htmlContent))
                                {
                                    ProcessEmailHtmlContent(htmlContent);
                                }

                            }
                        }
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("No messages found.");
        }
        Console.Read();
    }

    

    static String GetNestedBodyParts(IList<MessagePart> parts, String accumulatedHtml)
    {
        foreach (var part in parts)
        {
            if (part.Parts == null)
            {
                if (part.Body != null && !string.IsNullOrEmpty(part.Body.Data))
                {
                    accumulatedHtml += DecodeBase64String(part.Body.Data);
                }
            }
            else
            {
                accumulatedHtml = GetNestedBodyParts(part.Parts, accumulatedHtml);
            }
        }

        return accumulatedHtml;
    }
    static void ProcessEmailHtmlContent(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        DateTime todayDate = DateTime.UtcNow;

      
        List<Operation> operations = new List<Operation>();
       
        var dateNode = doc.DocumentNode.SelectSingleNode("//h5[@class='znaki']");
        if (dateNode != null)
        {
            var dateText = dateNode.InnerText;
            
            var parts = dateText.Split(new[] { " - " }, StringSplitOptions.None);
            var datePart = parts.Length > 0 ? parts[0].Trim() : string.Empty;

            
            if (DateTime.TryParse(datePart, out DateTime extractedDate))
            {
              //  Console.WriteLine("Data: " + extractedDate.ToString("yyyy-MM-dd"));
                todayDate = extractedDate;
            }
            else
            {
                Console.WriteLine("Nie udało się wydobyć daty.");
            }
        }

        
        var operationRows = doc.DocumentNode.SelectNodes("//tr[td[@class='data']]");
        if (operationRows != null)
        {
            foreach (var row in operationRows)
            {
                var cells = row.SelectNodes("td[@class='data']");
                if (cells != null && cells.Count >= 2)
                {
                    var timeNode = cells[0]; 
                    var descriptionNode = cells[1]; 

                    var time = timeNode.InnerText.Trim();
                    var description = descriptionNode.InnerText.Trim();

                    //Console.WriteLine($"Czas operacji: {time}");
                    //Console.WriteLine($"Opis operacji: {description}");
                    operations.Add(new Operation(todayDate, time, description));
                }
            }
        }

        foreach (var operation in operations) {
            Console.WriteLine(operation.ToString());

        }
        
        
    }

    static void ExtractAndProcessHtmlFromEmail(GmailService service, Message emailInfoResponse)
    {
        string htmlContent = "";
        if (emailInfoResponse.Payload.Parts == null && emailInfoResponse.Payload.Body != null)
        {
            htmlContent = DecodeBase64String(emailInfoResponse.Payload.Body.Data);
        }
        else
        {
            htmlContent = GetNestedHtmlParts(emailInfoResponse.Payload.Parts, "");
        }

        if (!string.IsNullOrEmpty(htmlContent))
        {
            ProcessEmailHtmlContent(htmlContent);
        }
    }
    static string GetNestedHtmlParts(IList<MessagePart> parts, string accumulatedHtml)
    {
        foreach (var part in parts)
        {
            if (part.MimeType == "text/html")
            {
                accumulatedHtml += DecodeBase64String(part.Body.Data);
            }
            else if (part.Parts != null)
            {
                accumulatedHtml = GetNestedHtmlParts(part.Parts, accumulatedHtml);
            }
        }
        return accumulatedHtml;
    }
    static async Task<string> GetAttachmentHtml(GmailService service, string userId, string messageId, string attachmentId)
    {
        try
        {
            var attachment = await service.Users.Messages.Attachments.Get(userId, messageId, attachmentId).ExecuteAsync();
            if (attachment != null)
            {
                
                string decodedHtml = DecodeBase64String(attachment.Data);
                return decodedHtml;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        return null;
    }

    static string DecodeBase64String(string s)
    {
        
        if (string.IsNullOrWhiteSpace(s))
            return "";

        
        string base64Corrected = s.Replace("-", "+").Replace("_", "/");

        
        int mod4 = base64Corrected.Length % 4;
        if (mod4 > 0)
        {
            base64Corrected += new string('=', 4 - mod4);
        }

        
        try
        {
            byte[] base64Bytes = Convert.FromBase64String(base64Corrected);
            return Encoding.UTF8.GetString(base64Bytes);
        }
        catch (FormatException ex)

        {
            
            Console.WriteLine($"Nie można zdekodować ciągu Base64: {ex.Message}");
        }
        return null;
    }


}