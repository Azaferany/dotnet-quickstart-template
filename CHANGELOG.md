## 1.0.0 (2022-08-01)


### ⚠ BREAKING CHANGES

* the IdentityExtensions.cs and httpContext.User.UserId() method removed
* the IDateTimeProvider remove and replace with ISystemClock from Microsoft.Extensions.Internal

### Features

* add api versioning ([ef3529b](https://github.com/Azaferany/dotnet-quickstart-template/commit/ef3529b016681b50b7e1493c30eb2e1d864b206c))
* add Cors ([46aa976](https://github.com/Azaferany/dotnet-quickstart-template/commit/46aa976f2341acc99225785627bc1c60edb2a127))
* add distributed cache ([5e05da7](https://github.com/Azaferany/dotnet-quickstart-template/commit/5e05da771bb4f42ce4fa85ad4affe0f264e0d22c))
* add ef DbContext with ISoftDeletable & ITimeable ([41b61b3](https://github.com/Azaferany/dotnet-quickstart-template/commit/41b61b3b1beef0a2e9b5db5e8d6a7a40cd91c20d))
* add health checks ([5043179](https://github.com/Azaferany/dotnet-quickstart-template/commit/5043179e7f56347e87af69b9ea0856dca0ee5e57))
* add oauth2 (jwt + introspection) authentication & authorization ([051c72d](https://github.com/Azaferany/dotnet-quickstart-template/commit/051c72d40a26b909cd7fd2a0f6338cd7f99798ae))
* add OpenTelemetry tracing ([5b47897](https://github.com/Azaferany/dotnet-quickstart-template/commit/5b4789721cd50b7e7ab7fce7286dc98394287ee6))
* add prometheus metrics ([a565aa5](https://github.com/Azaferany/dotnet-quickstart-template/commit/a565aa5a14d1ad86219cb7ae45655d2e9120a006))
* add request logging ([8f4bde1](https://github.com/Azaferany/dotnet-quickstart-template/commit/8f4bde13e9f740de63d92003dde18e27a7fbd097))
* add sentry ([90ab194](https://github.com/Azaferany/dotnet-quickstart-template/commit/90ab194c9b67442f99d34b622c4baca621360aad))
* add swagger ([be7b149](https://github.com/Azaferany/dotnet-quickstart-template/commit/be7b1492341244a4b8811b0555fc548ad26b2919))


### Bug Fixes

* add HttpContextAccessor to fix ITimeable entities UserId prop ([92e0989](https://github.com/Azaferany/dotnet-quickstart-template/commit/92e098996e4f27bcb6cd6baeff71ec5b7acce8ca))
* add Serilog.Expressions for serilog filter ([9788015](https://github.com/Azaferany/dotnet-quickstart-template/commit/9788015ae059f9d9aa48b43a34316405f296eb8c))


### Performance Improvements

* add http client request + response logging ([a53c199](https://github.com/Azaferany/dotnet-quickstart-template/commit/a53c199a5751fda035dfd60272f9fef0581ab82d))
* add IDateTimeProvider ([11cdfe8](https://github.com/Azaferany/dotnet-quickstart-template/commit/11cdfe8ded69ca1e135397b421964737416c2ae4))
* add localization ([081b5b3](https://github.com/Azaferany/dotnet-quickstart-template/commit/081b5b3e4409bacbc09bda42b7e05a6081f920bd))
* add Metrics for all HttpClients(provided by HttpClientFactory) ([726a11e](https://github.com/Azaferany/dotnet-quickstart-template/commit/726a11e9d6e5b423e219dc4fae5578614bc9f317))
* add migrations script ([79decf2](https://github.com/Azaferany/dotnet-quickstart-template/commit/79decf2afe31466945978cd6c053909e73a9b389))
* add startup.cs ([08bdbe3](https://github.com/Azaferany/dotnet-quickstart-template/commit/08bdbe36569aad6ce07208e7620072a7ca26ac61))
* enrich logs with trace ids ([8728d6d](https://github.com/Azaferany/dotnet-quickstart-template/commit/8728d6da8657586db6fb506dcd907e7cb76b2720))
* fill httpContext.User.Identity.Name correct and use it directly ([6ecd0ef](https://github.com/Azaferany/dotnet-quickstart-template/commit/6ecd0ef274ca7cdf995004f92013ff1b88a50831))
* include xml comments in swagger docs ([6029df7](https://github.com/Azaferany/dotnet-quickstart-template/commit/6029df73d32dda55303ec1e9288a533b97b1ffee))
* init clean architecture structure ([8d429d7](https://github.com/Azaferany/dotnet-quickstart-template/commit/8d429d7e5b04a16fc8bae70733469e43c7fdc1ef))
* move configs to appsettings.json ([c8d1c77](https://github.com/Azaferany/dotnet-quickstart-template/commit/c8d1c77962a157587a53325fe82352bd22dbf4ca))
* move sentry configs to appsettings.json ([55cf04a](https://github.com/Azaferany/dotnet-quickstart-template/commit/55cf04a2fd034e4d0803e446ee9bb9aa7efb5e96))
* remove UtcDateTimeProvider and use dotnet SystemClock ([63c9b86](https://github.com/Azaferany/dotnet-quickstart-template/commit/63c9b869bc4a73ed2bfaef0b062c9fd07c161940))