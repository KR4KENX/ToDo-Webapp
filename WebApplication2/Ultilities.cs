namespace WebApplication2
{
    public class Ultilities
    {
        public bool IsLogged(HttpContext context)
        {
            var username = context.Session.GetString(SessionVariables.SessionKeyUsername);
            var sessionId = context.Session.GetString(SessionVariables.SessionKeySessionId);

            if (username == null || sessionId == null)
                return false;
            return true;
        }
    }
}
