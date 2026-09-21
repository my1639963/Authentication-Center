namespace AuthCenter.Domain.Enums;

public enum UserStatus
{
    Normal = 0,
    Disabled = 1,
    Locked = 2
}

public enum LoginResult
{
    Success = 0,
    Failed = 1
}

public enum EventResult
{
    Success = 0,
    Failed = 1
}

public enum LoginType
{
    Password,
    RefreshToken,
    ClientCredentials,
    SSO
}

public enum OrganizationStatus
{
    Normal = 0,
    Disabled = 1
}

public enum DeletedFlag
{
    Active = 0,
    Deleted = 1
}
