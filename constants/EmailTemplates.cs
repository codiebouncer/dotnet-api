using System;

namespace PropMan.EmailTemplates
{
    public static class EmailTemplates
    {
        private const string BrandHeader = "<h2>Welcome to Kofi Agyei's api</h2>";

        // ========== OVERDUE INVOICE ==========

        public static string OverdueInvoiceSubject() => "Overdue Invoice Reminder";

        public static string OverdueInvoiceBody(string tenantName, decimal balance, DateTime dueDate)
        {
            var message =
                $"Dear {tenantName}, you have defaulted on your invoice of the amount GHC{balance} " +
                $"which was due on {dueDate:MMMM dd, yyyy}. Please make payment to avoid any penalties";

            return $"{BrandHeader}<p>{message}</p>";
        }

        // ========== PAYMENT REMINDER (UPCOMING) ==========

        public static string PaymentReminderSubject() => "Payment Reminder";

        public static string PaymentReminderBody(string tenantName, decimal balance, DateTime dueDate)
        {
            var message =
                $"Dear {tenantName}, your invoice of amount GHC{balance} is due on {dueDate:MMMM dd, yyyy}. " +
                "Please make payment to avoid late fees.";

            return $"{BrandHeader}<p>{message}</p>";
        }

        // ========== INVOICE GENERATED ==========

        public static string InvoiceGeneratedSubject() => "Invoice Generated";

        public static string InvoiceGeneratedBody(
            string tenantName,
            decimal amount,
            decimal balance,
            DateTime dueDate)
        {
            var message =
                $"Dear {tenantName}, your invoice of GHC{amount} has been generated. " +
                $"Balance: GHC{balance}. Please pay before {dueDate:MMMM dd, yyyy}.";

            return $"{BrandHeader}<p>{message}</p>";
        }
    }
}
