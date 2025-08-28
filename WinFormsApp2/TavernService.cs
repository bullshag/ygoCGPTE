namespace WinFormsApp2
{
    public static class TavernService
    {
        public static void NotifyPartyHired(int ownerId)
        {
            MailService.SendMail(null, ownerId, "Party Hired", "Your party has been hired from the tavern.");
        }

        public static void NotifyPartyReturned(int ownerId)
        {
            MailService.SendMail(null, ownerId, "Party Returned", "Your party has returned to the tavern.");
        }
    }
}
