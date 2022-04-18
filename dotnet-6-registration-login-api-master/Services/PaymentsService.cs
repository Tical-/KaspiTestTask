using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Payments;

namespace WebApi.Services
{
    public interface IPaymentsService
    {
        Task AddPaymentForBuyer(PaymentForBuyerRequest model, int userId);
        Task<IEnumerable<Payments>> GetMyPayments(int userId);
        Task CancelPayment(long PaymentId, int UserId);
        Task RejectPayment(long PaymentId, int UserId);
        Task MakePayment(long PaymentId, int UserId);
        Task<IEnumerable<Payments>> GetHistoryPayments(int userId);
    }
    public class PaymentsService : IPaymentsService
    {
        private DataContext _context;
        public PaymentsService(DataContext context)
        {
            _context = context;
        }
        public async Task MakePayment(long PaymentId, int UserId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(z => z.Id == PaymentId && z.ToUserId == UserId);
            if (payment == null)
                throw new Exception("Payment with current PaymentId for current user is not found");
            if (payment.IsRejected || payment.IsCanceled || payment.IsPayed)
                throw new Exception("Payment is Canceled or is Rejected or already Payed");
            //TODO:: Дернуть сервис по переводу денег
            payment.IsPayed = true;
            await _context.SaveChangesAsync();
        }
        public async Task RejectPayment(long PaymentId, int UserId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(z => z.Id == PaymentId && z.ToUserId == UserId);
            if (payment == null)
                throw new Exception("Payment with current PaymentId for current user is not found");
            payment.IsRejected = true;
            await _context.SaveChangesAsync();
        }
        public async Task CancelPayment(long PaymentId, int UserId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(z => z.Id == PaymentId && z.UserId == UserId);
            if (payment == null)
                throw new Exception("Payment with current PaymentId for current user is not found");
            payment.IsCanceled = true;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Payments>> GetMyPayments(int UserId)
        {
            return await _context.Payments.Where(z => (z.ToUserId == UserId || z.UserId == UserId) && z.IsPayed == false && z.IsCanceled == false && z.IsRejected == false && z.Date > DateTime.Now.AddDays(-1)).ToListAsync();
        }
        public async Task AddPaymentForBuyer(PaymentForBuyerRequest model, int userId)
        {
            if (model.Phone == null || model.Phone.Length != 12)
                throw new Exception("Phone Length is NULL or Phone Length != 12");
            if (userId == model.ToUserId)
                throw new Exception("Cant add Payment to self");
            var user = await _context.Users.FirstOrDefaultAsync(z => z.Phone == model.Phone);
            if (user == null)
                throw new Exception("User is not Found by Phone");

            _context.Payments.Add(new Payments()
            {
                Date = DateTime.Now,
                IsCanceled = false,
                IsPayed = false,
                IsRejected = false,
                Money = model.Money,
                UserId = userId,
                ToUserId = model.ToUserId,
            });
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Payments>> GetHistoryPayments(int UserId)
        {
            return await _context.Payments.Where(z => (z.ToUserId == UserId || z.UserId == UserId) && z.IsPayed == true).OrderByDescending(r=>r.Date).ToListAsync();
        }
    }
}
