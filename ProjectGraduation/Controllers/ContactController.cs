using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using System.Drawing;

namespace ProjectGraduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly GraduationProjectContext context;
        public ContactController(GraduationProjectContext context)
        {
            this.context = context;
        }
        [HttpGet("GetTeamMembers")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await context.Contacts.ToListAsync();
            if (contacts == null)
                return Ok("There is No Data yet!");
            var list = new List<ContactDto>();
            foreach (var contact in contacts)
            {
                list.Add(new ContactDto(contact));
            }
            return Ok(list);
        }


        [HttpGet("GetContactInfo")]
        public async Task<IActionResult> GetContactInfo()
        {
            var contacts  = await context.Contacts.Where(C=>C.IsTechnicalSupport == true).ToListAsync();
            if (contacts == null)
                return Ok("There is No Data yet!");
            var list = new List<ContactDto>();
            foreach (var contact in contacts)
            {
                list.Add(new ContactDto(contact));
            }
            return Ok(list);
        }


        [HttpPost("AddContact")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddContact([FromForm] ContactModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
            if (!(model.Image != null && allowedExtensions.Contains(fileExtension)))
            {
                return BadRequest("Invalid file type. Please upload an image.");
            }
            try
            {
                using var stream = new MemoryStream();
                await model.Image.CopyToAsync(stream);
                var contact = new Contact
                {
                    Email = model.Email,
                    FacebookAccount = model.FacebookAccount,
                    LinkedInAccount = model.LinkedInAccount,
                    Image = stream.ToArray(),
                    Specialty = model.Specialty,
                    Name = model.Name
                };
                await context.Contacts.AddAsync(contact);
                await context.SaveChangesAsync();
                return Ok("Contact is Added Successfully");
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return BadRequest("Couldn't Add Contact Due to" + msg);
            }

        }


        [HttpDelete("DeleteContact/{ContactId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletContact(string ContactId)
        {
            var contact = await context.Contacts.FirstOrDefaultAsync(C=>C.Id == ContactId);
            if (contact is null)
                return BadRequest("There is no Contact with id: " + ContactId);
            try
            {
                context.Contacts.Remove(contact);
                await context.SaveChangesAsync();
                return Ok("Contact is Deleted Successfully");
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return BadRequest("Error Deleting Contact Due to" + msg);
            }
        }


        [HttpPut("SetInSupportTeam/{ContactId}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> SetAsTechnicalSupport(string ContactId)
        {
            var Contact = await context.Contacts.FirstOrDefaultAsync(c => c.Id == ContactId);
            if (Contact is null)
                return BadRequest("There is no Contact with Id:" + ContactId);
            Contact.IsTechnicalSupport = true;
            try
            {
                await context.SaveChangesAsync();
                return Ok("Contact is Sat in Technical Support Team Successfully");
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return BadRequest("Error Setting Contact Due to" + msg);
            }
        }


        [HttpPut("EditContact/{ContactId}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> EditContact([FromRoute] string ContactId ,[FromForm] ContactModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Contact = await context.Contacts.FirstOrDefaultAsync(p => p.Id == ContactId);
            if (Contact == null)
                return BadRequest("There is no Contact with Id: " + ContactId);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
            if (!(model.Image != null && allowedExtensions.Contains(fileExtension)))
            {
                return BadRequest("Invalid file type. Please upload an image with extension [.jpg ,.jpeg , .png ");
            }
            try
            {
                using var stream = new MemoryStream();
                await model.Image.CopyToAsync(stream);
                {
                    Contact.Email = model.Email;
                    Contact.FacebookAccount = model.FacebookAccount;
                    Contact.LinkedInAccount = model.LinkedInAccount;
                    Contact.Image = stream.ToArray();
                    Contact.Specialty = model.Specialty;
                    Contact.Name = model.Name;
                }
                await context.SaveChangesAsync();
                return Ok("Contact is Updated Successfully");
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return BadRequest("Couldn't Update Contact Due to" + msg);
            }
        }
    }
}
