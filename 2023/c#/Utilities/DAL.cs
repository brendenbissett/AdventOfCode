using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SurrealDb.Net;
using SurrealDb.Net.Models;
using SurrealDb.Net.Models.Auth;
using System.Text.Json;
using System.Net;
using System.ComponentModel;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Utilities
{
    public class Project : Record
    {
        public string? Name { get; set; }
    }

    public class DAL
    {
        private readonly ISurrealDbClient db;

        public DAL(ISurrealDbClient client)
        {
            this.db = client;
        }

        public async void CreateProject(string name)
        {
            var project = new Project
            {
                Name = name
            };

            var created = await this.db.Create("projects", project);
            Console.WriteLine($"Created project: {created}");
        }

        public async Task<List<Project>> GetProjects()
        {
            var projects = await this.db.Select<Project>("projects");
            return projects.ToList<Project>();
        }
    }

    public static class DALFactory
    {
        public static DAL CreateDAL()
        {
            string db_endpoint = "ws:127.0.0.1:8000/rpc";
            string db_namespace = "AdventOfCode";
            string db_database = "2023";
            string db_user = "root";
            string db_password = "root";

            var db = new SurrealDbClient(db_endpoint);

            db.Configure(db_namespace, db_database, db_user, db_password);

            return new DAL(db);
        }
    }
}
