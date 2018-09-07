using System;
using System.Threading;

namespace TickMeHelpers
{
    public static class PaymentManagement
    {
        public static PaymentData ProcessPayment(PaymentData paymentData, bool succeed = true, int waitTimeMs = 10000)
        {
            Thread.Sleep(waitTimeMs);
            if(succeed)
            {
                paymentData.TransactionSuccessful = true;
                paymentData.TransactionData = $"{{\"Result\":\"OK\", \"TransactionDate\":\"{DateTime.UtcNow}\" }}";
            }
            else
            {
                paymentData.TransactionSuccessful = false;
                paymentData.TransactionData = $"{{\"Result\":\"ERROR\", \"TransactionDate\":\"{DateTime.UtcNow}\", \"Reason\":\"Too much money on card!\" }}";
            }
            return paymentData;
        }
    }
}
