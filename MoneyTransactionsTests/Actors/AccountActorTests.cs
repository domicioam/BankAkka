﻿using Akka.Actor;
using Akka.TestKit.Xunit2;
using MoneyTransactions;
using MoneyTransactions.Actors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MoneyTransactionsTests.Actors
{
    public class AccountActorTests : TestKit
    {
        [Fact]
        public void Should_create_actor()
        {
            var accountId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            decimal balance = 100m;
            var user = new User(userId, "Jonh", "Doe");
            var account = new Account(accountId, balance, user);

            var subject = Sys.ActorOf(Props.Create(() => new AccountActor(account)));
        }
    }
}
