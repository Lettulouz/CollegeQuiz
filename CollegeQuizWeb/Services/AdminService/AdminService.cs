using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.AdminService;

public class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ISmtpService _smtpService;

    public AdminService(ILogger<AdminService> logger, ApplicationDbContext context, ISmtpService smtpService,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _logger = logger;
        _context = context;
        _smtpService = smtpService;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserEntity>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task CreateCoupons(CouponDtoPayload obj)
    {
        var controller = obj.ControllerReference;
        int amount = obj.Dto.Amount;
        DateTime expiringAt = obj.Dto.ExpiringAt;
        int extensionTime = obj.Dto.ExtensionTime;
        int typeOfSubscription = obj.Dto.TypeOfSubscription;
        List<CouponEntity> listOfGeneretedCoupons = new();
        for (int i = 0; i < amount; i++)
        {
            bool isExactTheSame = false;
            string generatedToken;
            do
            {
                generatedToken = Utilities.GenerateOtaToken(20);
                var token = await _context.OtaTokens.FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
                if (token != null) isExactTheSame = true;
            } while (isExactTheSame);
            CouponEntity couponEntity = new();
            couponEntity.Token = generatedToken;
            couponEntity.ExpiringAt = expiringAt;
            couponEntity.ExtensionTime = extensionTime;
            couponEntity.TypeOfSubscription = typeOfSubscription;
            listOfGeneretedCoupons.Add(couponEntity);
        }

        await _context.AddRangeAsync(listOfGeneretedCoupons);
        await _context.SaveChangesAsync();
    }

}