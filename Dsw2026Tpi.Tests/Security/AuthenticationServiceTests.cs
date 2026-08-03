using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services.Security;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Identity;
using Dsw2026Tpi.Data.Identity;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dsw2026Tpi.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly UserManager<ApplicationUser> _mockUserManager;
    private readonly ISignInService _mockSignInManager;
    private readonly RoleManager<IdentityRole> _mockRoleManager;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthenticationService> _mockLogger;
    private readonly IPersistence _mockPersistence;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _mockUserManager = Substitute.For<UserManager<ApplicationUser>>(store, null!, null!, null!, null!, null!, null!, null!, null!);

        _mockSignInManager = Substitute.For<ISignInService>();

        var roleStore = Substitute.For<IRoleStore<IdentityRole>>();
        _mockRoleManager = Substitute.For<RoleManager<IdentityRole>>(roleStore, null!, null!, null!, null!);

        var mockConfiguration = Substitute.For<IConfiguration>();
        _jwtService = new JwtService(mockConfiguration);

        _mockLogger = Substitute.For<ILogger<AuthenticationService>>();
        _mockPersistence = Substitute.For<IPersistence>();

        _service = new AuthenticationService(
            _mockUserManager,
            _mockSignInManager,
            _mockRoleManager,
            _jwtService,
            _mockLogger,
            _mockPersistence
        );
    }

    [Fact]
    public async Task LoginAdmin_CuandoEmailEsInvalido_EntoncesLanzaAuthenticationException()
    {
        // Arrange
        var request = new LoginAdminModel.Request("emailinvalido", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _service.LoginAdmin(request));
    }

    [Fact]
    public async Task LoginAdmin_CuandoUsuarioNoExiste_EntoncesLanzaAuthenticationException()
    {
        // Arrange
        var request = new LoginAdminModel.Request("admin@test.com", "Password123!");
        _mockUserManager.FindByEmailAsync(request.Email).Returns((ApplicationUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _service.LoginAdmin(request));
    }

    [Fact]
    public async Task LoginPatient_CuandoEmailODniInvalido_EntoncesLanzaValidationException()
    {
        // Arrange
        var request = new LoginPatientModel.Request("invalido", "123");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.LoginPatient(request));
    }

    [Fact]
    public async Task Register_CuandoEmailEsInvalido_EntoncesLanzaValidationException()
    {
        // Arrange
        var request = new RegisterModel.Request("email-sin-formato", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.Register(request));
    }
}