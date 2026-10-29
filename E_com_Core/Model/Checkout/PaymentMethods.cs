using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Core.Model.Checkout
{
    public static class PaymentMethods
    {
        public const string CashOnDelivery = "CashOnDelivery";
        public const string InstaPay = "InstaPay";
        public const string VodafoneCash = "VodafoneCash";

        public static readonly string[] All = { CashOnDelivery, InstaPay, VodafoneCash };

        // الطرق اللي محتاجة رقم عملية
        public static bool RequiresReference(string method) =>
            method == InstaPay || method == VodafoneCash;
    }

    public static class OrderStatuses
    {
        public const string Pending = "Pending";                             // كاش
        public const string AwaitingPaymentReview = "AwaitingPaymentReview"; // إنستاباي / فودافون كاش
        public const string Confirmed = "Confirmed";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
        public const string Rejected = "Rejected";                           // التحويل مش موجود أو المبلغ غلط

        public static readonly string[] All =
        {
            Pending, AwaitingPaymentReview, Confirmed,
            Shipped, Delivered, Cancelled, Rejected
        };

        // مين يتحول لمين. أي حالة مش موجودة هنا (Delivered / Cancelled / Rejected) نهائية.
        private static readonly Dictionary<string, string[]> Transitions = new()
        {
            [AwaitingPaymentReview] = new[] { Confirmed, Rejected, Cancelled },
            [Pending] = new[] { Confirmed, Cancelled },
            [Confirmed] = new[] { Shipped, Cancelled },
            [Shipped] = new[] { Delivered },
        };

        public static bool CanTransition(string from, string to) =>
            Transitions.TryGetValue(from, out var next) && next.Contains(to);
    }
}