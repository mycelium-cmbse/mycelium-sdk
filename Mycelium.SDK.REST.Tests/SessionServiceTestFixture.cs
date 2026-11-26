// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionServiceTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.REST.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using ErrorOr;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;

    using RichardSzalay.MockHttp;

    using SysML2.NET.Dal;
    using SysML2.NET.PSM;
    using SysML2.NET.Serializer.Json;

    using SySML2.NET.REST;

    using Branch = SysML2.NET.PSM.DTO.Branch;

    [TestFixture]
    public class SessionServiceTestFixture
    {
        private const string JsonMediaType = "application/json";

        private const string ProjectsUri = "http://localhost/api/projects";

        private const string FirstProjectId = "6f1d2c3a-0b4e-4c1a-9a51-6d2e8f0a1c01";

        private const string SecondProjectId = "6f1d2c3a-0b4e-4c1a-9a51-6d2e8f0a1c02";

        private static readonly Uri ServerAddress = new Uri("http://localhost/api");

        private static readonly string[] Failures = ["status", "json", "network", "timeout"];

        private static readonly string[] ProjectNames = ["Spacecraft Mission", "GroundStation-v2"];

        private MockHttpMessageHandler server;

        private Mock<IHttpClientFactory> httpClientFactory;

        private Mock<ILogger<SessionService>> logger;

        private SessionServiceOptions options;

        private Session session;

        private SessionService service;

        [SetUp]
        public void SetUp()
        {
            this.server = new MockHttpMessageHandler();
            this.httpClientFactory = new Mock<IHttpClientFactory>();
            this.httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(() => this.server.ToHttpClient());
            this.logger = new Mock<ILogger<SessionService>>();
            this.options = new SessionServiceOptions();
            this.session = new Session(this.httpClientFactory.Object, new DeSerializer(), new Serializer(), new Assembler());
            this.service = new SessionService(this.session, Options.Create(this.options), this.logger.Object);
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
            this.ExpectProjectPages();

            var opened = await this.service.OpenAsync(ServerAddress);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(opened.IsError, Is.False);
                Assert.That(opened.Value.Select(project => project.Name), Is.EqualTo(ProjectNames));
                Assert.That(this.service.ServerAddress, Is.EqualTo(ServerAddress));
                this.server.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task VerifyThatClosingDropsTheConnection()
        {
            this.ExpectProjectPages();
            await this.service.OpenAsync(ServerAddress);

            this.service.Close();
            var projects = await this.service.GetProjectsAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.service.ServerAddress, Is.Null);
                Assert.That(projects.FirstError.Code, Is.EqualTo(SessionErrors.NotOpenCode));
            }
        }

        [Test]
        public async Task VerifyThatAServerThatFailsOnAnyPageReturnsAnErrorWithItsExceptionAndLeavesNoConnection([ValueSource(nameof(Failures))] string failure, [Values(1, 2)] int failingPage)
        {
            if (failingPage == 1)
            {
                Fail(this.server.Expect(HttpMethod.Get, ProjectsUri).WithExactQueryString(string.Empty), failure);
            }
            else
            {
                this.server.Expect(HttpMethod.Get, ProjectsUri).WithExactQueryString(string.Empty).Respond(HttpStatusCode.OK, NextLink("page-2"), JsonMediaType, Projects(FirstProjectId, ProjectNames[0]));
                Fail(this.server.Expect(HttpMethod.Get, ProjectsUri).WithQueryString("page[after]", "page-2"), failure);
            }

            var opened = await this.service.OpenAsync(ServerAddress);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(opened.FirstError.Code, Is.EqualTo(SessionErrors.UnavailableCode));
                Assert.That(opened.FirstError.Metadata?[SessionErrors.ExceptionKey], Is.InstanceOf<Exception>(), "The error must carry the exception that caused it.");
                Assert.That(this.service.ServerAddress, Is.Null);
                Assert.That(this.session.Client, Is.Null);
            }
        }

        [TestCase(true, 2)]
        [TestCase(false, 3)]
        public async Task VerifyThatPagesThatRepeatOrPassTheConfiguredLimitAreRefused(bool repeats, int expectedRequests)
        {
            this.options.MaximumPageCount = 3;
            var requests = 0;
            this.server.When(HttpMethod.Get, ProjectsUri).Respond(_ =>
            {
                requests++;
                var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Projects(FirstProjectId, ProjectNames[0]), System.Text.Encoding.UTF8, JsonMediaType) };
                response.Headers.Add("Link", $"<{ProjectsUri}?page%5Bafter%5D={(repeats ? "same" : $"page-{requests}")}>; rel=\"next\"");
                return response;
            });

            var opened = await this.service.OpenAsync(ServerAddress);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(opened.FirstError.Code, Is.EqualTo(SessionErrors.UnavailableCode));
                Assert.That(requests, Is.EqualTo(expectedRequests));
            }
        }

        [Test]
        public async Task VerifyThatACancelledOpeningReturnsAnErrorAndLeavesNoConnection()
        {
            using var cancellation = new CancellationTokenSource();
            this.server.When(HttpMethod.Get, ProjectsUri).Respond(_ =>
            {
                cancellation.Cancel();
                throw new OperationCanceledException(cancellation.Token);
            });

            var opened = await this.service.OpenAsync(ServerAddress, cancellation.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(opened.FirstError.Code, Is.EqualTo(SessionErrors.CancelledCode));
                Assert.That(opened.FirstError.Metadata[SessionErrors.ExceptionKey], Is.InstanceOf<OperationCanceledException>());
                Assert.That(this.service.ServerAddress, Is.Null);
            }
        }

        [Test]
        public void VerifyThatMisusedArgumentsAreRefused()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => new SessionService(null, Options.Create(this.options), this.logger.Object), Throws.ArgumentNullException);
                Assert.That(() => new SessionService(this.session, null, this.logger.Object), Throws.ArgumentNullException);
                Assert.That(() => new SessionService(this.session, Options.Create(this.options), null), Throws.ArgumentNullException);
                Assert.That(() => new SessionService(this.session, Options.Create(new SessionServiceOptions { MaximumPageCount = 0 }), this.logger.Object), Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => new SessionService(this.session, Options.Create(new SessionServiceOptions { RequestTimeout = TimeSpan.Zero }), this.logger.Object), Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => this.service.OpenAsync(null), Throws.ArgumentNullException);
                Assert.That(() => this.service.OpenAsync(new Uri("api", UriKind.Relative)), Throws.ArgumentException);
                Assert.That(() => this.service.ExecuteAsync(null), Throws.ArgumentNullException);
                Assert.That(() => this.service.ReadAllAsync<Branch>(null), Throws.ArgumentNullException);
            }
        }

        [Test]
        public async Task VerifyThatProjectsAreReadWithTheQueryParametersOfTheCallerWithoutChangingThem()
        {
            this.ExpectProjectPages();
            await this.service.OpenAsync(ServerAddress);
            var parameters = new QueryParameters { PageSize = 5 };
            this.server.Expect(HttpMethod.Get, ProjectsUri).WithExactQueryString("page[size]=5").Respond(HttpStatusCode.OK, NextLink("page-2"), JsonMediaType, Projects(FirstProjectId, ProjectNames[0]));
            this.server.Expect(HttpMethod.Get, ProjectsUri).WithQueryString("page[size]", "5").WithQueryString("page[after]", "page-2").Respond(JsonMediaType, Projects(SecondProjectId, ProjectNames[1]));

            var projects = await this.service.GetProjectsAsync(parameters);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(projects.Value.Select(project => project.Name), Is.EqualTo(ProjectNames));
                Assert.That(parameters.PageAfter, Is.Null, "The query parameters of the caller must not be changed.");
                this.server.VerifyNoOutstandingExpectation();
            }
        }

        [TestCase(HttpStatusCode.OK, null)]
        [TestCase(HttpStatusCode.NotFound, SessionErrors.ProjectNotFoundCode)]
        [TestCase(HttpStatusCode.InternalServerError, SessionErrors.UnavailableCode)]
        public async Task VerifyThatAProjectIsReadFromTheServer(HttpStatusCode status, string expectedCode)
        {
            this.ExpectProjectPages();
            await this.service.OpenAsync(ServerAddress);
            var request = this.server.Expect(HttpMethod.Get, $"{ProjectsUri}/{FirstProjectId}");

            if (status == HttpStatusCode.OK)
            {
                request.Respond(JsonMediaType, Project(FirstProjectId, ProjectNames[0]));
            }
            else
            {
                request.Respond(status);
            }

            var project = await this.service.GetProjectAsync(Guid.Parse(FirstProjectId));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(project.IsError ? project.FirstError.Code : null, Is.EqualTo(expectedCode));
                Assert.That(project.IsError ? null : project.Value.Name, Is.EqualTo(status == HttpStatusCode.OK ? ProjectNames[0] : null));
                this.server.VerifyNoOutstandingExpectation();
            }
        }

        [Test]
        public async Task VerifyThatAnOperationRunsOnTheOpenSessionAndItsFailureIsReturned()
        {
            var beforeOpening = await this.service.ExecuteAsync((_, _) => Task.CompletedTask);
            this.ExpectProjectPages();
            await this.service.OpenAsync(ServerAddress);
            ISession received = null;

            var succeeded = await this.service.ExecuteAsync((session, _) =>
            {
                received = session;
                return Task.CompletedTask;
            });

            var failed = await this.service.ExecuteAsync((_, _) => throw new InvalidOperationException("Broken operation"));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(beforeOpening.FirstError.Code, Is.EqualTo(SessionErrors.NotOpenCode));
                Assert.That(succeeded.Value, Is.EqualTo(Result.Success));
                Assert.That(received, Is.SameAs(this.session));
                Assert.That(failed.FirstError.Code, Is.EqualTo(SessionErrors.FailedCode));
                Assert.That(failed.FirstError.Metadata[SessionErrors.ExceptionKey], Is.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public async Task VerifyThatEveryPageOfAnotherListIsRead()
        {
            var projectId = Guid.Parse(FirstProjectId);
            var branchesUri = $"{ProjectsUri}/{FirstProjectId}/branches";
            var beforeOpening = await this.service.ReadAllAsync((client, parameters, token) => client.GetBranchesByProjectAsync(projectId, parameters, token));
            this.ExpectProjectPages();
            await this.service.OpenAsync(ServerAddress);
            this.server.Expect(HttpMethod.Get, branchesUri).WithExactQueryString(string.Empty).Respond(HttpStatusCode.OK, [new KeyValuePair<string, string>("Link", $"<{branchesUri}?page%5Bafter%5D=page-2>; rel=\"next\"")], JsonMediaType, "[]");
            this.server.Expect(HttpMethod.Get, branchesUri).WithQueryString("page[after]", "page-2").Respond(JsonMediaType, "[]");

            var branches = await this.service.ReadAllAsync((client, parameters, token) => client.GetBranchesByProjectAsync(projectId, parameters, token));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(beforeOpening.FirstError.Code, Is.EqualTo(SessionErrors.NotOpenCode));
                Assert.That(branches.IsError, Is.False);
                this.server.VerifyNoOutstandingExpectation();
            }
        }

        private static string Project(string projectId, string name) => $"{{\"@id\":\"{projectId}\",\"@type\":\"Project\",\"alias\":[],\"created\":\"2026-11-14T13:11:08.148Z\",\"defaultBranch\":{{\"@id\":\"{projectId.Substring(0, 35)}b\"}},\"description\":\"A model\",\"name\":\"{name}\"}}";

        private static string Projects(string projectId, string name) => $"[{Project(projectId, name)}]";

        private static KeyValuePair<string, string>[] NextLink(string cursor) => [new KeyValuePair<string, string>("Link", $"<{ProjectsUri}?page%5Bafter%5D={cursor}>; rel=\"next\"")];

        private void ExpectProjectPages()
        {
            this.server.Expect(HttpMethod.Get, ProjectsUri).WithExactQueryString(string.Empty).Respond(HttpStatusCode.OK, NextLink("page-2"), JsonMediaType, Projects(FirstProjectId, ProjectNames[0]));
            this.server.Expect(HttpMethod.Get, ProjectsUri).WithQueryString("page[after]", "page-2").Respond(JsonMediaType, Projects(SecondProjectId, ProjectNames[1]));
        }

        private static void Fail(MockedRequest request, string failure)
        {
            switch (failure)
            {
                case "status":
                    request.Respond(HttpStatusCode.InternalServerError);
                    break;
                case "json":
                    request.Respond(JsonMediaType, "{ not json");
                    break;
                case "network":
                    request.Throw(new HttpRequestException("No such host"));
                    break;
                default:
                    request.Throw(new TaskCanceledException("Timed out"));
                    break;
            }
        }
    }
}
