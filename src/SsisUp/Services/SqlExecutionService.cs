﻿using System.Collections.Generic;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;

namespace SsisUp.Services
{
    public interface ISqlExecutionService
    {
        DeploymentResult Execute(string connectionString, IEnumerable<SqlScript> sqlScripts, bool debug);
    }

    public class SqlExecutionService : ISqlExecutionService
    {
        public DeploymentResult Execute(string connectionString, IEnumerable<SqlScript> sqlScripts, bool debug)
        {
            UpgradeEngine upgradeEngine;
            
            upgradeEngine = debug 
                ? BuildEngineWithOutputScriptLoggingEnabled(connectionString, sqlScripts) 
                : BuildEngine(connectionString, sqlScripts);

            var result = upgradeEngine.PerformUpgrade();

            return !result.Successful
                ? new DeploymentResult(result.Error, result.Successful, GetType())
                : new DeploymentResult(null, result.Successful, GetType());
        }

        protected UpgradeEngine BuildEngine(string connectionString, IEnumerable<SqlScript> sqlScripts)
        {
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalTo(new NullJournal())
                .WithScripts(sqlScripts)
                .LogToConsole()
                .Build();

            return upgradeEngine;
        }

        protected UpgradeEngine BuildEngineWithOutputScriptLoggingEnabled(string connectionString, IEnumerable<SqlScript> sqlScripts)
        {
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalTo(new NullJournal())
                .WithScripts(sqlScripts)
                .LogScriptOutput()
                .LogToConsole()
                .Build();

            return upgradeEngine;
        }
    }
}