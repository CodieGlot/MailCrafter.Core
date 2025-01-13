namespace MailCrafter.Domain;
public enum EmailAccountStatus
{
    NotVerified = 0,              // Account not yet verified
    Active = 1,                   // Account is active and valid
    Suspended = 2,                // Account is temporarily restricted
    Revoked = 3,                  // Account is permanently invalidated
    Deactivated = 4,              // Account was manually deactivated, potentially by the user
    Expired = 5,                  // Account is expired
}
