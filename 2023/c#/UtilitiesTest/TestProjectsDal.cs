using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace UtilitiesTest
{
    [TestClass]
    public class TestProjectsDal
    {
        [TestMethod]
        public async Task Can_Create_Projects_In_SurrealDB()
        {
            var dal = DALFactory.CreateDAL();

            dal.CreateProject("Test Project");

            var projects = await dal.GetProjects();

            Assert.Equals(1, projects.Count);

            // TODO: Cleanup
            // Need to remove the project we just created

        }
    }
}