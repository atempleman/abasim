namespace ABASim.api.Dtos
{
    public class InboxMessageDto
    {

        public int Id { get; set; }
        
        public int SenderId { get; set; }

        public string SenderName { get; set; }

        public string SenderTeam { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverTeam { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string MessageDate { get; set; }

        public int IsNew { get; set; }
    }
}