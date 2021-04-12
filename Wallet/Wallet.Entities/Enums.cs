namespace Wallet.Business
{
    public enum EmailTemplatesEnum
    {
        FixedTermDepositClosed = 1, // That "= 1" is to make the enum starts at 1 in order to match the database model (IDs in database starts at 1)
        RefundRequestCanceled,
        RefundRequestCreated,
        RefundRequestStatusChanged
        /* New items here at the end; otherwise, modify the 
         * Database/Scripts/Script_Email_Templates.sql file to match
         */
    }
}
