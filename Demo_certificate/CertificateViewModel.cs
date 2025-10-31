using System;
using System.Collections.Generic;

namespace DPL_project.DemoCertificate
{
    public class CertificateViewModel
    {
        public CertificateDocumentViewModel Document { get; set; }
        public string DocumentQr { get; set; } = string.Empty; // Placeholder; real QR generated if empty
        public RenderedMetadata RenderedMetadata { get; set; } = new RenderedMetadata { Pages = "3 pages", FileSize = "1 MB" };

        public CertificateViewModel()
        {
            Document = new CertificateDocumentViewModel();
        }
    }

    public class CertificateDocumentViewModel
    {
        public string Filename { get; set; } = "sample.pdf";
        public string Id { get; set; } = "9C04C235";
        public DocumentTransaction DocumentTransaction { get; set; } = new DocumentTransaction();
        public DateTime DocumentSentAt { get; set; } = DateTime.Now.AddDays(-2);
        public DateTime? DocumentExecutedAt { get; set; } = DateTime.Now.AddDays(-1);
        public User User { get; set; } = new User();
        public string IdentityMethod { get; set; } = "Email";
        public string DistributionMethod { get; set; } = "Email";
        public string DocumentSignedBaseFileChecksum { get; set; } = "XYZ987654werfsdfv dkfvnsfv dkfcv dv";
        public bool SignerSequencing { get; set; } = true;
        public bool Passcode { get; set; } = false;
        public string Name { get; set; } = "Sample Document";
        public Upload Upload { get; set; } = new Upload();
        public List<Signer> Signers { get; set; } = new List<Signer> { new Signer() };
        public List<Audit> PublicAudits { get; set; } = new List<Audit> { new Audit() };
    }

    public class DocumentTransaction
    {
        public Workflow Workflow { get; set; } = new Workflow();
    }
    public class Workflow
    {
        public string Name { get; set; } = "Signature";
    }
    public class User
    {
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public string TimeFormat { get; set; } = "hh:mm tt";
        public string TimeZone { get; set; } = "EST";
    }
    public class Upload
    {
        public string Filename { get; set; } = "sample.pdf";
        public BaseFile BaseFile { get; set; } = new BaseFile();
    }
    public class BaseFile
    {
        public string ContentType { get; set; } = "application/pdf";
        public string OriginalChecksum { get; set; } = "ABCDEF123456";
    }
    public class Signer
    {
        public string Name { get; set; } = "John Doe";
        public string Email { get; set; } = "john@example.com";
        public int Sequence { get; set; } = 1;
        public List<Component> Components { get; set; } = new List<Component> { new Component() };
        public string Status { get; set; } = "Signed";
        public bool Signed { get; set; } = true;
        public Response Response { get; set; } = new Response();
        public DateTime? SignedAt { get; set; } = DateTime.Now.AddDays(-1);
        public DateTime? ViewedAt { get; set; } = DateTime.Now.AddDays(-2);
        public DateTime? IdentityAuthenticatedAt { get; set; } = DateTime.Now.AddDays(-2);
    }
    public class Component
    {
        public int Count { get; set; } = 2;
    }
    public class Response
    {
        public string FingerprintChecksum { get; set; } = "f53cdbb5f9a4d8e11bf11161202b945";
        public string IpAddress { get; set; } = "192.168.1.1";
        public string Device { get; set; } = "Chrome on Mac";
        public List<Signature> Signatures { get; set; } = new List<Signature> { new Signature() };
    }
    public class Signature
    {
        public string SignatureType { get; set; } = "drawn";
        public string ImageUrl { get; set; } = "signature.png";
        public string Id { get; set; } = "SIG123";
        public string ReferenceId { get; set; } = "REF123";
        public List<object> ParsedJsonData { get; set; } = new List<object> { new object() };
    }
    public class Audit
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now.AddDays(-2);
        public string Payload { get; set; } = "Anshul Gour (anshul.gour@progress.com) created document 'Sample_Sharefile_Pdf.pdf' on Safari via Mac from287687";
    }
    public class RenderedMetadata
    {
        public string Pages { get; set; } = "5";
        public string FileSize { get; set; } = "34";
    }

    // Extension methods and helpers for Razor compatibility
    public static class RazorExtensions
    {
        public static string Sanitize(this string input) => input ?? string.Empty;
        public static string Humanize(this string input) => input ?? string.Empty;
        public static string Titleize(this string input) => input ?? string.Empty;
        public static string Capitalize(this string input) => string.IsNullOrEmpty(input) ? string.Empty : char.ToUpper(input[0]) + input.Substring(1);
    }
    public static class DateTimeFormatter
    {
        public static string Timestamp(DateTime? date, string dateFormat, string timeFormat, string timeZone, bool showTimeZone)
        {
            if (date == null) return "N/A";
            var dt = date.Value;
            var formatted = dt.ToString($"{dateFormat} {timeFormat}");
            return showTimeZone ? $"{formatted} {timeZone}" : formatted;
        }
    }
}
