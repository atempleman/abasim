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

        public async Task<InboxMessageCountDto> CountOfMessages(int teamId)
        {
            var messages = await _context.InboxMessages.Where(x => x.ReceiverId == teamId && x.IsNew == 1).ToListAsync();
            InboxMessageCountDto com = new InboxMessageCountDto
            {
                CountOfMessages = messages.Count
            };
            return com;
        }

        public async Task<bool> DeleteInboxMessage(int messageId)
        {
            var msg = await _context.InboxMessages.FirstOrDefaultAsync(x => x.Id == messageId);
            _context.Remove(msg);
            return await _context.SaveChangesAsync() > 0;
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

        public async Task<IEnumerable<InboxMessageDto>> GetInboxMessages(int teamId)
        {
            List<InboxMessageDto> messages = new List<InboxMessageDto>();

            var inboxMessages = await _context.InboxMessages.Where(x => x.ReceiverId == teamId).ToListAsync();

            foreach (var im in inboxMessages)
            {
                InboxMessageDto dto = new InboxMessageDto
                {
                    Id = im.Id,
                    SenderId = im.SenderId,
                    SenderName = im.SenderName,
                    SenderTeam = im.SenderTeam,
                    ReceiverId = im.ReceiverId,
                    ReceiverName = im.ReceiverName,
                    ReceiverTeam = im.ReceiverTeam,
                    Subject = im.Subject,
                    Body = im.Body,
                    MessageDate = im.MessageDate,
                    IsNew = im.IsNew
                };
                messages.Add(dto);
            }
            return messages;
        }

        public async Task<bool> MarkMessageRead(int messageId)
        {
            var message = await _context.InboxMessages.FirstOrDefaultAsync(x => x.Id == messageId);
            message.IsNew = 0;
            _context.Update(message);
            return await _context.SaveChangesAsync() > 0;
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

        public async Task<bool> SendInboxMessage(InboxMessageDto message)
        {
            InboxMessage im = new InboxMessage
            {
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                SenderTeam = message.SenderTeam,
                ReceiverId = message.ReceiverId,
                ReceiverName = message.ReceiverName,
                ReceiverTeam = message.ReceiverTeam,
                Subject = message.Subject,
                Body = message.Body,
                MessageDate = message.MessageDate,
                IsNew = message.IsNew
            };
            await _context.AddAsync(im);
            return await _context.SaveChangesAsync() > 1;
        }
    }
}