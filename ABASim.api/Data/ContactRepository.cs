using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public class ContactRepository : IContactRepository
    {
        private readonly DataContext _context;

        public ContactRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveContactForm(ContactFormDto contactFormDto)
        {
            ContactForm contactForm = new ContactForm 
            {
                Name = contactFormDto.Name,
                Email = contactFormDto.Email,
                Contact = contactFormDto.Contact
            };

            await _context.ContactForms.AddAsync(contactForm);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}