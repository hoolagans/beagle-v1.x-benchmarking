using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;

namespace Supermodel.Persistence.Entities;

public abstract class UserEntity<TUserEntity, TDataContext> : Entity
    where TDataContext : class, IDataContext, new()
    where TUserEntity : UserEntity<TUserEntity, TDataContext>, new()
{
    #region Overrides
    //override to use a different hashing algorithm
    public virtual string HashPassword(string password, ref string? salt)
    {
        salt ??= Guid.NewGuid().ToString();
        var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.Unicode.GetBytes(password + salt));
        return Encoding.Unicode.GetString(hashBytes);
    }
    #endregion

    #region Validation
    public override async Task<ValidationResultList> ValidateAsync(ValidationContext validationContext)
    {
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        var vr = await base.ValidateAsync(validationContext) ?? new ValidationResultList();
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var repo = LinqRepoFactory.Create<TUserEntity>();
            if (repo.Items.Any(u => u.Username == Username && u.Id != Id)) vr.AddValidationResult(this, "User with this Username already exists", x => x.Username);
        }
        return vr;
    }
    #endregion

    #region Methods
    public bool PasswordEquals(string password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(PasswordHashValue) || string.IsNullOrEmpty(PasswordHashSalt)) return false;
        string? salt = PasswordHashSalt;
        return HashPassword(password, ref salt) == PasswordHashValue;
    }
    [NotMapped] public string Password
    {
        set
        {
            string? salt = null;
            PasswordHashValue = HashPassword(value, ref salt);
            PasswordHashSalt = salt!;
        }
    }
    #endregion
        
    #region Properties
    [Required, MaxLength(100)] public virtual string Username { get; set; } = "";
    [Required] public string PasswordHashValue { get; set; } = "";
    [Required] public string PasswordHashSalt { get; set; } = "";
    #endregion
}