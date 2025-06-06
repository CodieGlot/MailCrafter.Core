﻿using MailCrafter.Domain;
using MailCrafter.Repositories;
using MailCrafter.Utils.Extensions;
using MailCrafter.Utils.Helpers;
using System.Linq.Expressions;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace MailCrafter.Services;

public class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _userRepository;
    private readonly IAesEncryptionHelper _aesEncryptionHelper;
    private readonly ILogger<AppUserService> _logger;

    public AppUserService(
        IAppUserRepository userRepository, 
        IAesEncryptionHelper aesEncryptionHelper,
        ILogger<AppUserService> logger)
    {
        _userRepository = userRepository;
        _aesEncryptionHelper = aesEncryptionHelper;
        _logger = logger;
    }

    public async Task<AppUserEntity?> GetById(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<AppUserEntity?> GetByUsernameOrEmail(string usernameOrEmail)
    {
        Expression<Func<AppUserEntity, bool>> filter = usernameOrEmail.IsValidEmail()
            ? user => user.Email == usernameOrEmail
            : user => user.Username == usernameOrEmail;
        return await _userRepository.GetByPropertyAsync(filter);
    }

    public async Task<MongoInsertResult> Create(AppUserEntity user)
    {
        return await _userRepository.CreateAsync(user);
    }

    public async Task<MongoUpdateResult> Update(AppUserEntity user)
    {
        try
        {
            _logger.LogInformation("Attempting to update user with ID: {UserId}", user.ID);
            var result = await _userRepository.ReplaceAsync(user.ID, user);
            _logger.LogInformation("Update operation completed. Modified count: {ModifiedCount}", result.ModifiedCount);
            
            var updateResult = new MongoUpdateResult(new UpdateResult.Acknowledged(result.ModifiedCount, result.ModifiedCount, null));
            _logger.LogInformation("Update result created. IsSuccessful: {IsSuccessful}", updateResult.IsSuccessful);
            
            return updateResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", user.ID);
            throw;
        }
    }

    Task<MongoReplaceResult> IBasicOperations<AppUserEntity>.Update(AppUserEntity user)
    {
        return _userRepository.ReplaceAsync(user.ID, user);
    }

    public async Task<MongoDeleteResult> Delete(string id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<List<string>> GetEmailAccountsOfUser(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.EmailAccounts?.Select(account => account.Email).ToList() ?? [];
    }

    public async Task<MongoUpdateResult> AddEmailAccount(string id, EmailAccount emailAccount)
    {
        emailAccount.AppPassword = _aesEncryptionHelper.Encrypt(emailAccount.AppPassword);
        return await _userRepository.AddToArrayAsync(id, user => user.EmailAccounts, emailAccount);
    }

    public async Task<MongoUpdateResult> UpdateEmailPassword(string id, string email, string newPassword)
    {
        newPassword = _aesEncryptionHelper.Encrypt(newPassword);
        return await _userRepository.UpdateFieldInArrayWithConditionAsync(
                                        id,
                                        user => user.EmailAccounts,
                                        account => account.AppPassword,
                                        newPassword,
                                        account => account.Email == email);
    }

    public async Task<MongoUpdateResult> RemoveEmailAccount(string id, string email)
    {
        return await _userRepository.RemoveFromArrayAsync(id, user => user.EmailAccounts, account => account.Email == email);
    }

    public async Task<List<AppUserEntity>> GetPageQueryDataAsync(PageQueryDTO queryDTO)
    {
        return await _userRepository.GetPageQueryDataAsync(queryDTO);
    }
}
