using AutoMapper;
using ContactsManager.Application.Common;
using ContactsManager.Application.DTOs.Contacts;
using ContactsManager.Application.Services;
using ContactsManager.Domain.Entities;
using ContactsManager.Domain.Exceptions;
using ContactsManager.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ContactsManager.Tests.Contacts;

[TestFixture]
public class ContactServiceTests
{
    private Mock<IContactRepository> _contactRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<ILogger<ContactService>> _loggerMock = null!;
    private ContactService _contactService = null!;

    [SetUp]
    public void SetUp()
    {
        _contactRepositoryMock = new Mock<IContactRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ContactService>>();

        _contactService = new ContactService(
            _contactRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    // ---- GetAllAsync ----

    [Test]
    public async Task GetAllAsync_should_return_paged_contacts_when_valid()
    {
        var contacts = new List<Contact>
        {
            new Contact("Dupont", "Jean", "jean@example.com", "0601020304", 1),
            new Contact("Martin", "Marie", "marie@example.com", null, 1)
        };
        var dtos = contacts.Select(c => new ContactDto(c.Id, c.Nom, c.Prenom, c.Email, c.Telephone, c.UserId, c.CreatedAt, c.UpdatedAt)).ToList();

        _contactRepositoryMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync((contacts, 2));
        _mapperMock.Setup(m => m.Map<ContactDto>(It.IsAny<Contact>())).Returns<Contact>(c =>
            new ContactDto(c.Id, c.Nom, c.Prenom, c.Email, c.Telephone, c.UserId, c.CreatedAt, c.UpdatedAt));

        var result = await _contactService.GetAllAsync(1, 10);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
    }

    // ---- GetByIdAsync ----

    [Test]
    public async Task GetByIdAsync_should_return_contact_when_found()
    {
        var contact = new Contact("Dupont", "Jean", "jean@example.com", "0601020304", 1);
        var dto = new ContactDto(contact.Id, contact.Nom, contact.Prenom, contact.Email, contact.Telephone, contact.UserId, contact.CreatedAt, contact.UpdatedAt);

        _contactRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contact);
        _mapperMock.Setup(m => m.Map<ContactDto>(contact)).Returns(dto);

        var result = await _contactService.GetByIdAsync(1);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("jean@example.com");
    }

    [Test]
    public async Task GetByIdAsync_should_throw_notfound_when_invalid()
    {
        _contactRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Contact?)null);

        var act = async () => await _contactService.GetByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ---- CreateAsync ----

    [Test]
    public async Task CreateAsync_should_return_created_contact_when_valid()
    {
        var createDto = new CreateContactDto("Dupont", "Jean", "jean@example.com", "0601020304", 1);
        var contact = new Contact("Dupont", "Jean", "jean@example.com", "0601020304", 1);
        var dto = new ContactDto(contact.Id, contact.Nom, contact.Prenom, contact.Email, contact.Telephone, contact.UserId, contact.CreatedAt, contact.UpdatedAt);

        _mapperMock.Setup(m => m.Map<Contact>(createDto)).Returns(contact);
        _contactRepositoryMock.Setup(r => r.CreateAsync(contact)).ReturnsAsync(contact);
        _mapperMock.Setup(m => m.Map<ContactDto>(contact)).Returns(dto);

        var result = await _contactService.CreateAsync(createDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("jean@example.com");
    }

    // ---- UpdateAsync ----

    [Test]
    public async Task UpdateAsync_should_return_updated_contact_when_valid()
    {
        var updateDto = new UpdateContactDto("Dupont", "Jean-Pierre", "jean@example.com", "0601020304");
        var existing = new Contact("Dupont", "Jean", "jean@example.com", "0601020304", 1);
        var updated = new Contact("Dupont", "Jean-Pierre", "jean@example.com", "0601020304", 1);
        var dto = new ContactDto(updated.Id, updated.Nom, updated.Prenom, updated.Email, updated.Telephone, updated.UserId, updated.CreatedAt, updated.UpdatedAt);

        _contactRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _contactRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Contact>())).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<ContactDto>(updated)).Returns(dto);

        var result = await _contactService.UpdateAsync(1, updateDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Prenom.Should().Be("Jean-Pierre");
    }

    [Test]
    public async Task UpdateAsync_should_throw_notfound_when_invalid()
    {
        _contactRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Contact?)null);

        var act = async () => await _contactService.UpdateAsync(99, new UpdateContactDto("A", "B", "a@b.com", null));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ---- DeleteAsync ----

    [Test]
    public async Task DeleteAsync_should_succeed_when_contact_exists()
    {
        _contactRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _contactRepositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var act = async () => await _contactService.DeleteAsync(1);

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task DeleteAsync_should_throw_notfound_when_invalid()
    {
        _contactRepositoryMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var act = async () => await _contactService.DeleteAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
