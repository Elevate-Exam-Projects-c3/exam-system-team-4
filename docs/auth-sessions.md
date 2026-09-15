# Session refresh and logout

## Endpoints

| Endpoint | Request | Successful response |
| --- | --- | --- |
| `POST /api/auth/login` | Login ViewModel in JSON | Access token in `data` and a refresh cookie |
| `POST /api/auth/refresh` | Refresh cookie; no request body | New access token in `data` and a replacement refresh cookie |
| `POST /api/auth/logout` | Current refresh cookie; no request body | `200` response and an expired refresh cookie |

The access token response contains `accessToken`, `expiresIn` (seconds), and
`tokenType`. The raw refresh token and its expiry are excluded from JSON.

Refresh and logout use `AllowAnonymous` so they remain reachable after an access
token expires. Refresh still requires a valid refresh cookie. Logout can also be
repeated when the cookie is absent, unknown, or already revoked.

## Refresh

The refresh orchestrator rejects missing, unknown, used, revoked, or expired
tokens with `401`. The account must still exist, be active, have a confirmed
email, have at least one role, and not be locked out.

The new JWT uses the account's current roles and includes `studentId` when the
account has a student profile. A conditional database update marks the old token
`IsUsed = true` and records the replacement hash. The old-token update and the
new-token insertion commit in one transaction before the controller replaces
the cookie. A failed rotation does not return the generated tokens.

Reuse is rejected with `401` and publishes a warning notification containing
only the user ID and token record ID. A competing refresh that loses the
conditional update rereads the token and reports reuse if it has become used.

## Logout

Logout finds the token using the hash of the supplied cookie, then sets
`IsRevoked = true` on that token record. It then follows replacement links and
revokes any descendant refresh tokens in the same transaction. Each token is
revoked before its replacement is read, so a concurrent rotation cannot leave
a usable replacement behind. Used, expired, and already revoked records are
also followed. The controller clears the cookie after the transaction succeeds. If
server-side revocation fails, the cookie is retained so the request can be
retried.

Logout revokes the supplied refresh token and its replacements; it does not
revoke independently issued tokens from other logins. An already issued access
JWT remains valid until its expiry. The client
must discard its access token after logout.

## Cookie and authorization

Login, refresh, and logout share `RefreshTokenCookie`, with the same cookie name
(`refreshToken`), path (`/api/auth`), `HttpOnly`, `Secure`, and `SameSite=Lax`
settings for creation and deletion. Use HTTPS and a same-site browser client.
Session responses use `Cache-Control: no-store` and `Pragma: no-cache`.

Protected controllers validate the JWT signature, issuer, audience, and lifetime
with zero clock skew. Missing, expired, or invalid bearer tokens receive `401`.
A valid token without the required role receives `403`. The implemented quiz
creation and update controllers require the `Admin` role.

## Browser integration

This repository contains the backend. Automatic renewal must be connected in
the browser application's API client:

1. Include cookies in login, refresh, and logout requests (`credentials: "include"`
   with `fetch`). Keep the access token in memory and send it as a bearer token
   to protected endpoints.
2. Use `expiresIn` to renew before expiry, or handle a protected endpoint's `401`
   by calling refresh and retrying the original request once.
3. Share one in-flight refresh request across concurrent API calls and coordinate
   across browser tabs. Sending the same refresh token twice is treated as reuse.
4. A `401` from refresh ends renewal and requires login. Do not recursively refresh
   the refresh endpoint, or refresh in response to a role-related `403`.
5. Before logout, stop scheduling refreshes and wait for any in-flight refresh
   to finish, then send logout using the latest cookie. Clear the access token on
   success. Do not run logout and refresh concurrently.

The signing key must be supplied through the existing `Jwt:Key` configuration
(for example, development User Secrets or the `Jwt__Key` environment variable).
Keep it stable across requests and use the same key for all instances of the API.
