using EcoTrip.Models.DtoS;
using EcoTrip.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMail mail;
        public MailController(IMail mail)
        {
            this.mail = mail;
        }


        /// <summary>
        /// Email küldése POST
        /// </summary>
        /// <remarks>
        /// Email POST
        /// </remarks>
        [HttpPost]
        public ActionResult SendMail(SendMailDto sendMailDto)
        {
            mail.SendMail(sendMailDto);
            return Ok(new { message = "Sikeres mail küldés." });
        }
    }
}
