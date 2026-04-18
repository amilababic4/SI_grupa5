namespace SmartLib.API.Middleware
{
    /// <summary>
    /// JWT Middleware — Validacija tokena i postavljanje korisničkog konteksta
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // TODO: Implementirati JWT validaciju
            // 1. Izvući token iz Authorization headera
            // 2. Validirati token
            // 3. Postaviti korisnika u HttpContext
            // 4. Provjeriti ulogu za autorizaciju

            await _next(context);
        }
    }
}
