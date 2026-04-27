using ContactsManager.Application.Common;
using ContactsManager.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace ContactsManager.Tests.Setup;

[TestFixture]
public class SolutionStructureTests
{
    [Test]
    public void ApiResponse_Ok_ShouldReturnSuccessTrue()
    {
        var response = ApiResponse<string>.Ok("test data", "Success");

        response.Success.Should().BeTrue();
        response.Data.Should().Be("test data");
        response.Message.Should().Be("Success");
        response.Errors.Should().BeNull();
    }

    [Test]
    public void ApiResponse_Fail_ShouldReturnSuccessFalse()
    {
        var errors = new[] { "Field is required" };
        var response = ApiResponse<string>.Fail("Validation failed", errors);

        response.Success.Should().BeFalse();
        response.Data.Should().BeNull();
        response.Message.Should().Be("Validation failed");
        response.Errors.Should().Contain("Field is required");
    }

    [Test]
    public void NotFoundException_ShouldContainEntityNameAndKey()
    {
        var ex = new NotFoundException("Contact", 42);

        ex.Message.Should().Contain("Contact");
        ex.Message.Should().Contain("42");
    }

    [Test]
    public void PagedResult_TotalPages_ShouldCalculateCorrectly()
    {
        var result = new PagedResult<string>
        {
            Items = ["a", "b", "c"],
            TotalCount = 25,
            Page = 1,
            PageSize = 10
        };

        result.TotalPages.Should().Be(3);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }
}
