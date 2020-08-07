export interface InboxMessage {
    id: number;
    senderId: number;
    senderName: string;
    senderTeam: string;
    receiverId: number;
    receiverName: string;
    receiverTeam: string;
    subject: string;
    body: string;
    messageDate: string;
    isNew: number;
}
