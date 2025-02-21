using DemoAPI.BLL.CustomExceptions;

namespace DemoAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { 
                await _next(context);
            }catch(Exception ex)
            {
                int statusCode = 0;
                switch (ex)
                {
                    case EmailAlreadyExistsExecption:
                    case InvalidLoginException:
                        // 400 - erreur de la part de l'utilisateur (de mauvaise donnée lors de l'input)
                        context.Response.StatusCode = 400;
                        break;

                    case UtilisateurNotFoundException:
                        // 404 - la resource n'existe pas
                        context.Response.StatusCode = 404;
                        break;
                    default:
                        // 500 - l'erreur provient du serveur (merci de réessayer plus tard)
                        context.Response.StatusCode = 500;
                        break;
                }

                string responseMessage = context.Response.StatusCode == 500 ? "Server error" :  ex.Message;

                // TODO logging et StatusCode = 500, alors il faudrait écrire des logs

                await context.Response.WriteAsync(responseMessage);
            }
        }
    }
}
