using Microsoft.EntityFrameworkCore.Storage;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;
using System.Data;

namespace SimplePersonalFinance.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAccountRepository Accounts { get; }
        public IUserRepository Users { get; }
        public IBudgetRepository Budgets { get; }
        public ITransactionReadRepository Transactions { get; }

        private bool _disposed;
        private IDbContextTransaction? _transaction;
        private readonly AppDbContext _context;

        public UnitOfWork(IUserRepository userRepository,
                          IBudgetRepository budgetRepository,
                          IAccountRepository accountRepository,
                          AppDbContext context)
        {
            _context = context;
            Users = userRepository;
            Budgets = budgetRepository;
            Accounts = accountRepository;
        }


        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started. Call BeginTransactionAsync first.");
            }

            try
            {
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }

                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
