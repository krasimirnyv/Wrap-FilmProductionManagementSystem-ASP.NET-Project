namespace Wrap.Infrastructure.Tests;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using NUnit.Framework;

using GCommon.ValidationAttributes;

using static GCommon.OutputMessages;

[TestFixture]
public class IsAfterAttributeTests
{
    private sealed class ModelWithDates
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public object? StartObj { get; set; }
    }

    private static ValidationContext CreateContext(object instance) => new(instance);

    private static ValidationResult? Validate(IsAfter attribute, object? value, object instance)
        => attribute.GetValidationResult(value, CreateContext(instance));
    
    private static IEnumerable<TestCaseData> DateComparisonCases()
    {
        yield return new TestCaseData(
                new DateTime(2026, 1, 2), // end
                new DateTime(2026, 1, 1), // start
                true)                                     // isSucceeded
            .SetName("IsValid_WhenEndIsAfterStart_ReturnsSuccess");

        yield return new TestCaseData(
                new DateTime(2026, 1, 1), // end
                new DateTime(2026, 1, 1), // start
                false)                                    // isSucceeded
            .SetName("IsValid_WhenEndEqualsStart_ReturnsError");

        yield return new TestCaseData(
                new DateTime(2025, 12, 31), // end
                new DateTime(2026, 1, 1),   // start
                false)                                      // isSucceeded
            .SetName("IsValid_WhenEndIsBeforeStart_ReturnsError");
    }

    [TestCaseSource(nameof(DateComparisonCases))]
    public void IsValid_WhenEndComparedToStart_BehavesCorrectly(DateTime end, DateTime start, bool isSuccess)
    {
        // Arrange
        ModelWithDates model = new ModelWithDates
        {
            Start = start,
            End = end
        };

        IsAfter attribute = new IsAfter(nameof(ModelWithDates.Start));

        // Act
        ValidationResult? result = Validate(attribute, model.End, model);

        // Assert
        if (isSuccess)
        {
            Assert.That(result, Is.EqualTo(ValidationResult.Success));
        }
        else
        {
            Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
            Assert.That(result!.ErrorMessage, Is.EqualTo(string.Format(IsAfterExceptionMessage, start)));
        }
    }
    
    [Test]
    public void IsValid_WhenInvalidAndCustomErrorMessageProvided_ReturnsCustomMessage()
    {
        // Arrange
        ModelWithDates model = new ModelWithDates
        {
            Start = new DateTime(2026, 1, 1),
            End = new DateTime(2026, 1, 1)
        };

        IsAfter attribute = new IsAfter(nameof(ModelWithDates.Start))
        {
            ErrorMessage = "Custom message"
        };

        // Act
        ValidationResult? result = Validate(attribute, model.End, model);

        // Assert
        Assert.That(result, Is.Not.EqualTo(ValidationResult.Success));
        Assert.That(result!.ErrorMessage, Is.EqualTo("Custom message"));
    }
    
    private static IEnumerable<TestCaseData> NonDateCases()
    {
        yield return new TestCaseData(null).SetName("IsValid_WhenValueIsNull_ReturnsSuccess");
        yield return new TestCaseData("2026-01-01").SetName("IsValid_WhenValueIsNotDateTime_ReturnsSuccess");
        yield return new TestCaseData(123).SetName("IsValid_WhenValueIsInt_ReturnsSuccess");
    }

    [TestCaseSource(nameof(NonDateCases))]
    public void IsValid_WhenValueIsNullOrNotDateTime_ReturnsSuccess(object? value)
    {
        // Arrange
        ModelWithDates model = new ModelWithDates { Start = new DateTime(2026, 1, 1) };
        IsAfter attribute = new IsAfter(nameof(ModelWithDates.Start));

        // Act
        ValidationResult? result = Validate(attribute, value, model);

        // Assert
        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }
    
    [Test]
    public void IsValid_WhenComparisonPropertyDoesNotExist_ReturnsSuccess()
    {
        // Arrange
        ModelWithDates model = new ModelWithDates
        {
            Start = new DateTime(2026, 1, 1),
            End = new DateTime(2026, 1, 2)
        };

        IsAfter attribute = new IsAfter("MissingProperty");

        // Act
        ValidationResult? result = Validate(attribute, model.End, model);

        // Assert
        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }
    
    [Test]
    public void IsValid_WhenComparisonPropertyIsNotDateTime_ReturnsSuccess()
    {
        // Arrange
        ModelWithDates model = new ModelWithDates { StartObj = "not a date" };
        IsAfter attribute = new IsAfter(nameof(ModelWithDates.StartObj));

        // Act
        ValidationResult? result = Validate(attribute, new DateTime(2026, 1, 2), model);

        // Assert
        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }
    
    [Test]
    public void IsValid_WhenComparisonPropertyIsNull_ReturnsSuccess()
    {
        // Arrange
        ModelWithDates model = new ModelWithDates { StartObj = null };
        IsAfter attribute = new IsAfter(nameof(ModelWithDates.StartObj));

        // Act
        ValidationResult? result = Validate(attribute, new DateTime(2026, 1, 2), model);

        // Assert
        Assert.That(result, Is.EqualTo(ValidationResult.Success));
    }
}