using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Repository;
using Supermodel.ReflectionMapper;

namespace Supermodel.Persistence.Entities;

public abstract class Entity : IEntity
{
    #region Methods
    public virtual void Add()
    {
        var originalId = Id;
        try
        {
            AddInternal();
        }
        catch (Exception)
        {
            Id = originalId;
            throw;
        }
    }
    public virtual void Delete()
    {
        var originalId = Id;
        try
        {
            DeleteInternal();
        }
        catch (Exception)
        {
            Id = originalId;
            throw;
        }
    }
    public virtual bool IsNewModel()
    {
        return Id == 0;
    }

    public virtual Task BeforeSaveAsync(OperationEnum operation)
    { 
        return Task.CompletedTask; 
    }

    protected virtual void AddInternal()
    {
        RepoFactory.CreateForRuntimeType(GetType()).AddIEntity(this);
    }
    protected virtual void DeleteInternal()
    {
        RepoFactory.CreateForRuntimeType(GetType()).DeleteIEntity(this);
    }
    #endregion

    #region Validation
    public virtual Task<ValidationResultList> ValidateAsync(ValidationContext validationContext)
    {
        return Task.FromResult(new ValidationResultList());
    }
    #endregion

    #region Properties
    [NotRCompared] public virtual long Id { get; set; }
    #endregion
}