using System;
using System.Data;
using System.Data.Entity;

namespace TransactionalDbContext
{
    public class TransactionalDbContext : DbContext
    {
        private DbContextTransaction _currentTransaction;

        public void BeginTransaction()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    return;
                }

                _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
            }
            catch (Exception)
            {
                // todo: log transaction exception
                throw;
            }
        }
        public void CloseTransaction()
        {
            CloseTransaction(exception: null);
        }
        public void CloseTransaction(Exception exception)
        {
            try
            {
                if (_currentTransaction != null && exception != null)
                {
                    // todo: log exception
                    _currentTransaction.Rollback();
                    return;
                }

                SaveChanges();

                _currentTransaction?.Commit();
            }
            catch (Exception)
            {
                // todo: log exception
                if (_currentTransaction?.UnderlyingTransaction.Connection != null)
                {
                    _currentTransaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
