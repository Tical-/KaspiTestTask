using Swashbuckle.AspNetCore.Annotations;
using WebApi.Entities;
using WebApi.Models.Payments;

namespace WebApi.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private IPaymentsService _paymentsService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;
    private IHttpContextAccessor _contextAccessor;
    public PaymentsController(
        IPaymentsService paymentsService,
        IMapper mapper,
        IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
    {
        _paymentsService = paymentsService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _contextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// Выставление счета для покупателя
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [SwaggerOperation(Summary = "Выставление счета для покупателя")]
    [HttpPost("AddPaymentForBuyer")]
    public async Task<IActionResult> AddPaymentForBuyer(PaymentForBuyerRequest model)
    {
        var user = (User)_contextAccessor.HttpContext.Items["User"];
        if (user.IsSeller == false)
            throw new Exception("CurrentUser is not a Seller");
        await _paymentsService.AddPaymentForBuyer(model, user.Id);
        return Ok(new { message = "Payment added successfully" });
    }

    /// <summary>
    /// Выставленный счёт отображается на странице "счета на оплату" у продавца и на странице "платежи" у покупателя
    /// </summary>
    [SwaggerOperation(Summary = "Выставленный счёт отображается на странице \"счета на оплату\" у продавца и на странице \"платежи\" у покупателя")]
    [HttpPost("GetMyPayments")]
    public async Task<IActionResult> GetMyPayments()
    {
        var user = (User)_contextAccessor.HttpContext.Items["User"];
        var result = await _paymentsService.GetMyPayments(user.Id);
        return Ok(result);
    }

    /// <summary>
    /// Продавец может отменить счёт
    /// </summary>
    [SwaggerOperation(Summary = "Продавец может отменить счёт")]
    [HttpPost("CancelPayment")]
    public async Task<IActionResult> CancelPayment(long PaymentId)
    {
        var user = (User)_contextAccessor.HttpContext.Items["User"];
        if (user.IsSeller == false)
            throw new Exception("CurrentUser is not a Seller");
        await _paymentsService.CancelPayment(PaymentId, user.Id);
        return Ok();
    }

    /// <summary>
    /// покупатель может отклонить счет
    /// </summary>
    [SwaggerOperation(Summary = "Покупатель может отклонить счет")]
    [HttpPost("RejectPayment")]
    public async Task<IActionResult> RejectPayment(long PaymentId)
    {
        var user = (User)_contextAccessor.HttpContext.Items["User"];
        if (user.IsSeller == true)
            throw new Exception("CurrentUser is not a Buyer");
        await _paymentsService.RejectPayment(PaymentId, user.Id);
        return Ok();
    }

    /// <summary>
    /// У покупателя есть 24 часа, чтобы оплатить счёт. 
    /// </summary>
    [SwaggerOperation(Summary = "У покупателя есть 24 часа, чтобы оплатить счёт. ")]
    [HttpPost("MakePayment")]
    public async Task<IActionResult> MakePayment(long PaymentId)
    {
        var user = (User)_contextAccessor.HttpContext.Items["User"];
        if (user.IsSeller == true)
            throw new Exception("CurrentUser is not a Buyer");
        await _paymentsService.MakePayment(PaymentId, user.Id);
        return Ok();
    }

    /// <summary>
    /// Оплаченный счёт переносится на страницу "история платежей"
    /// </summary>
    [SwaggerOperation(Summary = "Оплаченный счёт переносится на страницу \"история платежей\"")]
    [HttpPost("GetHistoryPayments")]
    public async Task<IActionResult> GetHistoryPayments()
    {
        var user = (User)_contextAccessor.HttpContext.Items["User"];
        var result = await _paymentsService.GetHistoryPayments(user.Id);
        return Ok(result);
    }
}