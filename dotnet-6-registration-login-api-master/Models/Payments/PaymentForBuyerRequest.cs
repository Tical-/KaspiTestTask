namespace WebApi.Models.Payments
{
    public class PaymentForBuyerRequest
    {
        public decimal Money { get; set; }
        public string Phone { get; set; }
        public int ToUserId { get; set; }
    }


}
