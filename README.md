# ASP.NET Core with ZITADEL

[ASP.NET Core](https://learn.microsoft.com/aspnet/core/) is a powerful, production-ready framework for building web applications. It adopts an opinionated view of the platform and tooling so you can get started with minimum fuss. ASP.NET Core provides the libraries and patterns to build secure web applications efficiently.

To secure such an application, you need a reliable way to handle user logins. For ASP.NET Core applications, the built-in OpenID Connect handler is the standard and recommended approach for authentication and access control. Think of it as a flexible security guard for your app. This guide demonstrates how to use the OIDC handler with an ASP.NET Core application to implement a secure login with ZITADEL.

We'll be using the **OpenID Connect (OIDC)** protocol with the **Authorization Code Flow + PKCE**. This is the industry-best practice for security, ensuring that the login process is safe from start to finish. You can learn more in our [guide to OAuth 2.0 recommended flows](https://zitadel.com/docs/guides/integrate/login/oidc/oauth-recommended-flows).

This example uses the built-in OIDC handler, which is highly modular and supports OIDC natively. It handles communication with ZITADEL using the powerful OIDC standard to manage the secure PKCE flow automatically.

Check out our Example Application to see it in action.

## Example Application

The example repository includes a complete ASP.NET Core MVC application, ready to run, that demonstrates how to integrate ZITADEL for user authentication.

This example application showcases a typical web app authentication pattern: users start on a public landing page, click a login button to authenticate with ZITADEL, and are then redirected to a protected profile page displaying their user information. The app also includes secure logout functionality that clears the session and redirects users back to ZITADEL's logout endpoint. All protected routes are automatically secured using the authentication/authorization pipeline and session management, ensuring only authenticated users can access sensitive areas of your application.

### Prerequisites

Before you begin, ensure you have the following:

#### System Requirements

- .NET SDK 8 or later (devbox provides it)

#### Account Setup

You'll need a ZITADEL account and application configured. Follow the [ZITADEL documentation on creating applications](https://zitadel.com/docs/guides/integrate/login/oidc/web-app) to set up your account and create a Web application with Authorization Code + PKCE flow.

> **Important:** Configure the following URLs in your ZITADEL application settings:
>
>- **Redirect URIs:** Add `http://localhost:3000/auth/callback` (for development)
>- **Post Logout Redirect URIs:** Add `http://localhost:3000/auth/logout/callback` (for development)
>
>  These URLs must exactly match what your ASP.NET Core application uses. For production, add your production URLs.

### Configuration

To run the application, you first need to copy the `.env.example` file to a new file named `.env` and fill in your ZITADEL application credentials.

```dotenv
# Port number where your server will listen for incoming HTTP requests.
PORT=3000

# Session timeout in seconds. Users will be automatically logged out after this
# duration of inactivity. 3600 seconds = 1 hour.
SESSION_DURATION=3600

# Your ZITADEL instance domain URL. Found in your ZITADEL console under
# instance settings. Include the full https:// URL.
ZITADEL_DOMAIN="https://your-zitadel-domain"

# Application Client ID from your ZITADEL application settings.
ZITADEL_CLIENT_ID="your-client-id"

# While the Authorization Code Flow with PKCE for public clients
# does not strictly require a client secret for OIDC specification compliance,
# provide a value here for the handler.
ZITADEL_CLIENT_SECRET="your-randomly-generated-client-secret"

# Full URL where ZITADEL redirects after authentication.
ZITADEL_CALLBACK_URL="http://localhost:3000/auth/callback"

# Full URL where ZITADEL redirects after logout.
ZITADEL_POST_LOGOUT_URL="http://localhost:3000/auth/logout/callback"
```

### Installation and Running

Follow these steps to get the application running:

```bash
# Start the development server (restores dependencies automatically)
make start
```

The application will now be running at `http://localhost:3000`.

## Key Features

### PKCE Authentication Flow

The application implements the secure Authorization Code Flow with PKCE (Proof Key for Code Exchange), which is the recommended approach for modern web applications. The handler generates and validates the verifier/challenge automatically.

### Session Management

Built-in session management handles user authentication state across your application, with secure cookie storage.

### Route Protection

Protected routes automatically redirect unauthenticated users to the login flow, ensuring sensitive areas of your application remain secure.

### Logout Flow

Complete logout implementation that properly terminates both the local session and the ZITADEL session, with proper redirect handling to the configured callback page.

## TODOs

### 1. Security headers

Consider adding security headers middleware (e.g., CSP, Referrer-Policy, Permissions-Policy) appropriate for your deployment.
