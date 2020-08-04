using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Data
{
    public class ContactRepository : IContactRepository
    {
        private readonly DataContext _context;

        public ContactRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GlobalChatDto>> GetChatRecords()
        {
            List<GlobalChatDto> chatRecords = new List<GlobalChatDto>();

            var records = await _context.GlobalChats.OrderByDescending(x => x.Id).Take(25).ToListAsync();

            foreach (var chat in records)
            {
                GlobalChatDto dto = new GlobalChatDto
                {
                    ChatText = chat.ChatText,
                    ChatTime = chat.ChatTime,
                    Username = chat.Username
                };
                chatRecords.Add(dto);
            }
            return chatRecords;
        }

        public async Task<bool> SaveChatRecord(GlobalChatDto chatDto)
        {
            // convert string to int
            var temp = chatDto.Username;
            var userId = Convert.ToInt32(temp);

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == userId);

            GlobalChat chatRecord = new GlobalChat
            {
                ChatText = chatDto.ChatText,
                Username = team.Mascot,
                ChatTime = chatDto.ChatTime
            };

            await _context.GlobalChats.AddAsync(chatRecord);
            return await _context.SaveChangesAsync() > 0;
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