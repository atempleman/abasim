using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _repo;

        public ContactController(IContactRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("savecontact")]
        public async Task<IActionResult> SaveContact(ContactFormDto contactFormDto)
        {
            var createdForm = await _repo.SaveContactForm(contactFormDto);
            return StatusCode(201);
        }

        [HttpPost("savechatrecord")]
        public async Task<IActionResult> SaveChatRecord(GlobalChatDto chatDto)
        {
            var result = await _repo.SaveChatRecord(chatDto);
            return Ok(result);
        }

        [HttpGet("getchatrecords")]
        public async Task<IActionResult> GetChatRecords()
        {
            var records = await _repo.GetChatRecords();
            return Ok(records);
        }
    }
}