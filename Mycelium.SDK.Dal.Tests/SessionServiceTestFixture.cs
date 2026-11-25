// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionServiceTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging.Abstractions;

    using Mycelium.SDK.Dal;

    using SysML2.NET.Dal;
    using SysML2.NET.Serializer.Json;

    using SySML2.NET.REST;

    [TestFixture]
    public class SessionServiceTestFixture
    {
        private const string FirstProjectId = "6f1d2c3a-0b4e-4c1a-9a51-6d2e8f0a1c01";

        private const string SecondProjectId = "6f1d2c3a-0b4e-4c1a-9a51-6d2e8f0a1c02";

        private const string UnknownProjectId = "6f1d2c3a-0b4e-4c1a-9a51-6d2e8f0a1c03";

        private const string FailingProjectId = "6f1d2c3a-0b4e-4c1a-9a51-6d2e8f0a1c04";

        private const string FirstPage = "/api/projects";

        private const string SecondPage = "/api/projects?page[after]=page-2";

        private static readonly Uri ServerAddress = new Uri("http://localhost/api");

        private static readonly Uri RelativeAddress = new Uri("api", UriKind.Relative);

        private static readonly string[] Failures = ["status", "json", "network", "timeout"];

        private static readonly string[] ProjectNames = ["Spacecraft Mission", "GroundStation-v2"];

        private static readonly string[] BothPages = [FirstPage, SecondPage];

        private static readonly string[] ProjectReads = [$"/api/projects/{FirstProjectId}", $"/api/projects/{UnknownProjectId}", $"/api/projects/{FailingProjectId}"];

        private ProjectServerHandler server;

        private Session session;

        private SessionService service;

        [SetUp]
        public void SetUp()
        {
            this.server = new ProjectServerHandler();
            this.server.Answer(FirstPage, () => Page(FirstProjectId, "Spacecraft Mission", "page-2"));
            this.server.Answer(SecondPage, () => Page(SecondProjectId, "GroundStation-v2", null));
            this.session = new Session(new ProjectServerClientFactory(this.server), new DeSerializer(), new Serializer(), new Assembler());
            this.service = new SessionService(this.session, NullLogger<SessionService>.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            this.session.Close();
            this.server.Dispose();
        }

        [Test]
        public async Task VerifyThatOpeningReadsEveryPageOfProjectsAndKeepsTheServerAddress()
        {
            var result = await this.service.OpenAsync(ServerAddress, CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.IsError, Is.False, "The opening must succeed when every page answers.");
                Assert.That(result.Value.Select(project => project.Name), Is.EqualTo(ProjectNames), "The projects of both pages must be returned in order.");
                Assert.That(this.server.Requests, Is.EqualTo(BothPages), "The second page must be read through the cursor of the first.");
                Assert.That(this.service.ServerAddress, Is.EqualTo(ServerAddress), "The address of the open connection must be kept.");
                Assert.That(this.session.Client, Is.Not.Null, "The connection must stay open.");
            }
        }

        [Test]
        public async Task VerifyThatClosingDropsTheConnection()
        {
            await this.service.OpenAsync(ServerAddress, CancellationToken.None);

            this.service.Close();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.service.ServerAddress, Is.Null, "No address must be kept once the connection is closed.");
                Assert.That(this.session.Client, Is.Null, "The SysML2.NET session must be closed.");
            }
        }

        [Test]
        public async Task VerifyThatAServerThatFailsOnAnyPageLeavesNoConnection([ValueSource(nameof(Failures))] string failure, [Values(FirstPage, SecondPage)] string failingPage)
        {
            await this.service.OpenAsync(ServerAddress, CancellationToken.None);
            this.server.Answer(failingPage, () => Fail(failure));

            var result = await this.service.OpenAsync(ServerAddress, CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.IsError, Is.True, $"A '{failure}' failure on '{failingPage}' must be reported.");
                Assert.That(result.FirstError, Is.EqualTo(SessionErrors.Unavailable), $"A '{failure}' failure on '{failingPage}' must read as unavailable.");
                Assert.That(this.service.ServerAddress, Is.Null, $"A '{failure}' failure on '{failingPage}' must leave no address.");
                Assert.That(this.session.Client, Is.Null, $"A '{failure}' failure on '{failingPage}' must leave no open connection.");
            }
        }

        [TestCase(true, 2)]
        [TestCase(false, 1000)]
        public async Task VerifyThatAServerWhosePagesRepeatOrNeverEndIsRefused(bool repeats, int requestCount)
        {
            var pageNumber = 2;
            this.server.Answer(SecondPage, () => Page(SecondProjectId, "GroundStation-v2", repeats ? "page-2" : $"page-{++pageNumber}"));
            this.server.Fallback = _ => Page(SecondProjectId, "GroundStation-v2", $"page-{++pageNumber}");

            var result = await this.service.OpenAsync(ServerAddress, CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.FirstError, Is.EqualTo(SessionErrors.Unavailable), "Pages that repeat or never end must be refused.");
                Assert.That(this.server.Requests, Has.Count.EqualTo(requestCount), "The reading must stop at the repeated cursor or at the page limit.");
                Assert.That(this.service.ServerAddress, Is.Null, "A refused server must leave no address.");
            }
        }

        [Test]
        public async Task VerifyThatACancelledOrRefusedOpeningLeavesNoConnection()
        {
            using var cancellation = new CancellationTokenSource();
            this.server.Answer(SecondPage, () =>
            {
                cancellation.Cancel();
                throw new OperationCanceledException(cancellation.Token);
            });

            Assert.That(async () => await this.service.OpenAsync(ServerAddress, cancellation.Token), Throws.InstanceOf<OperationCanceledException>(), "A cancellation by the caller must not become an error.");
            var clientAfterCancellation = this.session.Client;
            var addressAfterCancellation = this.service.ServerAddress;

            this.server.Answer(SecondPage, () => Page(SecondProjectId, "GroundStation-v2", null));
            await this.service.OpenAsync(ServerAddress, CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(clientAfterCancellation, Is.Null, "An opening cancelled after the first page must close the SysML2.NET session.");
                Assert.That(addressAfterCancellation, Is.Null, "A cancelled opening must leave no address.");
                Assert.That(async () => await this.service.OpenAsync(RelativeAddress, CancellationToken.None), Throws.ArgumentException, "A relative address must be refused.");
                Assert.That(this.session.Client, Is.Null, "A refused address must close the connection it replaces.");
                Assert.That(this.service.ServerAddress, Is.Null, "A refused address must leave no address.");
            }
        }

        [Test]
        public void VerifyThatMissingArgumentsAreRefused()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => new SessionService(null, NullLogger<SessionService>.Instance), Throws.ArgumentNullException, "A session is required.");
                Assert.That(() => new SessionService(this.session, null), Throws.ArgumentNullException, "A logger is required.");
                Assert.That(async () => await this.service.OpenAsync(null, CancellationToken.None), Throws.ArgumentNullException, "An address is required.");
            }
        }

        [Test]
        public async Task VerifyThatProjectsAreReadAgainFromEveryPage()
        {
            var beforeOpening = await this.service.GetProjectsAsync(CancellationToken.None);
            await this.service.OpenAsync(ServerAddress, CancellationToken.None);
            this.server.Requests.Clear();

            var reread = await this.service.GetProjectsAsync(CancellationToken.None);
            var rereadRequests = this.server.Requests.ToArray();
            this.server.Answer(SecondPage, () => Fail("status"));
            var failed = await this.service.GetProjectsAsync(CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(beforeOpening.FirstError, Is.EqualTo(SessionErrors.Unavailable), "No projects can be read before the opening.");
                Assert.That(reread.Value.Select(project => project.Name), Is.EqualTo(ProjectNames), "The projects of both pages must be returned in order.");
                Assert.That(rereadRequests, Is.EqualTo(BothPages), "Every page must be read from the server again.");
                Assert.That(failed.FirstError, Is.EqualTo(SessionErrors.Unavailable), "A failed page must read as unavailable.");
                Assert.That(this.service.ServerAddress, Is.EqualTo(ServerAddress), "A failed read must keep the connection open.");
            }
        }

        [Test]
        public async Task VerifyThatAProjectIsReadFromTheServer()
        {
            var beforeOpening = await this.service.GetProjectAsync(Guid.Parse(FirstProjectId), CancellationToken.None);
            await this.service.OpenAsync(ServerAddress, CancellationToken.None);
            this.server.Requests.Clear();
            this.server.Answer($"/api/projects/{FirstProjectId}", () => Json(ProjectJson(FirstProjectId, "Spacecraft Mission")));
            this.server.Answer($"/api/projects/{FailingProjectId}", () => Fail("status"));

            var known = await this.service.GetProjectAsync(Guid.Parse(FirstProjectId), CancellationToken.None);
            var unknown = await this.service.GetProjectAsync(Guid.Parse(UnknownProjectId), CancellationToken.None);
            var failing = await this.service.GetProjectAsync(Guid.Parse(FailingProjectId), CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(beforeOpening.FirstError, Is.EqualTo(SessionErrors.Unavailable), "No project can be read before the opening.");
                Assert.That(known.Value.Name, Is.EqualTo("Spacecraft Mission"), "A project the server knows must be returned.");
                Assert.That(unknown.FirstError, Is.EqualTo(SessionErrors.ProjectNotFound), "A project the server does not know must read as not found.");
                Assert.That(failing.FirstError, Is.EqualTo(SessionErrors.Unavailable), "A server error must read as unavailable.");
                Assert.That(this.server.Requests, Is.EqualTo(ProjectReads), "Every project must be read from the server by its identifier.");
            }
        }

        private static string ProjectJson(string projectId, string name) => $"{{\"@id\":\"{projectId}\",\"@type\":\"Project\",\"alias\":[],\"created\":\"2026-11-14T13:11:08.148Z\",\"defaultBranch\":{{\"@id\":\"{projectId.Substring(0, 35)}b\"}},\"description\":\"A model\",\"name\":\"{name}\"}}";

        private static HttpResponseMessage Json(string json) => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

        private static HttpResponseMessage Page(string projectId, string name, string next)
        {
            var response = Json($"[{ProjectJson(projectId, name)}]");

            if (next != null)
            {
                response.Headers.Add("Link", $"<{ServerAddress}/projects?page%5Bafter%5D={next}>; rel=\"next\"");
            }

            return response;
        }

        private static HttpResponseMessage Fail(string failure) => failure switch
        {
            "status" => new HttpResponseMessage(HttpStatusCode.InternalServerError),
            "json" => Json("{ not json"),
            "network" => throw new HttpRequestException("No such host"),
            "timeout" => throw new TaskCanceledException("Timed out"),
            _ => throw new ArgumentOutOfRangeException(nameof(failure), failure, "Unknown failure kind")
        };
    }
}
