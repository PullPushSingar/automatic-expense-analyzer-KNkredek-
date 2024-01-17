﻿using Google.Apis.Auth.OAuth2;
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

        
        UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List("me");

        
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
                                Console.WriteLine("HTML content of the attachment:");
                                Console.WriteLine(htmlContent);
                               
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

        
        var nodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                Console.WriteLine($"Link found: {node.Attributes["href"].Value}");
                
            }
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