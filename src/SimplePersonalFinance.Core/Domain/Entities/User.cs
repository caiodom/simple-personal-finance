using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Results;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class User:Entity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime BirthdayDate { get; set; }

    public string Role { get; private set; }

    private User(string name, Email email,DateTime birthdayDate, string passwordHash,string role)
    {
        Name=name;
        Email=email;
        PasswordHash=passwordHash;
        BirthdayDate = birthdayDate;
        Role = role;
    }

    public static Result<User>Create(
        string name,
        string email,
        string passwordHash,
        string role,
        DateTime birthDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<User>("Name cannot be empty");

        if (birthDate >= DateTime.Today)
            return Result.Failure<User>("Birth date must be in the past");

        var emailResult= Email.Create(email);
        if(emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<User>("Password hash cannot be empty");

        var user = new User(
            name,
            emailResult.Value,
            birthDate,
            passwordHash,
            role);

        return Result.Success(user);
    }

    // Constructor for EF Core
    protected User() { }

    //Ef Rel
    public ICollection<Account> Accounts { get; }
    public ICollection<Budget> Budgets { get;}
}
