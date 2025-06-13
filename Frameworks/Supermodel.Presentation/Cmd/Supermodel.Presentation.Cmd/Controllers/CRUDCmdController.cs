using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pluralize.NET.Core;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.Cmd.ConsoleOutput;
using Supermodel.Presentation.Cmd.Models;
using Supermodel.Presentation.Cmd.Rendering;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Cmd.Controllers;

public class CRUDCmdController<TEntity, TCmdModel, TDataContext>
    (string detailTitle, string? listTitle = null) : CRUDCmdController<TEntity, TCmdModel, TCmdModel, TDataContext>(
        detailTitle, listTitle)
    where TEntity : class, IEntity, new()
    where TCmdModel : CmdModelForEntity<TEntity>, new()
    where TDataContext : class, IDataContext, new();

public class CRUDCmdController<TEntity, TDetailMvcModel, TListMvcModel, TDataContext>
    where TEntity : class, IEntity, new()
    where TDetailMvcModel : CmdModelForEntity<TEntity>, new()
    where TListMvcModel : CmdModelForEntity<TEntity>, new()
    where TDataContext : class, IDataContext, new()
{
    #region Constructors
    public CRUDCmdController(string detailTitle, string? listTitle = null)
    { 
        DetailTitle = detailTitle;
        ListTitle = listTitle ?? new Pluralizer().Pluralize(DetailTitle);
    }
    #endregion

    #region Action Methods
    public virtual async Task RunCRUDAsync()
    {
        var currentColors = FBColors.FromCurrent();

        while(true)
        {
            await ListAsync();
                
            //method returns true if we quit
            if (await TryShowPromptAndProcessActionAsync()) break; 
        }

        currentColors.SetColors();
    }
    public virtual async Task ListAsync()
    {
        if (ClearScreenOnList) Console.Clear();

        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var entities = await GetItems().ToListAsync().ConfigureAwait(false);
            var mvcModels = new List<TListMvcModel>();
            mvcModels = await mvcModels.MapFromAsync(entities).ConfigureAwait(false);

            //Init mvc model if it requires async initialization
            foreach (var mvcModelItem in mvcModels)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync().ConfigureAwait(false);
            }

            ShowListTitle();
            foreach (var mvcModel in mvcModels)
            {
                CmdScaffoldingSettings.ListEntityId?.SetColors();
                Console.Write($"{mvcModel.Id}: "); 
                CmdScaffoldingSettings.DefaultListLabel?.SetColors();
                mvcModel.Label.WriteLineToConsole();
            }
        }
    }
    public virtual async Task ViewDetailAsync(long id)
    {
        TDetailMvcModel? mvcModelItem;            
        try
        {
            mvcModelItem = await CreateMvcModelAsync(id);
        }
        catch (Exception)
        {
            CmdScaffoldingSettings.InvalidValueMessage?.SetColors();
            Console.WriteLine($"ERROR: {DetailTitle} with ID={id} does not exist.");
            PressAnyKey();
            return;
        }
            
        Console.WriteLine();
        ShowViewDetailTitle(id);
        CmdRender.DisplayForModel(mvcModelItem);
        PressAnyKey();
    }
    public virtual async Task EditDetailAsync(long id)
    {
        TDetailMvcModel? mvcModelItem;            
        try
        {
            mvcModelItem = await CreateMvcModelAsync(id);
        }
        catch (Exception)
        {
            CmdScaffoldingSettings.InvalidValueMessage?.SetColors();
            Console.WriteLine($"ERROR: {DetailTitle} with ID={id} does not exist.");
            PressAnyKey();
            return;
        }

        Console.WriteLine();
        ShowEditDetailTitle(id);
        while(true)
        {
            await using (new UnitOfWork<TDataContext>())
            {
                var savedCtrlEscEnabled = CmdContext.CtrlEscEnabled;
                CmdContext.CtrlEscEnabled = true;
                try
                {
                    await EditMvcModelAsync(mvcModelItem).ConfigureAwait(false);
                    if (ClearScreenOnList) PressAnyKey();
                    return;
                }
                catch (ModelStateInvalidException ex)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction

                    //Init ex.Model fs it requires async initialization
                    if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync().ConfigureAwait(false);

                    CmdScaffoldingSettings.Prompt?.SetColors();
                    Console.WriteLine("Please fix the following validation errors:");
                    mvcModelItem = (TDetailMvcModel)ex.Model;
                    CmdRender.ShowValidationSummary(mvcModelItem, CmdScaffoldingSettings.ValidationErrorMessage, CmdScaffoldingSettings.Label, CmdScaffoldingSettings.Prompt);
                    Console.WriteLine();
                }
                catch (ShiftEscException)
                {
                    CmdScaffoldingSettings.Prompt?.SetColors();
                    Console.WriteLine();
                    Console.Write("Operation cancelled. ");
                    if (ClearScreenOnList) PressAnyKey();
                    else Console.WriteLine();
                    return;
                }
                finally
                {
                    CmdContext.CtrlEscEnabled = savedCtrlEscEnabled;
                }
            }
        }
    }
    public virtual async Task AddDetailAsync()
    {
        Console.WriteLine();
        ShowAddDetailTitle();
        var mvcModelItem = await CreateMvcModelAsync(0);

        while (true)
        {
            await using (new UnitOfWork<TDataContext>())
            {
                var savedCtrlEscEnabled = CmdContext.CtrlEscEnabled;
                CmdContext.CtrlEscEnabled = true;
                try
                {
                    await EditMvcModelAsync(mvcModelItem).ConfigureAwait(false);
                    if (ClearScreenOnList) PressAnyKey();
                    return;
                }
                catch (ModelStateInvalidException ex)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction

                    //Init ex.Model fs it requires async initialization
                    if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync().ConfigureAwait(false);

                    CmdScaffoldingSettings.Prompt?.SetColors();
                    Console.WriteLine("Please fix the following validation errors:");
                    mvcModelItem = (TDetailMvcModel)ex.Model;
                    CmdRender.ShowValidationSummary(mvcModelItem, CmdScaffoldingSettings.ValidationErrorMessage, CmdScaffoldingSettings.Label, CmdScaffoldingSettings.Prompt);
                    Console.WriteLine();
                }
                catch (ShiftEscException)
                {
                    CmdScaffoldingSettings.Prompt?.SetColors();
                    Console.WriteLine();
                    Console.Write("Operation cancelled. ");
                    if (ClearScreenOnList) PressAnyKey();
                    else Console.WriteLine();
                    return;
                }
                finally
                {
                    CmdContext.CtrlEscEnabled = savedCtrlEscEnabled;
                }
            }
        }
    }
    public virtual async Task DeleteDetailAsync(long id)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            try
            {
                var entityItem = await GetItemAsync(id).ConfigureAwait(false);
                entityItem.Delete();
            }
            catch (UnableToDeleteException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                CmdScaffoldingSettings.InvalidValueMessage?.SetColors();
                Console.WriteLine($"ERROR: {ex.Message}");
                PressAnyKey();
            }
            catch (Exception)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                CmdScaffoldingSettings.InvalidValueMessage?.SetColors();
                    
                Console.WriteLine("ERROR: Unable to delete. Most likely reason: references from other entities.");
                PressAnyKey();
            }
        }
    }
    #endregion

    #region Protected Methods & Properties
    protected virtual async Task<TEntity> GetItemAndCacheItAsync(long id)
    {
        var item = await GetItemAsync(id);
        UnitOfWorkContext.CustomValues[$"Item_{id}"] = item; //we cache this, for MvcModel validation
        return item;
    }
    protected virtual Task<TEntity> GetItemAsync(long id)
    {
        return GetItems().SingleAsync(x => x.Id == id);
    }
    protected virtual IQueryable<TEntity> GetItems()
    {
        var repo = (ILinqDataRepo<TEntity>)RepoFactory.Create<TEntity>();
        return repo.Items;        }
        
    //this method return true if we quit
    protected virtual async Task<bool> TryShowPromptAndProcessActionAsync()
    {
        ShowActionPrompt();
        while (true)
        {
            CmdScaffoldingSettings.CommandValue?.SetColors();
                
            string input;
            if (AllowQuit) input = ConsoleExt.EditLineAllCaps("", x => char.IsDigit(x) || "VEADQ".Contains(x)).Trim().ToUpper();
            else input = ConsoleExt.EditLineAllCaps("", x => char.IsDigit(x) || "VEAD".Contains(x)).Trim().ToUpper();
                
            if (input.StartsWith("V"))
            {
                var id = GetIdForCommand(input);
                if (id == null)
                {
                    PrintInvalidCommandTryAgain();
                    continue;
                }
                await ViewDetailAsync(id.Value);
                Console.WriteLine();
                return false;
            }
            if (input.StartsWith("E"))
            {
                var id = GetIdForCommand(input);
                if (id == null)
                {
                    PrintInvalidCommandTryAgain();
                    continue;
                }
                await EditDetailAsync(id.Value);
                Console.WriteLine();
                return false;
            }
            if (input == "A")
            {
                await AddDetailAsync();
                Console.WriteLine();
                return false;
            }
            if (input.StartsWith("D"))
            {
                var id = GetIdForCommand(input);
                if (id == null)
                {
                    PrintInvalidCommandTryAgain();
                    continue;
                }
                CmdScaffoldingSettings.Label?.SetColors();
                Console.Write($"Are you sure you want to delete {DetailTitle} with ID = {id}? ");
                CmdScaffoldingSettings.Value?.SetColors();
                if (!ConsoleExt.EditBool(false)) 
                {
                    Console.WriteLine();
                    return false;
                }
                await DeleteDetailAsync(id.Value);
                Console.WriteLine();
                return false;
            }
            if (AllowQuit && input == "Q") 
            {
                CmdScaffoldingSettings.Prompt?.SetColors();
                Console.WriteLine($"Quitting {ListTitle}...");
                return true;
            }

            PrintInvalidCommandTryAgain();
        }
    }

    protected virtual long? GetIdForCommand(string input)
    {
        long? id;
        if (input.Length == 1)
        {
            CmdScaffoldingSettings.Prompt?.SetColors();
            Console.Write("Pick ID: ");
            using(CmdContext.NewRequiredScope(true, "ID"))
            {
                CmdScaffoldingSettings.CommandValue?.SetColors();
                id = ConsoleExt.EditInteger((long?)null) ?? throw new Exception("ID == null: this should never happen!");
            }
        }
        else
        {
            if (long.TryParse(input[1..].Trim(), out var tmpId)) id = tmpId;
            else id = null;
        }
        return id;
    }
    protected virtual void ShowActionPrompt()
    {
        CmdScaffoldingSettings.Prompt?.SetColors();
        Console.Write("Pick a command (");

        CmdScaffoldingSettings.CommandValue?.SetColors();
        Console.Write("V");

        CmdScaffoldingSettings.Prompt?.SetColors();
        Console.Write("iew, ");

        CmdScaffoldingSettings.CommandValue?.SetColors();
        Console.Write("E");

        CmdScaffoldingSettings.Prompt?.SetColors();
        Console.Write("dit, ");

        CmdScaffoldingSettings.CommandValue?.SetColors();
        Console.Write("A");

        if (AllowQuit)
        {
            CmdScaffoldingSettings.Prompt?.SetColors();
            Console.Write("dd, ");

            CmdScaffoldingSettings.CommandValue?.SetColors();
            Console.Write("D");

            CmdScaffoldingSettings.Prompt?.SetColors();
            // ReSharper disable once StringLiteralTypo
            Console.Write("elete, or ");

            CmdScaffoldingSettings.CommandValue?.SetColors();
            Console.Write("Q");

            CmdScaffoldingSettings.Prompt?.SetColors();
            Console.Write("uit): ");
        }
        else
        {
            CmdScaffoldingSettings.Prompt?.SetColors();
            Console.Write("dd, or ");

            CmdScaffoldingSettings.CommandValue?.SetColors();
            Console.Write("D");

            CmdScaffoldingSettings.Prompt?.SetColors();
            // ReSharper disable once StringLiteralTypo
            Console.Write("elete): ");
        }
    }
    protected virtual void PrintInvalidCommandTryAgain()
    {
        CmdScaffoldingSettings.InvalidCommandMessage?.SetColors();
        Console.Write("Invalid command. ");

        CmdScaffoldingSettings.Prompt?.SetColors();
        Console.Write("Pick again: ");
    }

    protected virtual void ShowListTitle()
    {
        ShowTitle($"List of {ListTitle}", CmdScaffoldingSettings.Title);
    }
    protected virtual void ShowEditDetailTitle(long id)
    {
        ShowTitle($"Edit {DetailTitle} with ID = {id} (shift-esc to cancel)", CmdScaffoldingSettings.Title);
    }
    protected virtual void ShowViewDetailTitle(long id)
    {
        ShowTitle($"View {DetailTitle} with ID = {id}", CmdScaffoldingSettings.Title);
    }
    protected virtual void ShowAddDetailTitle()
    {
        ShowTitle($"Add New {DetailTitle} (shift-esc to cancel)", CmdScaffoldingSettings.Title);
    }
    protected virtual async Task<TDetailMvcModel> CreateMvcModelAsync(long id)
    {
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var mvcModelItem = new TDetailMvcModel();

            //Init mvc model if it requires async initialization
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync().ConfigureAwait(false);

            if (id == 0)
            {
                mvcModelItem = await mvcModelItem.MapFromAsync(new TEntity()).ConfigureAwait(false);
            }
            else
            {
                var entityItem = await GetItemAsync(id).ConfigureAwait(false);
                mvcModelItem = await mvcModelItem.MapFromAsync(entityItem).ConfigureAwait(false);
            }

            return mvcModelItem;
        }
    }
    //this methods will catch validation exceptions that happen during mapping from mvc to domain (when it runs validation for mvc model by creating a domain object)
    protected virtual async Task<Tuple<TEntity, TDetailMvcModel>> EditMvcModelAsync(TDetailMvcModel mvcModelItem)
    {
        try
        {
            mvcModelItem = CmdRender.EditForModel(mvcModelItem);
                
            CmdContext.ValidationResultList.Clear();
            var vrl = new ValidationResultList();
            if (!await AsyncValidator.TryValidateObjectAsync(mvcModelItem, new ValidationContext(mvcModelItem), vrl).ConfigureAwait(false)) CmdContext.ValidationResultList.AddValidationResultList(vrl);
            if (CmdContext.ValidationResultList.IsValid != true) throw new ModelStateInvalidException(mvcModelItem);

            TEntity entityItem;
            if (mvcModelItem.IsNewModel()) 
            {
                entityItem = new TEntity();
                entityItem.Add();
            }
            else
            {
                entityItem = await GetItemAndCacheItAsync(mvcModelItem.Id).ConfigureAwait(false);
            }
                
            entityItem = await mvcModelItem.MapToAsync(entityItem);
            if (CmdContext.ValidationResultList.IsValid != true) throw new ModelStateInvalidException(mvcModelItem);

            //Validation: we only run ValidateAsync() here because attribute - based validation is already picked up by the framework
            vrl = await mvcModelItem.ValidateAsync(new ValidationContext(mvcModelItem));
            if (vrl.Count != 0) throw new ValidationResultException(vrl);

            return Tuple.Create(entityItem, mvcModelItem);
        }
        catch (ValidationResultException ex)
        {
            CmdContext.ValidationResultList.AddValidationResultList(ex.ValidationResultList);
            throw new ModelStateInvalidException(mvcModelItem);
        }
    }
    protected virtual void PressAnyKey()
    {
        CmdScaffoldingSettings.Prompt?.SetColors();
        Console.WriteLine("Press any key...");
        while(!Console.KeyAvailable) { /* do nothing */ }
        Console.ReadKey(true);
    }
    protected virtual void ShowTitle(string title, FBColors? colors)
    {
        colors?.SetColors();
        Console.WriteLine(title);
        Console.WriteLine("".PadRight(title.Length).Replace(" ", "="));
    }
    #endregion

    #region Properties
    public string ListTitle { get; set; }
    public string DetailTitle { get; set; }
    public bool ClearScreenOnList { get; set; } = true;
    public bool AllowQuit { get; set; } = true;
    #endregion
}