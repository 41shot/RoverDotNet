# RoverDotNet.Config

`RoverDotNet.Config` implements Rover's `config` sub-commands as native .NET classes: `whoami` and `auth`. It mirrors the behaviour of Rover's `src/command/config/whoami.rs` and `src/command/config/auth.rs`.

## `ConfigAuth` — `config auth`

Prompts for an Apollo Studio API key (via an injected `IApiKeyPrompt`), validates it, and stores it in a named profile using `ProfileConfig`.

```csharp
var auth = new ConfigAuth(profileConfig, apiKeyPrompt);
AuthResult result = await auth.ExecuteAsync(profileName: "default", cancellationToken);
```

- `profileName` defaults to `ProfileConfig.DefaultProfile` ("default") if not supplied.
- Throws `RoverException` if the API key is empty or invalid.
- After storing, the credential is read back to verify persistence (matching Rover's behaviour).
- Returns an `AuthResult` with a confirmation message suggesting `config whoami` to verify authentication.

## `ConfigWhoAmI` — `config whoami`

Loads the stored credential for a profile and verifies it against Apollo Studio via `WhoAmIOperation`, returning display-ready identity details.

```csharp
var whoAmI = new ConfigWhoAmI(profileConfig, whoAmIOperation);
WhoAmIResult result = await whoAmI.ExecuteAsync(profileName: "default", unmaskKey: false, cancellationToken);
```

- `unmaskKey` controls whether the raw API key is included in the result (`false` by default — the key is masked via `ApiKeyMasker`).
- Throws `InvalidKeyException` if the key isn't recognised by Studio, or its actor type is neither `User` nor `Graph`.
- The returned `WhoAmIResult` includes the (masked or raw) API key, actor type, user/graph id, graph title, and the credential's origin (e.g. `--profile <name>` or the `APOLLO_KEY` environment variable).

## Dependencies

Both classes rely on `RoverDotNet.Core` for shared configuration/credential types (`ProfileConfig`, `CredentialOrigin`, `RoverException`, `InvalidKeyException`) and on `RoverDotNet.Client` for the Apollo Studio `WhoAmIOperation` used by `ConfigWhoAmI`.

## Usage notes

- Credentials can originate from a named profile stored in the local config file, or from the `APOLLO_KEY` environment variable — `WhoAmIResult.Origin` reports which was used.
- These classes are designed to be consumed directly by other .NET hosts (e.g. the `RoverDotNet` CLI or the WinForms demo), rather than exposing their own CLI surface.
