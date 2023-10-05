using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbitt.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MicroRabbit.Banking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        private readonly IAccountService accountService;

        public BankingController(IAccountService _accountService) 
        {
            this.accountService = _accountService;
        }

        // GET: api/Banking
        [HttpGet("getaccounts")]
        public ActionResult<IEnumerable<Account>> Get()
        {
            return Ok(this.accountService.GetAccounts().ToList());
        }
    }
}
