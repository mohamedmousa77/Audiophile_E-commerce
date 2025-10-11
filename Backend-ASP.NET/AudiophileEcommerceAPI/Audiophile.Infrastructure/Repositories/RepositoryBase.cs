

using Audiophile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiophile.Infrastructure.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly AppDbContext Context;
        private IDbContextTransaction? _transaction;

        protected RepositoryBase(AppDbContext context)
        {
            Context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await Context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await Context.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

}
