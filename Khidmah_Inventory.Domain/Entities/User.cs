using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool EmailConfirmed { get; private set; } = false;
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public string? AvatarUrl { get; private set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<UserCompany> UserCompanies { get; private set; } = new List<UserCompany>();

    private User() { }

    public User(
        Guid companyId,
        string email,
        string userName,
        string passwordHash,
        string firstName,
        string lastName,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Email = email;
        UserName = userName;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber, Guid? updatedBy = null)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdateAuditInfo(updatedBy);
    }

    public void ChangePassword(string newPasswordHash, Guid? updatedBy = null)
    {
        PasswordHash = newPasswordHash;
        UpdateAuditInfo(updatedBy);
    }

    public void SetRefreshToken(string? refreshToken, DateTime? expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        UpdateAuditInfo(updatedBy);
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void UpdateAvatar(string? avatarUrl, Guid? updatedBy = null)
    {
        AvatarUrl = avatarUrl;
        UpdateAuditInfo(updatedBy);
    }
}

