# dotnet-quickstart-template
template with all essential tools and configs for basic dotnet api

## Usage : for use this template run below  commands

1. `dotnet new --install QuickstartTemplate`

2. `dotnet new quickstart -n {project name}`

### Features

* add api versioning ([ef3529b](https://github.com/Azaferany/dotnet-quickstart-template/commit/ef3529b016681b50b7e1493c30eb2e1d864b206c))
* add Cors ([46aa976](https://github.com/Azaferany/dotnet-quickstart-template/commit/46aa976f2341acc99225785627bc1c60edb2a127))
* add distributed cache ([5e05da7](https://github.com/Azaferany/dotnet-quickstart-template/commit/5e05da771bb4f42ce4fa85ad4affe0f264e0d22c))
* add SystemClock ([63c9b86](https://github.com/Azaferany/dotnet-quickstart-template/commit/63c9b869bc4a73ed2bfaef0b062c9fd07c161940))
* add ef DbContext with ISoftDeletable & ITimeable ([41b61b3](https://github.com/Azaferany/dotnet-quickstart-template/commit/41b61b3b1beef0a2e9b5db5e8d6a7a40cd91c20d))
* add health checks ([5043179](https://github.com/Azaferany/dotnet-quickstart-template/commit/5043179e7f56347e87af69b9ea0856dca0ee5e57))
* add oauth2 (jwt + introspection) authentication & authorization ([051c72d](https://github.com/Azaferany/dotnet-quickstart-template/commit/051c72d40a26b909cd7fd2a0f6338cd7f99798ae))
* add OpenTelemetry tracing ([5b47897](https://github.com/Azaferany/dotnet-quickstart-template/commit/5b4789721cd50b7e7ab7fce7286dc98394287ee6))
* add prometheus metrics ([a565aa5](https://github.com/Azaferany/dotnet-quickstart-template/commit/a565aa5a14d1ad86219cb7ae45655d2e9120a006))
* add request logging ([8f4bde1](https://github.com/Azaferany/dotnet-quickstart-template/commit/8f4bde13e9f740de63d92003dde18e27a7fbd097))
* add sentry ([90ab194](https://github.com/Azaferany/dotnet-quickstart-template/commit/90ab194c9b67442f99d34b622c4baca621360aad))
* add swagger ([be7b149](https://github.com/Azaferany/dotnet-quickstart-template/commit/be7b1492341244a4b8811b0555fc548ad26b2919))
* add http client request + response logging ([a53c199](https://github.com/Azaferany/dotnet-quickstart-template/commit/a53c199a5751fda035dfd60272f9fef0581ab82d))
* add initial WebApplicationFactory for IntegrationTests ([9d4ccd3](https://github.com/Azaferany/dotnet-quickstart-template/commit/9d4ccd35e7dcf7f458734f3aa9dd889f23633b65))
* add localization ([081b5b3](https://github.com/Azaferany/dotnet-quickstart-template/commit/081b5b3e4409bacbc09bda42b7e05a6081f920bd))
* add Metrics for all HttpClients(provided by HttpClientFactory) ([726a11e](https://github.com/Azaferany/dotnet-quickstart-template/commit/726a11e9d6e5b423e219dc4fae5578614bc9f317))
* add migrations script ([79decf2](https://github.com/Azaferany/dotnet-quickstart-template/commit/79decf2afe31466945978cd6c053909e73a9b389))
* add startup.cs ([08bdbe3](https://github.com/Azaferany/dotnet-quickstart-template/commit/08bdbe36569aad6ce07208e7620072a7ca26ac61))
* add Swagger Snapshot Tests ([925f6d1](https://github.com/Azaferany/dotnet-quickstart-template/commit/925f6d1a656807510db623b4cacd729eb080dcd5))
* add test for ISoftDeletable & ITimeable Auto fill properties ([#3](https://github.com/Azaferany/dotnet-quickstart-template/issues/3)) ([f0efc66](https://github.com/Azaferany/dotnet-quickstart-template/commit/f0efc660b2ec5588b2b1a8b468535fb20ad92870))
* add user detail tests ([#2](https://github.com/Azaferany/dotnet-quickstart-template/issues/2)) ([f4db540](https://github.com/Azaferany/dotnet-quickstart-template/commit/f4db540eca6b970616b4bd159f0e75aa8f823caf))
* enrich logs with trace ids ([8728d6d](https://github.com/Azaferany/dotnet-quickstart-template/commit/8728d6da8657586db6fb506dcd907e7cb76b2720))
* fill httpContext.User.Identity.Name correct and use it directly ([6ecd0ef](https://github.com/Azaferany/dotnet-quickstart-template/commit/6ecd0ef274ca7cdf995004f92013ff1b88a50831))
* include xml comments in swagger docs ([6029df7](https://github.com/Azaferany/dotnet-quickstart-template/commit/6029df73d32dda55303ec1e9288a533b97b1ffee))
* init clean architecture structure ([8d429d7](https://github.com/Azaferany/dotnet-quickstart-template/commit/8d429d7e5b04a16fc8bae70733469e43c7fdc1ef))
