namespace SquirrelUnitTest;

public record Transaction
(
    string CustomerID,
    string FirstName,
    string LastName,
    string TransactionDate,
    string TransactionTime,
    string TransactionType,
    string TransactionAmount,
    string TransactionCurrency
);