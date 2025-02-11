﻿using Akka.Actor;
using Akka.TestKit.Xunit2;
using MoneyTransactions;
using MoneyTransactions.Actors;
using MoneyTransactions.Foundation;
using System;
using Xunit;
using static MoneyTransactions.Actors.AccountActor;

namespace MoneyTransactionsTests.Actors
{
    public class AccountActorTests : TestKit
    {
        [Fact]
        public void Deposit_should_succeed_when_requested_with_correct_values()
        {
            var accountId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            decimal balance = 100m;
            var client = new Client(clientId, "Jonh", "Doe");
            var account = new Account(accountId, balance, client);

            var subject = Sys.ActorOf(AccountActor.Props(account));
            
            decimal amount = 50m;
            subject.Tell(new Deposit(amount));
            ExpectMsg((Result<Deposit> msg) => Assert.Equal(MoneyTransactions.Status.Success, msg.Status));
            
            subject.Tell(new CheckBalance());
            ExpectMsg<BalanceStatus>(msg => Assert.Equal(balance + amount, msg.Balance ));
        }

        [Fact]
        public void Withdraw_should_succeed_when_requested_with_correct_values()
        {
            var accountId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            decimal balance = 100m;
            var client = new Client(clientId, "Jonh", "Doe");
            var account = new Account(accountId, balance, client);

            var subject = Sys.ActorOf(AccountActor.Props(account));

            decimal amount = 50m;
            subject.Tell(new Withdraw(amount));
            ExpectMsg((Result<Withdraw> msg) => Assert.True(msg.Status == MoneyTransactions.Status.Success));

            subject.Tell(new CheckBalance());
            ExpectMsg<BalanceStatus>(msg => Assert.Equal(balance - amount, msg.Balance));
        }

        [Fact]
        public void Withdraw_followed_by_deposit_should_work()
        {
            var accountId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            decimal balance = 100m;
            var client = new Client(clientId, "Jonh", "Doe");
            var account = new Account(accountId, balance, client);

            var subject = Sys.ActorOf(AccountActor.Props(account));

            decimal withdraw = 50m;
            subject.Tell(new Withdraw(withdraw));
            ExpectMsg((Result<Withdraw> msg) => Assert.True(msg.Status == MoneyTransactions.Status.Success));

            subject.Tell(new CheckBalance());
            ExpectMsg<BalanceStatus>(msg =>
            {
                decimal expected = balance - withdraw;
                Assert.Equal(expected, msg.Balance);
            });

            decimal deposit = 50m;
            subject.Tell(new Deposit(deposit));
            ExpectMsg((Result<Deposit> msg) => Assert.Equal(MoneyTransactions.Status.Success, msg.Status));

            subject.Tell(new CheckBalance());
            ExpectMsg<BalanceStatus>(msg =>
            {
                decimal expected = balance + deposit - withdraw;
                Assert.Equal(expected, msg.Balance);
            });
        }

        [Fact]
        public void Withdraw_should_fail_when_balance_is_not_enough()
        {
            var accountId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            decimal balance = 40m;
            var client = new Client(clientId, "Jonh", "Doe");
            var account = new Account(accountId, balance, client);

            var subject = Sys.ActorOf(AccountActor.Props(account));

            decimal amount = 50m;
            subject.Tell(new Withdraw(amount));
            ExpectMsg((Result<Withdraw> msg) => Assert.True(msg.Status == MoneyTransactions.Status.Error));

            subject.Tell(new CheckBalance());
            ExpectMsg<BalanceStatus>(msg => Assert.Equal(balance, msg.Balance));
        }
    }
}
