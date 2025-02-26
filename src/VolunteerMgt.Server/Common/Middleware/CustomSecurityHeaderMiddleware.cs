namespace VolunteerMgt.Server.Common.Middleware;
public class CustomSecurityHeaderMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //The X-Frame-Options header option is used to call your web page, called Clickjacking, on another web page with the iframe method and prevent any action.
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        //It is used to prevent man-in-the-middle (MITM) attacks and automatically convert HTTP requests made by the client to HTTPS
        // context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

        //The X-XSS-Protection header causes browsers to stop loading the web page when they detect a cross-site scripting attack.
        context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");

        //It is used to prevent browsers from determining the MIME type sent with the Content Type header in requests sent from the client.
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        //When a site accesses a different site, it sends its own address with a referrer. In some cases, the Referrer-Policy header is used when it is not desired to send the source address explicitly.
        //context.Response.Headers.Append("Referrer-Policy", "no-referrer");

        //The app’s camera, microphone, usb etc. It is the title that we determine whether or not it will need such requirements.
        //context.Response.Headers.Append("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");

        //Content-Security-Policy is a security policy used to control data injection attacks that may occur due to a web page’s style and script files.
        //context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");

        await next.Invoke(context);
    }
}
