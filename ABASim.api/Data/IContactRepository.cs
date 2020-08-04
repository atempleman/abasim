using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;

namespace ABASim.api.Data
{
    public interface IContactRepository
    {
        Task<bool> SaveContactForm(ContactFormDto contactFormDto);

        Task<bool> SaveChatRecord(GlobalChatDto chatDto);

        Task<IEnumerable<GlobalChatDto>> GetChatRecords();
    }
}