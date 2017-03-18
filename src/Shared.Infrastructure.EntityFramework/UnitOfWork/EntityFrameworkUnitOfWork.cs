﻿using Autofac;
using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using System;
using Shared.Infrastructure.UnitOfWork;

namespace Shared.Infrastructure.UnitOfWork.EntityFramework
{
    public class EntityFrameworkUnitOfWork<TContext> : UnitOfWorkBase
        where TContext : DbContext
    {
        public TContext Context { get; private set; }

        private IComponentContext ComponentContext { get; set; }

        public EntityFrameworkUnitOfWork(TContext context, IComponentContext componentContext)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ComponentContext = componentContext;
        }

        public override ITransaction BeginTransaction()
        {
            var transaction = this.Context.Database.BeginTransaction();
            return new EntityFrameworkTransaction(transaction);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.Context != null)
            {
                this.Context.Dispose();
            }
        }

        protected override T ResolveRepository<T>()
        {
            var ovList = new Parameter[]
                {
                    new TypedParameter(typeof(DbContext), this.Context)
                };
            var repository = this.ComponentContext.Resolve<T>(ovList);

            return repository;
        }
    }
}
