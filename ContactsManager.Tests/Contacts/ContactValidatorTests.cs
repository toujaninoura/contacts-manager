using ContactsManager.Application.DTOs.Contacts;
using ContactsManager.Application.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace ContactsManager.Tests.Contacts;

[TestFixture]
public class ContactValidatorTests
{
    private CreateContactDtoValidator _createValidator = null!;
    private UpdateContactDtoValidator _updateValidator = null!;

    [SetUp]
    public void SetUp()
    {
        _createValidator = new CreateContactDtoValidator();
        _updateValidator = new UpdateContactDtoValidator();
    }

    // ---- CreateContactDtoValidator ----

    [Test]
    public void CreateValidator_should_pass_when_all_fields_valid()
    {
        var dto = new CreateContactDto("Dupont", "Jean", "jean@example.com", "0601020304", 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void CreateValidator_should_fail_when_nom_empty()
    {
        var dto = new CreateContactDto("", "Jean", "jean@example.com", null, 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nom" && e.ErrorMessage.Contains("nom"));
    }

    [Test]
    public void CreateValidator_should_fail_when_nom_whitespace()
    {
        var dto = new CreateContactDto("   ", "Jean", "jean@example.com", null, 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nom");
    }

    [Test]
    public void CreateValidator_should_fail_when_prenom_empty()
    {
        var dto = new CreateContactDto("Dupont", "", "jean@example.com", null, 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Prenom" && e.ErrorMessage.Contains("prenom"));
    }

    [Test]
    public void CreateValidator_should_fail_when_email_invalid_format()
    {
        var dto = new CreateContactDto("Dupont", "Jean", "not-an-email", null, 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public void CreateValidator_should_fail_when_email_empty()
    {
        var dto = new CreateContactDto("Dupont", "Jean", "", null, 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public void CreateValidator_should_fail_when_telephone_too_long()
    {
        var dto = new CreateContactDto("Dupont", "Jean", "jean@example.com", new string('0', 21), 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Telephone");
    }

    [Test]
    public void CreateValidator_should_pass_when_telephone_null()
    {
        var dto = new CreateContactDto("Dupont", "Jean", "jean@example.com", null, 1);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void CreateValidator_should_fail_when_userId_zero()
    {
        var dto = new CreateContactDto("Dupont", "Jean", "jean@example.com", null, 0);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    // ---- UpdateContactDtoValidator ----

    [Test]
    public void UpdateValidator_should_pass_when_all_fields_valid()
    {
        var dto = new UpdateContactDto("Dupont", "Jean", "jean@example.com", "0601020304");

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void UpdateValidator_should_fail_when_nom_empty()
    {
        var dto = new UpdateContactDto("", "Jean", "jean@example.com", null);

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nom" && e.ErrorMessage.Contains("nom"));
    }

    [Test]
    public void UpdateValidator_should_fail_when_nom_whitespace()
    {
        var dto = new UpdateContactDto("   ", "Jean", "jean@example.com", null);

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nom");
    }

    [Test]
    public void UpdateValidator_should_fail_when_prenom_empty()
    {
        var dto = new UpdateContactDto("Dupont", "", "jean@example.com", null);

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Prenom" && e.ErrorMessage.Contains("prenom"));
    }

    [Test]
    public void UpdateValidator_should_fail_when_email_invalid_format()
    {
        var dto = new UpdateContactDto("Dupont", "Jean", "invalid-email", null);

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public void UpdateValidator_should_fail_when_email_empty()
    {
        var dto = new UpdateContactDto("Dupont", "Jean", "", null);

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public void UpdateValidator_should_pass_when_telephone_null()
    {
        var dto = new UpdateContactDto("Dupont", "Jean", "jean@example.com", null);

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void UpdateValidator_should_fail_when_telephone_too_long()
    {
        var dto = new UpdateContactDto("Dupont", "Jean", "jean@example.com", new string('0', 21));

        var result = _updateValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Telephone");
    }
}
