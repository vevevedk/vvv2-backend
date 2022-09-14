using System.ComponentModel;

namespace Veveve.Domain.Exceptions;

/// <summary>
/// Error messages to use when business rules are broken.
/// These should be encapsulated in an exception - for instance the BusinessRuleException.
/// </summary>
public enum ErrorCodesEnum
{
    // ==== GENERIC 0-10000 ============
    [Description("An unhandled error occured. Staff have been notified")]
    GENERIC_INTERNAL = 101000,
    [Description("Unable to process request. Validation of properties failed")]
    GENERIC_VALIDATION = 102000,
    [Description("Client is not authorized")]
    GENERIC_UNAUTHORIZED = 103000,
    [Description("Client is forbidden.")]
    GENERIC_FORBIDDEN = 103100,
    [Description("The property {0} is in an incorrect format")]
    GENERIC_PROPERTY_BAD_FORMAT = 105000,

    // ==== User 10000-20000 ============
    [Description("The provided email or password is incorrect")]
    USER_LOGIN_EMAIL_OR_PASSWORD_INVALID = 10101,
    [Description("The reset password token is invalid. Might have already been used")]
    USER_RESETPASSWORD_TOKEN_INVALID = 10201,
    [Description("The provided email already exists and cannot be used")]
    USER_EMAIL_ALREADY_EXIST = 10301,
    [Description("The provided email does not exist")]
    USER_EMAIL_DOESNT_EXIST = 10302,
    [Description("The User doesn't exist")]
    USER_ID_DOESNT_EXIST = 10303,


    // ==== Client 20000-30000 ============
    [Description("The client doesn't exist")]
    CLIENT_ID_DOESNT_EXIST = 20101,
    [Description("The client name already exists")]
    CLIENT_NAME_ALREADY_EXISTS = 20301,

    // ==== Account 30000-40000 ============
    [Description("The User doesn't exist")]
    ACCOUNT_ID_DOESNT_EXIST = 30101,
    [Description("The provided google ads id already exists")]
    ACCOUNT_GOOGLEADSID_ALREADY_EXIST = 30302,
}