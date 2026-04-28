using ContactsManager.Domain.Entities;
using ContactsManager.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ContactsManager.Tests.Database;

[TestFixture]
public class DbContextConfigurationTests
{
    private ApplicationDbContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void ApplicationDbContext_should_have_Users_and_Contacts_DbSets()
    {
        _context.Users.Should().NotBeNull();
        _context.Contacts.Should().NotBeNull();
    }

    [Test]
    public void User_Email_should_be_required_and_max_256()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(User))!;

        var emailProp = entityType.FindProperty(nameof(User.Email))!;
        emailProp.IsNullable.Should().BeFalse();
        emailProp.GetMaxLength().Should().Be(256);
    }

    [Test]
    public void User_Email_should_have_unique_index()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(User))!;

        var hasUniqueEmailIndex = entityType.GetIndexes()
            .Any(i => i.IsUnique && i.Properties.Any(p => p.Name == nameof(User.Email)));

        hasUniqueEmailIndex.Should().BeTrue();
    }

    [Test]
    public void User_FirstName_and_LastName_should_be_required_max_100()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(User))!;

        var firstNameProp = entityType.FindProperty(nameof(User.FirstName))!;
        firstNameProp.IsNullable.Should().BeFalse();
        firstNameProp.GetMaxLength().Should().Be(100);

        var lastNameProp = entityType.FindProperty(nameof(User.LastName))!;
        lastNameProp.IsNullable.Should().BeFalse();
        lastNameProp.GetMaxLength().Should().Be(100);
    }

    [Test]
    public void Contact_Email_should_be_required_max_256_and_unique()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Contact))!;

        var emailProp = entityType.FindProperty(nameof(Contact.Email))!;
        emailProp.IsNullable.Should().BeFalse();
        emailProp.GetMaxLength().Should().Be(256);

        var hasUniqueEmailIndex = entityType.GetIndexes()
            .Any(i => i.IsUnique && i.Properties.Any(p => p.Name == nameof(Contact.Email)));
        hasUniqueEmailIndex.Should().BeTrue();
    }

    [Test]
    public void Contact_Nom_and_Prenom_should_be_required_max_100()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Contact))!;

        var nomProp = entityType.FindProperty(nameof(Contact.Nom))!;
        nomProp.IsNullable.Should().BeFalse();
        nomProp.GetMaxLength().Should().Be(100);

        var prenomProp = entityType.FindProperty(nameof(Contact.Prenom))!;
        prenomProp.IsNullable.Should().BeFalse();
        prenomProp.GetMaxLength().Should().Be(100);
    }

    [Test]
    public void Contact_Telephone_should_be_nullable_max_20()
    {
        var model = _context.Model;
        var entityType = model.FindEntityType(typeof(Contact))!;

        var telProp = entityType.FindProperty(nameof(Contact.Telephone))!;
        telProp.GetMaxLength().Should().Be(20);
    }

    [Test]
    public async Task Can_insert_and_retrieve_User_via_InMemory()
    {
        var user = new User("test@example.com", "hashedpassword", "John", "Doe");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        retrieved.Should().NotBeNull();
        retrieved!.FirstName.Should().Be("John");
    }

    [Test]
    public async Task Can_insert_and_retrieve_Contact_via_InMemory()
    {
        var user = new User("owner@example.com", "hash", "Owner", "User");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var contact = new Contact("Doe", "Jane", "jane@example.com", "0600000000", user.Id);
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Contacts.FirstOrDefaultAsync(c => c.Email == "jane@example.com");
        retrieved.Should().NotBeNull();
        retrieved!.Nom.Should().Be("Doe");
    }
}
